using DataAccess.Repository.IRepository;
using Models;
using Models.ViewModels;
using Models.ViewModels.DisplayClasses;
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

            List<Trade> listTrades = await GetAllTrades(tradeParams);
            await SetCurrentTrade();

            await SetViewData(sampleSizes, listTrades.Count, tradeParams);

            return _tradesVM;
        }

        public async Task UpdateTradeDataAsync(Trade tradeData)
        {
            Trade trade = await _unitOfWork.Trade.GetAsync(x => x.Id == tradeData.Id, includeProperties: "SampleSize");
            if (trade == null)
            {
                throw new InvalidOperationException($"Trade with ID {tradeData.Id} not found.");
            }

            EntityMapper.ViewModelToEntity(trade, tradeData);
            await _unitOfWork.Trade.UpdateAsync(trade);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateReviewAsync(TradesVM data)
        {
            ValidateReviewData(data);

            Review review = await _unitOfWork.Review.GetAsync(x => x.Id == data.CurrentSampleSize.Review.Id);
            if (review == null)
            {
                throw new InvalidOperationException($"The review for sample size with ID {data.CurrentTrade.SampleSizeId} wasn't found in the database.");
            }

            SetReviewValues(review, data);
            await _unitOfWork.Review.UpdateAsync(review);
            await _unitOfWork.SaveAsync();
        }

        public async Task UpdateJournalAsync(TradesVM data)
        {
            if (data.CurrentTrade.Journal == null)
            {
                throw new InvalidOperationException("Journal wasn't updated. Journal was null.");
            }

            Journal journal = await _unitOfWork.Journal.GetAsync(x => x.Id == data.CurrentTrade.JournalId);
            if (journal == null)
            {
                throw new InvalidOperationException($"Journal with ID {data.CurrentTrade.JournalId} not found.");
            }

            SetJournalValues(journal, data);
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
            List<SampleSize> list = new List<SampleSize>();
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

        #region Helper Methods - Update

        private void ValidateReviewData(TradesVM data)
        {
            if (data.CurrentSampleSize == null)
            {
                throw new ArgumentException("CurrentSampleSize is null.");
            }
            else if (data.CurrentSampleSize.Id == 0)
            {
                throw new ArgumentException("SampleSize Id is 0");
            }
            else if (data.CurrentSampleSize.Review == null)
            {
                throw new ArgumentException("Review is null.");
            }
            else if (data.CurrentSampleSize.Review.Id == 0)
            {
                throw new ArgumentException("Review Id is 0");
            }
        }

        private void SetReviewValues(Review review, TradesVM data)
        {
            review.First = data.CurrentSampleSize.Review!.First;
            review.Second = data.CurrentSampleSize.Review.Second;
            review.Third = data.CurrentSampleSize.Review.Third;
            review.Forth = data.CurrentSampleSize.Review.Forth;
            review.Summary = data.CurrentSampleSize.Review.Summary;
        }

        private void SetJournalValues(Journal journal, TradesVM data)
        {
            journal.Pre = data.CurrentTrade.Journal!.Pre;
            journal.During = data.CurrentTrade.Journal.During;
            journal.Exit = data.CurrentTrade.Journal.Exit;
            journal.Post = data.CurrentTrade.Journal.Post;
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
                return [.. (await _unitOfWork.Trade
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
                return [.. (await _unitOfWork.Trade
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
            bool tfFound = false;
            listSampleSizes.ForEach(sampleSize =>
            {
                if (sampleSize.TimeFrame == tradeParams.TimeFrame)
                {
                    tfFound = true;
                }
            });
            if (!tfFound && listSampleSizes.Any())
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

        private async Task<List<Trade>> GetAllTrades(LoadTradeParams tradeParams)
        {
            if (tradeParams.Status == EStatus.All)
            {
                return [.. await _unitOfWork.Trade.GetAllAsync(x => x.SampleSizeId == _tradesVM.CurrentSampleSize.Id)];
            }
            else
            {
                return [.. (await _unitOfWork.Trade.GetAllAsync(x => x.SampleSizeId == _tradesVM.CurrentSampleSize.Id && x.Status == tradeParams.Status))];
            }
        }

        private void SetCurrentTrade(TradesVM tradesVM, List<Trade> listTrades, LoadTradeParams tradeParams)
        {
            if (tradeParams.ShowLastTrade)
            {
                tradesVM.CurrentTrade = listTrades.LastOrDefault()!;
            }
            else if (listTrades.Count == 1)
            {
                tradesVM.CurrentTrade = listTrades.FirstOrDefault()!;
            }
            else if (tradeParams.Status != EStatus.All)
            {
                List<Trade> filteredTrades = listTrades.Where(trade => trade.Status == tradeParams.Status).ToList();
                if (tradeParams.TradeNumber > filteredTrades.Count)
                {
                    tradesVM.CurrentTrade = filteredTrades.LastOrDefault()!;
                }
                else
                {
                    tradesVM.CurrentTrade = filteredTrades[tradeParams.TradeNumber - 1];
                }
            }
            else
            {
                tradesVM.CurrentTrade = listTrades[tradeParams.TradeNumber - 1];
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
