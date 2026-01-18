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

        public async Task<TradesVM> LoadSampleSizeAsync(int sampleSizeId)
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
            if (updateResearchData.Strategy == EStrategy.SRS)
            {
                SRS srs = JsonConvert.DeserializeObject<SRS>(updateResearchData.Data!)!;
                await _unitOfWork.SRS.UpdateAsync(srs);
                await _unitOfWork.SaveAsync();
            }
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
            return [.. await _unitOfWork.SampleSize.GetAllAsync(sampleSize => sampleSize.TradeType == ETradeType.Trade, includeProperties: "Review")];
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
            if (_tradesVM.CurrentSampleSize.Strategy == EStrategy.SRS)
            {
                _tradesVM.AllTradesInSampleSize = [.. (await _unitOfWork.SRS.GetAllAsync(trade => trade.SampleSizeId == _tradesVM.CurrentSampleSize.Id,
                                                                                        includeProperties: "Journal")).OrderBy(t => t.Id).Cast<object>()];

                // Set current trade and calculate its position
                _tradesVM.CurrentTrade = _tradesVM.AllTradesInSampleSize.Last() as BaseTrade;
                _tradesVM.SRSTrade = _tradesVM.AllTradesInSampleSize.Last() as SRS;
            }
        }

        private async Task SetAvailableMenus(List<SampleSize> sampleSizes = null, EStatus? status = null)
        {
            if (status != null && status != EStatus.All)
            {
                _allSampleSizes = sampleSizes;
            }
            else if (_allSampleSizes == null)
            {
                _allSampleSizes = await _unitOfWork.SampleSize.GetAllAsync();
            }

            SetTimeframesAndStrategies();
            SortMenus();
        }

        private void SetTimeframesAndStrategies()
        {
            foreach (SampleSize sampleSize in _allSampleSizes)
            {
                if (!_tradesVM.AvailableTimeframes.Contains(sampleSize.TimeFrame))
                {
                    _tradesVM.AvailableTimeframes.Add(sampleSize.TimeFrame);
                }
                if (!_tradesVM.AvailableStrategies.Contains(sampleSize.Strategy))
                {
                    _tradesVM.AvailableStrategies.Add(sampleSize.Strategy);
                }
            }
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
