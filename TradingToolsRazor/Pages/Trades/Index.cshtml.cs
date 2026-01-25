using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.RequestModels;
using Models.ViewModels;
using Newtonsoft.Json;
using SharedEnums.Enums;
using TradingToolsRazor.Pages.Shared;
using TradingToolsRazor.Services.Interfaces;

namespace TradingToolsRazor.Pages.Trades
{
    public class IndexModel(ITradesService tradesService) : BaseIndexModel
    {

        #region Private Properties

        private readonly ITradesService _tradesService = tradesService;

        #endregion

        #region Public Properties

        public TradesVM TradesVM { get; set; } = new TradesVM();

        #endregion

        #region Handlers

        public async Task<IActionResult> OnGetAsync()
        {
            TradesVM = await _tradesService.InitializeTradesViewModelAsync();
            return Page();
        }

        public async Task<IActionResult> OnGetLoadType(SampleSizeType sampleSizeType, Strategy strategy)
        {
            TradesVM = await _tradesService.LoadTypeAsync(sampleSizeType, strategy);
            return Page();
        }

        public async Task<IActionResult> OnGetLoadStrategy([FromQuery] Strategy strategy, SampleSizeType sampleSizeType)
        {
            TradesVM = await _tradesService.LoadStrategyAsync(strategy, sampleSizeType);
            return Page();
        }

        public async Task<IActionResult> OnGetLoadTimeFrameAsync([FromQuery] Strategy strategy, [FromQuery] SampleSizeType sampleSizeType, [FromQuery] TimeFrame timeFrame)
        {
            var requestModel = new TradesLoadTimeFrameRequestModel
            {
                Strategy = strategy,
                SampleSizeType = sampleSizeType,
                TimeFrame = timeFrame,
            };

            TradesVM = await _tradesService.LoadTimeFrameAsync(requestModel);
            return Page();
        }

        public async Task<IActionResult> OnGetLoadSampleSizeNumberAsync(int sampleSizeId)
        {
            TradesVM = await _tradesService.LoadSampleSizeNumberAsync(sampleSizeId);
            return Page();
        }

        public async Task<IActionResult> OnPostDeleteTrade([FromBody] DeleteTradeRequestModel deleteTradeRequest)
        {
            var jsonResult = ValidateModel();
            if (jsonResult is not null)
            {
                return jsonResult;
            }

            await _tradesService.DeleteTrade(deleteTradeRequest);

            return new JsonResult(new { success = "Trade deleted." });
        }

        public async Task<IActionResult> OnPostUpdateResearchData([FromBody] UpdateResearchDataModel updateResearchData)
        {
            var jsonResult = ValidateModel();
            if (jsonResult is not null)
            {
                return jsonResult; 
            }

            await _tradesService.UpdateResearchData(updateResearchData);

            return new JsonResult(new { success = "Research data updated." });
        }

        public async Task<IActionResult> OnPostUpdateTradeDataAsync([FromBody] BaseTrade tradeData)
        {
            try
            {
                await _tradesService.UpdateTradeDataAsync(tradeData);
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = $"An error occured while updating the trade: {ex.Message}" });
            }

            return new JsonResult(new { success = "Trade updated" });
        }

        public async Task<IActionResult> OnPostUpdateReviewAsync([FromBody] Review review)
        {
            var jsonResult = ValidateModel();
            if (jsonResult is not null)
            {
                return jsonResult;
            }

            try
            {
                await _tradesService.UpdateReviewAsync(review);
                return new JsonResult(new { success = "Review updated." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = $"An error occurred while updating the review: {ex.Message}" });
            }
        }

        public async Task<IActionResult> OnPostUpdateJournalAsync([FromBody] Journal journal)
        {
            var jsonResult = ValidateModel();
            if (jsonResult is not null)
            {
                return jsonResult;
            }

            try
            {
                await _tradesService.UpdateJournalAsync(journal);
                return new JsonResult(new { success = "Journal updated." });
            }
            catch (InvalidOperationException ex)
            {
                return new JsonResult(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = $"An error occurred while updating the journal: {ex.Message}" });
            }
        }

        #endregion
    }
}
