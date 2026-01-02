using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http;
using Models;
using Models.ViewModels;
using Models.ViewModels.DisplayClasses;
using SharedEnums.Enums;
using System.Diagnostics;
using TradingToolsRazor.Services.Interfaces;
using Utilities;

namespace TradingToolsRazor.Services
{
    public class NewTradeService : INewTradeService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private NewTradeVM _viewModel = null!;
        private IFormFile[] _files = null!;

        public NewTradeService(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task SaveTradeAsync(NewTradeVM viewModel, IFormFile[] files)
        {
            _viewModel = viewModel;
            _files = files;

            if (_viewModel.SampleSizeViewData.TradeType == ETradeType.Research)
            {
                await SaveResearchTradeAsync();
            }
            else
            {
                await SaveTradeAsync();
            }
        }

        private async Task SaveResearchTradeAsync()
        {
            switch (_viewModel.Strategy)
            {
                case EStrategy.FirstBarPullback:
                    await SaveResearchDataFirstbarPullback(maxTradesProSampleSize: 100);
                    break;
                case EStrategy.Cradle:
                    await SaveResearchCradleData(maxTradesProSampleSize: 100);
                    break;
                case EStrategy.CandleBracketing:
                    await SaveCandleBracketingData(maxTradesProSampleSize: 200);
                    break;
            }
        }

        private async Task SaveTradeAsync()
        {
            switch (_viewModel.SampleSizeViewData.Strategy)
            {
                case EStrategy.FirstBarPullback:
                    await SaveResearchFirstBarPullbackTrade();
                    break;
                case EStrategy.SRS:
                    await SaveSRSTrade();
                    break;
            }
        }

        private async Task SaveResearchFirstBarPullbackTrade()
        {
            var researchData = await SaveResearchDataFirstbarPullback(maxTradesProSampleSize: 20);
            var newTrade = SetNewTradeData(researchData, _files, 20);
            newTrade.JournalId = await CreateJournal();

            _unitOfWork.Trade.Add(newTrade);
            await _unitOfWork.SaveAsync();
        }

        private async Task SaveSRSTrade()
        {
            try
            {
                var (sampleSizeId, isFull) = await ProcessSampleSize(maxTradesProSampleSize: 20, _viewModel.SRSTrade.IsFlippedTheSwitch);
                _viewModel.SRSTrade.SampleSizeId = sampleSizeId;
                _viewModel.SRSTrade.JournalId = await CreateJournal();
                _viewModel.SRSTrade.ScreenshotsUrls = await ScreenshotsService.SaveFilesAsync(_webHostEnvironment.WebRootPath, _viewModel, _viewModel.SRSTrade, _files, isFull);

                _unitOfWork.SRS.Add(_viewModel.SRSTrade);
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error in NewTradeService.SaveSRSTrade(): {ex.Message}");
            }
           
        }

        private async Task<int> CreateJournal()
        {
            var journal = new Journal();
            _unitOfWork.Journal.Add(journal);
            await _unitOfWork.SaveAsync();

            return journal.Id;
        }

        private Trade SetNewTradeData(ResearchFirstBarPullback researchData, IFormFile[] files, int maxTradesProSampleSize)
        {
            var newTrade = EntityMapper.ViewModelDisplayToEntity<Trade, TradeDisplay>(_viewModel.TradeData, existingEntity: null);

            newTrade.ResearchId = researchData.Id;
            newTrade.SampleSizeId = researchData.SampleSizeId;
            newTrade.Status = _viewModel.Status;
            newTrade.SampleSize = researchData.SampleSize;

            return newTrade;
        }

        private async Task SaveCandleBracketingData(int maxTradesProSampleSize)
        {
            var viewData = _viewModel.ResearchData as ResearchCandleBracketing;
            var sampleSizeData = await ProcessSampleSize(maxTradesProSampleSize, viewData!.IsFlippedTheSwitch);
            viewData!.SampleSizeId = sampleSizeData.id;

            var researchData = new ResearchCandleBracketing();
            EntityMapper.ViewModelToEntity(researchData, viewData);
            researchData.SampleSizeId = sampleSizeData.id;
            researchData.ScreenshotsUrls = await ScreenshotsService.SaveFilesAsync(_webHostEnvironment.WebRootPath, _viewModel, viewData, _files, sampleSizeData.isFull);

            _unitOfWork.ResearchCandleBracketing.Add(researchData);
            try
            {
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                Debug.Write($"{ex.Message}");
            }
        }

        private async Task<ResearchCradle> SaveResearchCradleData(int maxTradesProSampleSize)
        {
            var viewData = _viewModel.ResearchData as ResearchCradle;
            var sampleSizeData = await ProcessSampleSize(maxTradesProSampleSize);
            viewData!.SampleSizeId = sampleSizeData.id;

            var researchData = new ResearchCradle();
            EntityMapper.ViewModelToEntity(researchData, viewData);
            researchData.SampleSizeId = sampleSizeData.id;
            researchData.ScreenshotsUrls = await ScreenshotsService.SaveFilesAsync(_webHostEnvironment.WebRootPath, _viewModel, viewData, _files, sampleSizeData.isFull);

            _unitOfWork.ResearchCradle.Add(researchData);
            await _unitOfWork.SaveAsync();

            return viewData;
        }

        private async Task<ResearchFirstBarPullback> SaveResearchDataFirstbarPullback(int maxTradesProSampleSize)
        {
            var viewData = _viewModel.ResearchData as ResearchFirstBarPullbackDisplay;
            var researchData = EntityMapper.ViewModelDisplayToEntity<ResearchFirstBarPullback, ResearchFirstBarPullbackDisplay>(viewData, existingEntity: null);

            var sampleSizeData = await ProcessSampleSize(maxTradesProSampleSize);
            researchData.SampleSizeId = sampleSizeData.id;

            researchData.ScreenshotsUrls = await ScreenshotsService.SaveFilesAsync(_webHostEnvironment.WebRootPath, _viewModel, researchData, _files, sampleSizeData.isFull);

            _unitOfWork.ResearchFirstBarPullback.Add(researchData);
            await _unitOfWork.SaveAsync();

            return researchData;
        }

        private async Task<(int id, bool isFull)> ProcessSampleSize(int maxTradesProSampleSize, bool isFlippedTheSwitch = false)
        {
            var sampleSizeData = await CheckLastSampleSize(maxTradesProSampleSize, isFlippedTheSwitch);

            if (sampleSizeData.isFull || sampleSizeData.id == 0)
            {
                Review review = new Review();
                _unitOfWork.Review.Add(review);
                await _unitOfWork.SaveAsync();

                var newSampleSize = new SampleSize
                {
                    Strategy = _viewModel.Strategy,
                    TimeFrame = _viewModel.TimeFrame,
                    TradeType = _viewModel.TradeType,
                    ReviewId = review?.Id
                };

                _unitOfWork.SampleSize.Add(newSampleSize);
                await _unitOfWork.SaveAsync();
                sampleSizeData.id = newSampleSize.Id;
            }

            return sampleSizeData;
        }

        private async Task<(int id, bool isFull)> CheckLastSampleSize(int maxTradesProSampleSize, bool isFlippedTheSwitch)
        {
            bool isFull = false;

            var listSampleSizes = await _unitOfWork.SampleSize.GetAllAsync(x =>
                x.TimeFrame == _viewModel.TimeFrame &&
                x.Strategy == _viewModel.Strategy &&
                x.TradeType == _viewModel.TradeType);

            if (!listSampleSizes.Any())
                return (0, false);

            var sampleSize = listSampleSizes.Last();

            int numberTradesInSampleSize = _viewModel.TradeType switch
            {
                ETradeType.Research when _viewModel.Strategy == EStrategy.FirstBarPullback =>
                    (await _unitOfWork.ResearchFirstBarPullback.GetAllAsync(x => x.SampleSizeId == sampleSize.Id)).Count,

                ETradeType.Research when _viewModel.Strategy == EStrategy.Cradle =>
                    (await _unitOfWork.ResearchCradle.GetAllAsync(x => x.SampleSizeId == sampleSize.Id)).Count,

                ETradeType.Research when _viewModel.Strategy == EStrategy.CandleBracketing =>
                    (await _unitOfWork.ResearchCandleBracketing.GetAllAsync(x => x.SampleSizeId == sampleSize.Id))
                        .Select(trade => trade.Date)
                        .Distinct()
                        .Count(),

                ETradeType.Trade =>
                    (await _unitOfWork.Trade.GetAllAsync(x => x.SampleSizeId == sampleSize.Id)).Count,

                _ => 0
            };

            if (sampleSize.Strategy == EStrategy.CandleBracketing && numberTradesInSampleSize == 100 && !isFlippedTheSwitch)
            {
                isFull = true;
            }
            else if (numberTradesInSampleSize == maxTradesProSampleSize)
            {
                isFull = true;
            }

            return (sampleSize.Id, isFull);
        }
    }
}
