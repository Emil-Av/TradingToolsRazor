using DataAccess.Repository.IRepository;
using Models;
using Models.RequestModels;
using SharedEnums.Enums;
using Statistics.Interfaces;
using Statistics.Models;

namespace Statistics.Services
{
    public class StatisticsService(IUnitOfWork unitOfWork) : IStatisticsService
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task<StatisticsPageViewModel> BuildViewModelAsync(StatisticsQueryModel query)
        {
            var viewModel = new StatisticsPageViewModel();

            var allSampleSizes = await GetAllResearchSampleSizesAsync();
            if (!allSampleSizes.Any())
                return viewModel;

            await PopulateViewModelAsync(query, viewModel, allSampleSizes);

            return viewModel;
        }

        private async Task PopulateStatisticsAsync(StatisticsPageViewModel viewModel)
        {
            if (viewModel.CurrentSampleSize!.Strategy == EStrategy.CandleBracketing)
            {
                await PopulateCandleBracketingDetailsAsync(viewModel);
            }
        }

        private async Task PopulateViewModelAsync(StatisticsQueryModel query, StatisticsPageViewModel viewModel, List<SampleSize> allSampleSizes)
        {
            viewModel.AvailableStrategies = BuildAvailableStrategies(allSampleSizes);
            viewModel.AvailableTimeFrames = BuildAvailableTimeFrames(allSampleSizes);

            var (currentSampleSize, numberOfSampleSizes) = await ResolveCurrentSampleSizeAsync(allSampleSizes, query);
            viewModel.CurrentSampleSize = currentSampleSize;
            viewModel.NumberSampleSizes = numberOfSampleSizes;
            viewModel.CurrentSampleSizeNumber = numberOfSampleSizes;

            await PopulateStatisticsAsync(viewModel);
        }

        private Task<List<SampleSize>> GetAllResearchSampleSizesAsync()
        {
            return _unitOfWork.SampleSize.GetAllAsync(sampleSize => sampleSize.TradeType == ETradeType.Research);
        }

        private static List<EStrategy> BuildAvailableStrategies(List<SampleSize> allSampleSizes)
        {
            return [.. allSampleSizes
                .Where(sampleSize => sampleSize.Strategy == allSampleSizes.Last().Strategy)
                .Select(sampleSize => sampleSize.Strategy)
                .Distinct()
                .OrderBy(strategy => strategy)];
        }

        private static List<ETimeFrame> BuildAvailableTimeFrames(List<SampleSize> allSampleSizes)
        {
            return [.. allSampleSizes
                .Where(sampleSize => sampleSize.Strategy == allSampleSizes.Last().Strategy)
                .Select(sampleSize => sampleSize.TimeFrame)
                .Distinct()
                .OrderBy(timeFrame => timeFrame)];
        }

        private async Task<(SampleSize? currentSampleSize, int numberOfSampleSizes)> ResolveCurrentSampleSizeAsync(List<SampleSize> allSampleSizes, StatisticsQueryModel query)
        {
            if (query.IsInitialLoading)
            {
                return await ResolveSampleSizeForInitialLoadingAsync(allSampleSizes);
            }

            if (query.Strategy == EStrategy.CandleBracketing)
            {
                return await ResolveSampleSizeForCandleBracketingAsync(allSampleSizes, query);
            }

            var selectedSampleSize = allSampleSizes.First(sampleSize => sampleSize.TimeFrame == query.TimeFrame && sampleSize.Strategy == query.Strategy && sampleSize.TradeType == query.TradeType);
            return (selectedSampleSize, 1);
        }

        private async Task<(SampleSize? currentSampleSize, int numberOfSampleSizes)> ResolveSampleSizeForCandleBracketingAsync(List<SampleSize> allSampleSizes, StatisticsQueryModel query)
        {
            List<int> sampleSizeIds = [];
            if (query.IsNewLoad)
            {
                sampleSizeIds = await ResolveSampleSizeForNewLoadAsync(allSampleSizes, query, sampleSizeIds);
            }
            else
            {
                sampleSizeIds = await ResolveSampleSizeForGivenTimeAsync(query, sampleSizeIds);
            }

            var currentSampleSize = allSampleSizes.FirstOrDefault(sampleSize => sampleSizeIds.Contains(sampleSize.Id));
            return (currentSampleSize, sampleSizeIds.Count);
        }

        private async Task<List<int>> ResolveSampleSizeForGivenTimeAsync(StatisticsQueryModel query, List<int> sampleSizeIds)
        {
            var time = TimeOnly.Parse(query.Time!);

            var trades = await _unitOfWork.ResearchCandleBracketing
                .GetAllAsync(trade => trade.SampleSize!.TimeFrame == query.TimeFrame &&
                             trade.SampleSize.TradeType == query.TradeType &&
                             trade.SampleSize.Strategy == query.Strategy,
                             includeProperties: "SampleSize");

            sampleSizeIds = [.. trades
                .Where(trade => trade.Time == time)
                .Select(trade => trade.SampleSizeId)
                .Distinct()];

            return sampleSizeIds;
        }

        private async Task<List<int>> ResolveSampleSizeForNewLoadAsync(List<SampleSize> allSampleSizes, StatisticsQueryModel query, List<int> sampleSizeIds)
        {
            var trades = await _unitOfWork.ResearchCandleBracketing.GetAllAsync(trade => trade.SampleSize!.TimeFrame == query.TimeFrame &&
                                                                                                 trade.SampleSize.TradeType == query.TradeType &&
                                                                                                 trade.SampleSize.Strategy == query.Strategy,
                                                                                                 includeProperties: "SampleSize");

            var lastSampleSize = allSampleSizes
                .Where(sampleSize => sampleSize.TimeFrame == query.TimeFrame &&
                             sampleSize.TradeType == query.TradeType &&
                             sampleSize.Strategy == query.Strategy)
                .OrderBy(sampleSize => sampleSize.Id)
                .LastOrDefault();

            if (lastSampleSize is not null)
            {
                var timeFromLastSampleSize = trades
                    .Where(time => time.SampleSizeId == lastSampleSize.Id)
                    .Select(time => time.Time)
                    .FirstOrDefault();

                sampleSizeIds = [.. trades
                    .Where(trade => trade.Time == timeFromLastSampleSize)
                    .Select(trade => trade.SampleSizeId)
                    .Distinct()];
            }

            return sampleSizeIds;
        }

        private async Task<(SampleSize? currentSampleSize, int numberOfSampleSizes)> ResolveSampleSizeForInitialLoadingAsync(List<SampleSize> allSampleSizes)
        {
            var currentSampleSize = allSampleSizes.Last();
            int numberOfSampleSizes = 0;
            if (currentSampleSize.Strategy == EStrategy.CandleBracketing)
            {
                var time = (await _unitOfWork.ResearchCandleBracketing.GetAsync(trade => trade.SampleSizeId == currentSampleSize.Id)).Time;
                numberOfSampleSizes = (await _unitOfWork.ResearchCandleBracketing
                    .GetAllAsync(trade => trade.Time == time))
                    .Select(trade => trade.SampleSizeId)
                    .Distinct()
                    .Count();
            }
            else
            {
                numberOfSampleSizes = allSampleSizes.Count(sampleSize => sampleSize.TimeFrame == currentSampleSize.TimeFrame && sampleSize.Strategy == currentSampleSize.Strategy && sampleSize.TradeType == currentSampleSize.TradeType);
            }
            return (currentSampleSize, numberOfSampleSizes);
        }

        private async Task PopulateCandleBracketingDetailsAsync(StatisticsPageViewModel viewModel)
        {
            if (viewModel.CurrentSampleSize is null)
                return;

            var tradesForTimeFrame = await _unitOfWork.ResearchCandleBracketing.GetAllAsync(trade => trade.SampleSize!.TimeFrame == viewModel.CurrentSampleSize.TimeFrame, includeProperties: "SampleSize");

            viewModel.Times = [.. tradesForTimeFrame
                .Select(trade => trade.Time)
                .Distinct()
                .OrderBy(time => time)];

            var tradesForCurrentSampleSize = await _unitOfWork.ResearchCandleBracketing.GetAllAsync(trade => trade.SampleSizeId == viewModel.CurrentSampleSize.Id);

            viewModel.CurrentResearchCandleBracketingTrade = tradesForCurrentSampleSize.LastOrDefault();
            viewModel.CandleBracketingStats = CandleBracketingStatistics.GetAllStats(tradesForCurrentSampleSize);
        }
    }
}
