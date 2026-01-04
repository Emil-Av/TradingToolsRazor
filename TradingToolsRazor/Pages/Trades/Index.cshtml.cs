using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.ViewModels;
using TradingToolsRazor.Services.Interfaces;

namespace TradingToolsRazor.Pages.Trades
{
    public class IndexModel(ITradesService tradesService) : PageModel
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

        public async Task<IActionResult> OnPostUpdateReviewAsync([FromBody] TradesVM data)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { error = "Invalid model state" });
            }

            try
            {
                await _tradesService.UpdateReviewAsync(data);
                return new JsonResult(new { success = "Review updated." });
            }
            catch (ArgumentException ex)
            {
                return new JsonResult(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return new JsonResult(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = $"An error occurred while updating the review: {ex.Message}" });
            }
        }

        public async Task<IActionResult> OnPostUpdateJournalAsync([FromBody] TradesVM data)
        {
            try
            {
                await _tradesService.UpdateJournalAsync(data);
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

        public async Task<IActionResult> OnGetLoadTradeAsync([FromQuery] LoadTradeParams tradeParams)
        {
            if (!ModelState.IsValid)
            {
                return new JsonResult(new { error = "Invalid model state" });
            }

            try
            {
                TradesVM = await _tradesService.LoadTradeAsync(tradeParams);
                
                if (!string.IsNullOrEmpty(TradesVM.ErrorMsg))
                {
                    return new JsonResult(new { info = TradesVM.ErrorMsg });
                }

                return new JsonResult(new { tradesVM = TradesVM });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = $"An error occurred while loading trade: {ex.Message}" });
            }
        }

        #endregion
    }
}
