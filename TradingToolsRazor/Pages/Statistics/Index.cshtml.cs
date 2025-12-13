using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.RequestModels;
using SharedEnums.Enums;
using Statistics.Models;
using Statistics.Services;

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

        public async Task<IActionResult> OnGetAsync()
        {
            var sampleSizes = await _unitOfWork.SampleSize.GetAllAsync(sampleSize => sampleSize.TradeType == ETradeType.Research);
            if (!sampleSizes.Any())
            {
                return Page();
            }

            SetTimeFrameMenu(sampleSizes);
            SetStrategiesMenu(sampleSizes);
            await SetTimesMenu(sampleSizes.Last());

            await SetStatisticsAndCurrentTrade(sampleSizes.Last());
            CurrentSampleSize = sampleSizes.Last();

            return Page();
        }

        protected JsonResult? ValidateModelState()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage)
                    .ToList();

                string allErrors = string.Join(", ", errors);
                return new JsonResult(new { error = allErrors });
            }

            return null;
        }

        public async Task<IActionResult> OnPostGetStatistics([FromBody] GetStatisticsModel paramaters)
        {
            if (CurrentSampleSize.Strategy == EStrategy.CandleBracketing)
            {
                return new JsonResult(new { data = CandleBracketingStats });
            }

            return Page();
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

            Times = [.. (await _unitOfWork.ResearchCandleBracketing.GetAllAsync())
                .Where(trade => trade.SampleSize!.Strategy == sampleSize.Strategy)
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
