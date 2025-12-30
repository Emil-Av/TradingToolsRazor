using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using Models;
using Models.RequestModels;
using SharedEnums.Enums;
using Statistics.Models;
using Statistics.Services;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TradingToolsRazor.Pages.Statistics
{
    public class IndexModel(IUnitOfWork unitOfWork) : PageModel
    {

        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public List<CandleBracketingStatisticItem> CandleBracketingStats { get; set; } = [];

        public List<ETimeFrame> AvailableTimeFrames { get; set; } = [];

        public List<EStrategy> AvailableStrategies { get; set; } = [];

        /// <summary>
        /// Relevant for CandleBracketing strategies.
        /// </summary>
        public List<TimeOnly> Times { get; set; } = [];

        public ResearchCandleBracketing? CurrentResearchCandleBracketingTrade { get; set; }

        public SampleSize? CurrentSampleSize { get; set; }

        public int NumberSampleSizes { get; set; }

        public int CurrentSampleSizeNumber { get; set; }

        public async Task<IActionResult> OnGetAsync([FromQuery] StatisticsQueryModel query)
        {
            var sampleSizes = await _unitOfWork.SampleSize.GetAllAsync(sampleSize => sampleSize.TradeType == ETradeType.Research);
            if (!sampleSizes.Any())
            {
                return Page();
            }

            SetTimeFrameMenu(sampleSizes);
            SetStrategiesMenu(sampleSizes);

            var sampleSizeData = await GetSampleSize(sampleSizes, query);
            CurrentSampleSize = sampleSizeData.sampleSizes.FirstOrDefault();
            await SetSampleSizeNumber(sampleSizeData, sampleSizes, query);

            await SetTimesMenu(sampleSizeData.sampleSizes.FirstOrDefault()!);
            await SetStatisticsAndCurrentTrade(sampleSizeData.sampleSizes.FirstOrDefault()!);

            return Page();
        }

        private async Task SetTimesMenu(SampleSize sampleSize)
        {
            Times = [.. (await _unitOfWork.ResearchCandleBracketing.GetAllAsync(trade => trade.SampleSize!.TimeFrame == sampleSize.TimeFrame)).Select(trade => trade.Time).Distinct()];
        }

        private async Task SetSampleSizeNumber((List<SampleSize> sampleSizes, int numberSampleSizes) sampleSizeData, List<SampleSize> sampleSizes, StatisticsQueryModel query)
        {
            if (query.IsInitialLoading)
            {
                await SetSampleSizeNumberWhenInitialLoading(sampleSizeData, sampleSizes, query);
            }
            // Menu buttons clicked
            else
            {
                if (query.Strategy == EStrategy.CandleBracketing)
                {
                    // load all sample sizes for candle bracketing, trade type, time frame and time
                    await SetSampleSizeNumberForCandleBracketing(sampleSizes, sampleSizeData, query);
                }
            }
        }
        private async Task SetSampleSizeNumberWhenInitialLoading((List<SampleSize> sampleSizes, int numberSampleSizes) sampleSizeData, List<SampleSize> sampleSizes, StatisticsQueryModel query)
        {
            if (sampleSizeData.sampleSizes.FirstOrDefault()?.Strategy == EStrategy.CandleBracketing)
            {
                await SetSampleSizeNumberForCandleBracketing(sampleSizes, sampleSizeData, query);
            }
        }

        private async Task SetSampleSizeNumberForCandleBracketing(List<SampleSize> sampleSizes, (List<SampleSize> sampleSizes, int numberSampleSizes) sampleSizeData, StatisticsQueryModel query)
        {
            List<int> sampleSizeIds = [];
            var trades = await _unitOfWork.ResearchCandleBracketing
                    .GetAllAsync(x => x.SampleSize!.TimeFrame == sampleSizeData.sampleSizes.FirstOrDefault()!.TimeFrame && x.SampleSize.TradeType == sampleSizeData.sampleSizes.FirstOrDefault()!.TradeType,
                                  includeProperties: "SampleSize");

            if (query.IsInitialLoading)
            {
                var currentTime = trades.First(trade => trade.SampleSizeId == sampleSizeData.sampleSizes.FirstOrDefault()!.Id).Time;
                sampleSizeIds = [.. trades
                    .Where(trade => trade.Time == currentTime)
                    .Select(trade => trade.SampleSizeId)
                    .Distinct()];
            }
            else
            {
                sampleSizeIds = [.. trades
                    .Where(trade => trade.Time == TimeOnly.Parse(query.Time!))
                    .Select(trade => trade.SampleSizeId)
                    .Distinct()];
            }

            NumberSampleSizes = sampleSizeIds.Count;
            CurrentSampleSizeNumber = NumberSampleSizes;
        }

        private async Task<(List<SampleSize> sampleSizes, int numberSampleSizes)> GetSampleSize(List<SampleSize> sampleSizes, StatisticsQueryModel query)
        {
            if (query.IsInitialLoading)
            {
                var sampleSize = sampleSizes.Last();
                var numberSampleSizes = sampleSizes.Where(x => x.TimeFrame == sampleSize.TimeFrame && x.Strategy == sampleSize.Strategy && x.TradeType == sampleSize.TradeType).Count();
                return (new List<SampleSize> { sampleSize }, numberSampleSizes);
            }

            var sampleSizeIdsForTime = (await _unitOfWork.ResearchCandleBracketing.GetAllAsync(trade => trade.Time == TimeOnly.Parse(query.Time!))).Select(trade => trade.SampleSizeId).Distinct().ToList();

            return (sampleSizes.Where(sampleSize => sampleSizeIdsForTime.Contains(sampleSize.Id)).ToList(), sampleSizeIdsForTime.Count);
        }

        private async Task SetStatisticsAndCurrentTrade(SampleSize sampleSize)
        {
            if (sampleSize.Strategy == EStrategy.CandleBracketing)
            {
                var trades = await _unitOfWork.ResearchCandleBracketing.GetAllAsync(trade => trade.SampleSizeId == sampleSize.Id);
                CurrentResearchCandleBracketingTrade = trades.Last();
                CandleBracketingStats = CandleBracketingStatistics.GetAllStats(trades);
            }
        }

        private void SetStrategiesMenu(List<SampleSize> sampleSizes)
        {
            AvailableStrategies = [.. sampleSizes
                .Where(sampleSize => sampleSize.Strategy == sampleSizes.Last().Strategy)
                .Select(sampleSize => sampleSize.Strategy)
                .Distinct()
                .OrderBy(strategy => strategy)];
        }

        private void SetTimeFrameMenu(List<SampleSize> sampleSizes)
        {
            AvailableTimeFrames = [.. sampleSizes
                .Where(sampleSize => sampleSize.Strategy == sampleSizes.Last().Strategy)
                .Select(sampleSize => sampleSize.TimeFrame)
                .Distinct()
                .OrderBy(timeFrame => timeFrame)];
        }
    }
}
