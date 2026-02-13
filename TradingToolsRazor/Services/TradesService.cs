using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Models;
using Models.RequestModels;
using Models.ViewModels;
using Models.ViewModels.DisplayClasses;
using Newtonsoft.Json;
using Shared;
using SharedEnums.Enums;
using System.Diagnostics;
using System.IO.Compression;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;
using TradingToolsRazor.Services.Interfaces;
using Utilities;
using Utilities.Trade;

namespace TradingToolsRazor.Services
{
    public class TradesService(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, DeleteTradeService deleteTradeService) : ITradesService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private readonly IWebHostEnvironment webHostEnvironment = webHostEnvironment;
        private readonly DeleteTradeService _deleteTradeService = deleteTradeService;
        private List<SampleSize> _allSampleSizes = [];
        private TradesVM _tradesVM = new();

        public async Task<TradesVM> LoadTypeAsync(SampleSizeType sampleSizeType, Strategy strategy)
        {
            _allSampleSizes = await _unitOfWork.SampleSize.GetAllAsync(sampleSize => sampleSize.SampleSizeType == sampleSizeType, includeProperties: "Review");
            if (!_allSampleSizes.Any())
            {
                await InitializeTradesViewModelAsync();
            }
            await SetViewModel(strategy);
            return _tradesVM;
        }

        public async Task<TradesVM> LoadStrategyAsync(Strategy strategy, SampleSizeType sampleSizeType)
        {
            _allSampleSizes = await _unitOfWork.SampleSize.GetAllAsync(sampleSize => sampleSize.SampleSizeType == sampleSizeType, includeProperties: "Review");
            if (!_allSampleSizes.Any())
            {
                await InitializeTradesViewModelAsync();
            }
            await SetViewModel(strategy);
            return _tradesVM;
        }

        public async Task<TradesVM> LoadTimeFrameAsync(TradesLoadTimeFrameRequestModel requestModel)
        {
            _allSampleSizes = await _unitOfWork.SampleSize.GetAllAsync(sampleSize => sampleSize.Strategy == requestModel.Strategy &&
                                                                                     sampleSize.SampleSizeType == requestModel.SampleSizeType &&
                                                                                     sampleSize.TimeFrame == requestModel.TimeFrame, 
                                                                                     includeProperties: "Review");

            if (!_allSampleSizes.Any())
            {
                await InitializeTradesViewModelAsync();
            }

            await SetViewModel();

            return _tradesVM;
        }

        public async Task<TradesVM> LoadSampleSizeNumberAsync(int sampleSizeId)
        {
            _allSampleSizes = await GetAllSampleSizes();

            if (!_allSampleSizes.Any())
            {
                return _tradesVM;
            }

            await SetViewModel(sampleSizeId);

            return _tradesVM;
        }

        public async Task<TradesVM> InitializeTradesViewModelAsync()
        {
            _allSampleSizes = await GetAllSampleSizes();

            if (!_allSampleSizes.Any())
            {
                return _tradesVM;
            }

            await SetViewModel();
            _tradesVM.ErrorMsg = _tradesVM.CurrentTrade is null ? "No trade for the selected paramaters." : string.Empty;

            return _tradesVM;
        }

        public TradesVM InitializeNewTradeTradesViewModel()
        {
            return new();
        }

        public async Task UpdateTradeDataAsync(BaseTrade tradeData)
        {
            await _unitOfWork.BaseTrade.UpdateAsync(tradeData);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateResearchData([FromBody] UpdateResearchDataModel updateResearchData)
        {
            switch (updateResearchData.Strategy)
            {
                case Strategy.SRS:
                    await UpdateSRSResearchData(updateResearchData);
                    break;

                case Strategy.BrunchBreak:
                    await UpdateBrunchBreakResearchData(updateResearchData);
                    break;
            }
        }

        private async Task UpdateBrunchBreakResearchData(UpdateResearchDataModel updateResearchData)
        {
            BrunchBreak brunchBreak = JsonConvert.DeserializeObject<BrunchBreak>(updateResearchData.Data!)!;
            await _unitOfWork.BrunchBreak.UpdateAsync(brunchBreak);
            await _unitOfWork.SaveAsync();
        }

        private async Task UpdateSRSResearchData(UpdateResearchDataModel updateResearchData)
        {
            SRS srs = JsonConvert.DeserializeObject<SRS>(updateResearchData.Data!)!;
            await _unitOfWork.SRS.UpdateAsync(srs);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateReviewAsync(Review review)
        {
            await _unitOfWork.Review.UpdateAsync(review);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateJournalAsync(Journal journal)
        {
            await _unitOfWork.Journal.UpdateAsync(journal);
            await _unitOfWork.SaveAsync();
        }

        #region Helper Methods - General

        private async Task<List<SampleSize>> GetAllSampleSizes()
        {
            return [.. await _unitOfWork.SampleSize.GetAllAsync(sampleSize => sampleSize.SampleSizeType != SampleSizeType.Research, includeProperties: "Review")];
        }

        private async Task SetViewModel(Strategy strategy)
        {
            _tradesVM.CurrentSampleSize = _allSampleSizes.Where(sampleSize => sampleSize.Strategy == strategy).Last();
            _tradesVM.SampleSizes = _allSampleSizes;

            await SetCurrentTrade();
            await SetAvailableMenus(strategy);
        }

        // Overload that accepts a specific sample size ID
        private async Task SetViewModel(int sampleSizeId)
        {
            _tradesVM.CurrentSampleSize = _allSampleSizes.FirstOrDefault(sampleSize => sampleSize.Id == sampleSizeId)!;
            _tradesVM.SampleSizes = _allSampleSizes;

            await SetCurrentTrade();
            await SetAvailableMenus();
        }

        private async Task SetViewModel()
        {
            _tradesVM.CurrentSampleSize = _allSampleSizes.Last();
            _tradesVM.SampleSizes = _allSampleSizes;

            await SetCurrentTrade();
            await SetAvailableMenus();
        }


        private async Task SetCurrentTrade()
        {
            switch (_tradesVM.CurrentSampleSize.Strategy)
            {
                case Strategy.SRS:
                    await SetSRSCurrentTrade();
                    break;

                case Strategy.BrunchBreak:
                    await SetBrunchBreakCurrentTrade();
                    break;
                default:
                    throw new ArgumentException($"Strategy {_tradesVM.CurrentSampleSize.Strategy.ToString()} not implemented in {nameof(TradesService)}.{nameof(SetCurrentTrade)}");
            }
        }

        private async Task SetBrunchBreakCurrentTrade()
        {
            _tradesVM.AllTradesInSampleSize = [.. (await _unitOfWork.BrunchBreak.GetAllAsync(trade => trade.SampleSizeId == _tradesVM.CurrentSampleSize.Id,
                                                                                            includeProperties: "Journal")).OrderBy(t => t.Id).Cast<object>()];

            _tradesVM.CurrentTrade = _tradesVM.AllTradesInSampleSize.Last() as BaseTrade;
            _tradesVM.BrunchBreakTrade = _tradesVM.AllTradesInSampleSize.Last() as BrunchBreak;
        }

        private async Task SetSRSCurrentTrade()
        {
            _tradesVM.AllTradesInSampleSize = [.. (await _unitOfWork.SRS.GetAllAsync(trade => trade.SampleSizeId == _tradesVM.CurrentSampleSize.Id,
                                                                                            includeProperties: "Journal")).OrderBy(t => t.Id).Cast<object>()];

            _tradesVM.CurrentTrade = _tradesVM.AllTradesInSampleSize.Last() as BaseTrade;
            _tradesVM.SRSTrade = _tradesVM.AllTradesInSampleSize.Last() as SRS;
        }

        private async Task SetAvailableMenus(Strategy strategy)
        {
            SetSampleSizeMenu(strategy);
            await SetTimeFrames();
            await SetStrategies();
            SortMenus();
        }

        private async Task SetAvailableMenus()
        {
            SetSampleSizeMenu();
            await SetTimeFrames();
            await SetStrategies();
            SortMenus();
        }

        private async Task SetStrategies()
        {
            var sampleSizes = (await GetAllSampleSizes()).Where(sampleSize => sampleSize.SampleSizeType == _tradesVM.CurrentSampleSize.SampleSizeType);
            foreach (SampleSize sampleSize in sampleSizes)
            {
                if (!_tradesVM.AvailableStrategies.Contains(sampleSize.Strategy))
                {
                    _tradesVM.AvailableStrategies.Add(sampleSize.Strategy);
                }
            }
        }

        private async Task SetTimeFrames()
        {
            var sampleSizesForCurrentStrategy = (await GetAllSampleSizes())
                .Where(sampleSize => sampleSize.Strategy == _tradesVM.CurrentSampleSize.Strategy && 
                       sampleSize.SampleSizeType == _tradesVM.CurrentSampleSize.SampleSizeType);

            foreach (SampleSize sampleSize in sampleSizesForCurrentStrategy)
            {
                if (!_tradesVM.AvailableTimeframes.Contains(sampleSize.TimeFrame))
                {
                    _tradesVM.AvailableTimeframes.Add(sampleSize.TimeFrame);
                }
            }
        }

        private void SetSampleSizeMenu(Strategy strategy)
        {
            _tradesVM.SampleSizes = [.. _allSampleSizes.Where(sampleSize => sampleSize.Strategy == strategy && sampleSize.TimeFrame == _tradesVM.CurrentSampleSize.TimeFrame)];
        }

        private void SetSampleSizeMenu()
        {
            _tradesVM.SampleSizes = [.. _allSampleSizes.Where(sampleSize => sampleSize.TimeFrame == _tradesVM.CurrentSampleSize.TimeFrame &&
                                                                            sampleSize.SampleSizeType == _tradesVM.CurrentSampleSize.SampleSizeType &&
                                                                            sampleSize.Strategy == _tradesVM.CurrentSampleSize.Strategy)];
        }

        private void SortMenus()
        {
            _tradesVM.AvailableTimeframes.Sort();
            _tradesVM.AvailableStrategies.Sort();
        }

        #endregion

        public async Task DeleteTrade(DeleteTradeRequestModel deleteTradeRequest)
        {
            await _deleteTradeService.DeleteTrade(deleteTradeRequest.Strategy, deleteTradeRequest.Id, webHostEnvironment.WebRootPath);
        }
    }
}
