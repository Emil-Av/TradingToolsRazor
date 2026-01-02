using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.ViewModels;
using Models.ViewModels.DisplayClasses;
using Newtonsoft.Json;
using Shared;
using SharedEnums.Enums;
using System.Diagnostics;
using Utilities;
using TradingToolsRazor.Services.Interfaces;

namespace TradingToolsRazor.Pages.NewTrade
{
    public class IndexModel(INewTradeService newTradeService) : PageModel
    {
        private readonly INewTradeService _newTradeService = newTradeService;

        public NewTradeVM NewTradeVM { get; set; } = new();

        public PartialViewsVM NewTradeParentVM { get; set; } = new();

        #region Handlers

        public IActionResult OnGet()
        {
            NewTradeParentVM.CandleBracketing.Date = DateOnly.FromDateTime(DateTime.Now);
            NewTradeParentVM.CandleBracketing.Time = TimeOnly.FromDateTime(DateTime.Now);
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

        public async Task<JsonResult> OnPostSaveNewTradeAsync([FromForm] IFormFile[] files, [FromForm] string viewData, [FromForm] string sampleSizeViewData)
        {
            var validationResult = ValidateModelState();
            if (validationResult != null)
                return new JsonResult(new { error = validationResult.Value });

            string errorMsg = NewTradeVM.ParseViewData(viewData, sampleSizeViewData);
            if (!string.IsNullOrEmpty(errorMsg))
                return new JsonResult(new { error = errorMsg });

            try
            {
                await _newTradeService.SaveTradeAsync(NewTradeVM, files);
                return new JsonResult(new { success = "Trade saved." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = $"Error while saving the trade: {ex.Message}" });
            }
        }

        public async Task<JsonResult> OnPostSaveNewTradeOldAsync([FromForm] IFormFile[] files, [FromForm] string tradeParams, [FromForm] string researchData, [FromForm] string tradeData)
        {
            var validationResult = ValidateModelState();
            if (validationResult != null) 
                return validationResult;

            string errorMsg = NewTradeVM.SetValues(tradeParams, researchData, tradeData);
            if (!string.IsNullOrEmpty(errorMsg)) 
                return new JsonResult(new { error = errorMsg });

            try
            {
                await _newTradeService.SaveTradeAsync(NewTradeVM, files);
                return new JsonResult(new { success = "Trade saved." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = $"Error while saving the trade: {ex.Message}" });
            }
        }

        #endregion
    }
}
