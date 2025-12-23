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

        public List<string> Times { get; set; } = [];

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

            var sampleSizeData = GetSampleSize(sampleSizes, query);
            CurrentSampleSize = sampleSizeData.sampleSize;
            await SetSampleSizeNumber(sampleSizeData, sampleSizes, query);

            await SetTimesMenu(sampleSizeData.sampleSize);
            await SetStatisticsAndCurrentTrade(sampleSizeData.sampleSize);

            return Page();
        }

        private async Task SetSampleSizeNumber((SampleSize sampleSize, int numberSampleSizes) sampleSizeData, List<SampleSize> sampleSizes, StatisticsQueryModel query)
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
                    await SetSampleSizeNumberForCandleBracketing(null, sampleSizes, sampleSizeData, query);
                }
            }
        }

        private async Task SetSampleSizeNumberWhenInitialLoading((SampleSize sampleSize, int numberSampleSizes) sampleSizeData, List<SampleSize> sampleSizes, StatisticsQueryModel query)
        {
            if (sampleSizeData.sampleSize.Strategy == EStrategy.CandleBracketing)
            {
                var time = (await _unitOfWork.ResearchCandleBracketing.GetAsync(trade => trade.SampleSizeId == sampleSizeData.sampleSize.Id)).Time;
                await SetSampleSizeNumberForCandleBracketing(time, sampleSizes, sampleSizeData, query);
            }
            else
            {
                // To be done when other strategies are added
                NumberSampleSizes = sampleSizeData.numberSampleSizes;
                CurrentSampleSizeNumber = NumberSampleSizes;
            }
        }

        private async Task SetSampleSizeNumberForCandleBracketing(TimeOnly? time, List<SampleSize> sampleSizes, (SampleSize sampleSize, int numberSampleSizes) sampleSizeData, StatisticsQueryModel query)
        {
            time ??= TimeOnly.Parse(query.Time!);
            List<int> sampleSizeIds = [];

            if (query.IsInitialLoading)
            {
                sampleSizeIds = [.. (await _unitOfWork.ResearchCandleBracketing
                    .GetAllAsync(x => x.Time == time && x.SampleSize!.TimeFrame == sampleSizeData.sampleSize.TimeFrame, includeProperties: "SampleSize"))
                    .Select(x => x.SampleSizeId)
                    .Distinct()];
            }
            else
            {
                sampleSizeIds = [.. (await _unitOfWork.ResearchCandleBracketing
                    .GetAllAsync(x => x.TimeFrame == query.TimeFrame && x.SampleSize!.TradeType == query.TradeType))
                    .Select(x => x.SampleSizeId)
                    .Distinct()];
            }

            NumberSampleSizes = sampleSizes.Where(x => sampleSizeIds.Contains(x.Id)).Count();
            CurrentSampleSizeNumber = NumberSampleSizes;
        }

        private (SampleSize sampleSize, int numberSampleSizes) GetSampleSize(List<SampleSize> sampleSizes, StatisticsQueryModel query)
        {
            if (query.IsInitialLoading)
            {
                var sampleSize = sampleSizes.Last();
                var numberSampleSizes = sampleSizes.Where(x => x.TimeFrame == sampleSize.TimeFrame && x.Strategy == sampleSize.Strategy && x.TradeType == sampleSize.TradeType).Count();
                return (sampleSize, numberSampleSizes);
            }

            return (sampleSizes.First(sampleSize => sampleSize.TimeFrame == query.TimeFrame && sampleSize.Strategy == query.Strategy && sampleSize.TradeType == query.TradeType), 1);
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

        private async Task SetTimesMenu(SampleSize sampleSize)
        {
            if (sampleSize.Strategy != EStrategy.CandleBracketing)
            {
                return;
            }

            Times = [.. (await _unitOfWork.ResearchCandleBracketing.GetAllAsync(trade => trade.SampleSize!.Id == sampleSize.Id, includeProperties: "SampleSize"))
                .Select(trade => trade.Time.ToString("HH:mm"))
                .Distinct()
                .OrderBy(time => time)];
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
