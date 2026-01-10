using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
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

namespace TradingToolsRazor.Services
{
    public class TradesService(IUnitOfWork unitOfWork) : ITradesService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;
        private List<SampleSize> _allSampleSizes = [];
        private TradesVM _tradesVM = new();

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
            return new TradesVM { CurrentTrade = new() };
        }

        public async Task<TradesVM> LoadTradeAsync(LoadTradeParams tradeParams)
        {
            _tradesVM = new TradesVM();
            tradeParams.ConvertParamsFromView();

            List<SampleSize> sampleSizes = await GetSampleSizesForTradeParams(tradeParams);
            if (!sampleSizes.Any())
            {
                _tradesVM.ErrorMsg = "No sample sizes for the selected trade paramaters.";
                return _tradesVM;
            }

            SetCurrentSampleSize(sampleSizes, tradeParams);

            List<BaseTrade> listTrades = await GetAllTrades(tradeParams);
            await SetCurrentTrade();

            await SetViewData(sampleSizes, listTrades.Count, tradeParams);

            return _tradesVM;
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

        private async Task<List<SampleSize>> GetSampleSizes(List<int> sampleSizeIds)
        {
            List<SampleSize> list = new();
            foreach (int id in sampleSizeIds)
            {
                SampleSize sampleSize = await _unitOfWork.SampleSize.GetAsync(sampleSize => sampleSize.Id == id);
                list.Add(sampleSize);
            }

            return list;
        }

        private async Task SetViewModel()
        {
            _tradesVM.CurrentSampleSize = _allSampleSizes.Last();

            await SetCurrentTrade();
            SetMenuNumberSampleSizes();
            await SetAvailableMenus();
        }

        private async Task SetCurrentTrade()
        {
            if (_tradesVM.CurrentSampleSize.Strategy == EStrategy.SRS)
            {
                var trades = (await _unitOfWork.SRS.GetAllAsync(trade => trade.SampleSizeId == _tradesVM.CurrentSampleSize.Id, includeProperties: "Journal"));
                _tradesVM.CurrentTrade = trades.Last();
                _tradesVM.TradesInSampleSize = trades.Count;
            }
        }

        private void SetMenuNumberSampleSizes()
        {
            _tradesVM.NumberSampleSizes = _allSampleSizes.Where(x => x.Strategy == _tradesVM.CurrentSampleSize.Strategy && x.TimeFrame == _tradesVM.CurrentSampleSize.TimeFrame).Count();
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

        #region Helper Methods - LoadTrade

        private async Task<List<SampleSize>> GetSampleSizesForTradeParams(LoadTradeParams tradeParams)
        {
            List<int> sampleSizeIds = await GetSampleSizeIds(tradeParams);
            List<SampleSize> listSampleSizes = await GetSampleSizes(sampleSizeIds);
            CheckIfTradeParamTimeframeExistsInSampleSizes(listSampleSizes, tradeParams);

            return listSampleSizes;
        }

        private async Task<List<int>> GetSampleSizeIds(LoadTradeParams tradeParams)
        {
            if (tradeParams.Status == EStatus.All)
            {
                return [.. (await _unitOfWork.BaseTrade
                                    .GetAllAsync(trade =>
                                                    trade.SampleSize!.Strategy == tradeParams.Strategy &&
                                                    trade.SampleSize.TradeType == tradeParams.TradeType &&
                                                    tradeParams.Status == EStatus.All &&
                                                    trade.SampleSize.TimeFrame == tradeParams.TimeFrame))
                                                    .Select(trade => trade.SampleSizeId)
                                                    .Distinct()];
            }
            else
            {
                return [.. (await _unitOfWork.BaseTrade
                                    .GetAllAsync(trade =>
                                                    trade.SampleSize!.Strategy == tradeParams.Strategy &&
                                                    trade.SampleSize.TradeType == tradeParams.TradeType &&
                                                    trade.Status == tradeParams.Status))
                                                    .Select(trade => trade.SampleSizeId)
                                                    .Distinct()];
            }
        }

        private void CheckIfTradeParamTimeframeExistsInSampleSizes(List<SampleSize> listSampleSizes, LoadTradeParams tradeParams)
        {
            bool timeFrameFound = false;
            listSampleSizes.ForEach(sampleSize =>
            {
                if (sampleSize.TimeFrame == tradeParams.TimeFrame)
                {
                    timeFrameFound = true;
                }
            });
            if (!timeFrameFound && listSampleSizes.Any())
            {
                tradeParams.TimeFrame = listSampleSizes.LastOrDefault()!.TimeFrame;
            }
        }

        private void SetCurrentSampleSize(List<SampleSize> sampleSizes, LoadTradeParams tradeParams)
        {
            if (tradeParams.LoadLastSampleSize)
            {
                _tradesVM.CurrentSampleSize = sampleSizes.Where(sampleSize => sampleSize.TimeFrame == tradeParams.TimeFrame).LastOrDefault()!;
                _tradesVM.CurrentSampleSizeNumber = sampleSizes.Where(sampleSize => sampleSize.TimeFrame == tradeParams.TimeFrame).Count();
            }
            else if (tradeParams.StatusChanged)
            {
                _tradesVM.CurrentSampleSize = sampleSizes.LastOrDefault()!;
                _tradesVM.CurrentSampleSizeNumber = sampleSizes.Where(sampleSize => sampleSize.TimeFrame == tradeParams.TimeFrame).Count();
            }
            else
            {
                _tradesVM.CurrentSampleSize = sampleSizes
                                                            .Where(sampleSize => sampleSize.TimeFrame == tradeParams.TimeFrame)
                                                            .ToList()[tradeParams.SampleSizeNumber - 1];
                _tradesVM.CurrentSampleSizeNumber = tradeParams.SampleSizeNumber;
            }
        }

        private async Task<List<BaseTrade>> GetAllTrades(LoadTradeParams tradeParams)
        {
            if (tradeParams.Status == EStatus.All)
            {
                return [.. await _unitOfWork.BaseTrade.GetAllAsync(x => x.SampleSizeId == _tradesVM.CurrentSampleSize.Id)];
            }
            else
            {
                return [.. (await _unitOfWork.BaseTrade.GetAllAsync(x => x.SampleSizeId == _tradesVM.CurrentSampleSize.Id && x.Status == tradeParams.Status))];
            }
        }

        private async Task SetViewData(List<SampleSize> sampleSizes, int tradesInSampleSize, LoadTradeParams tradeParams)
        {
            _tradesVM.NumberSampleSizes = sampleSizes.Where(sampleSize => sampleSize.TimeFrame == tradeParams.TimeFrame).Count();
            _tradesVM.TradesInSampleSize = tradesInSampleSize;

            await SetJournalAndReviewData();
            await SetAvailableMenus(sampleSizes, tradeParams.Status);
        }

        private async Task SetJournalAndReviewData()
        {
            _tradesVM.CurrentTrade.Journal = await _unitOfWork.Journal.GetAsync(x => x.Id == _tradesVM.CurrentTrade!.JournalId);
            int? reviewID = (await _unitOfWork.SampleSize.GetAsync(x => x.Id == _tradesVM.CurrentTrade!.SampleSizeId)).ReviewId;
            _tradesVM.CurrentSampleSize.Review = await _unitOfWork.Review.GetAsync(x => x.Id == reviewID);
        }

        #endregion
    }
}
