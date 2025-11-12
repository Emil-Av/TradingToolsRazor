using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.ViewModels;
using Models.ViewModels.DisplayClasses;
using Newtonsoft.Json;
using Shared;
using SharedEnums.Enums;
using Utilities;

namespace TradingToolsRazor.Pages.NewTrade
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public IndexModel(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            NewTradeVM = new();
            NewTradeParentVM = new();
        }

        public NewTradeVM NewTradeVM { get; set; }

        public PartialViewsVM NewTradeParentVM { get; set; }

        #region Handlers

        public IActionResult OnGet()
        {
            // Display the main page
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

        public async Task<JsonResult> OnPostSaveNewTradeAsync([FromForm] IFormFile[] files, [FromForm] string tradeParams, [FromForm] string researchData, [FromForm] string tradeData)
        {
            var validationResult = ValidateModelState();
            if (validationResult != null) return validationResult;

            string errorMsg = NewTradeVM.SetValues(tradeParams, researchData, tradeData);
            if (!string.IsNullOrEmpty(errorMsg)) return new JsonResult(new { error = errorMsg });

            try
            {
                await SaveTradeAsync(files);
                return new JsonResult(new { success = "Trade saved." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = $"Error while saving the trade: {ex.Message}" });
            }
        }

        #endregion

        #region Private Methods

        private async Task SaveTradeAsync(IFormFile[] files)
        {
            // Research trades
            if (NewTradeVM.TradeType == ETradeType.Research)
            {
                if (NewTradeVM.Strategy == EStrategy.FirstBarPullback)
                {
                    await SaveResearchDataFirstbarPullback(maxTradesProSampleSize: 100);
                }
                else if (NewTradeVM.Strategy == EStrategy.Cradle)
                {
                    await SaveResearchCradleData(maxTradesProSampleSize: 100);
                }
                else if (NewTradeVM.Strategy == EStrategy.CandleBracketing)
                {
                    await SaveCandleBracketingData(maxTradesProSampleSize: 100);
                }
            }
            else // PaperTrade or normal Trade
            {
                if (NewTradeVM.Strategy == EStrategy.FirstBarPullback)
                {
                    var researchData = await SaveResearchDataFirstbarPullback(maxTradesProSampleSize: 20);
                    var newTrade = await SetNewTradeData(researchData, files);
                    await CreateJournal(newTrade);

                    _unitOfWork.Trade.Add(newTrade);
                    await _unitOfWork.SaveAsync();
                }
                else if (NewTradeVM.Strategy == EStrategy.Cradle)
                {
                    // TODO: Implement cradle trade saving logic
                }
            }

            #region Local Helper Methods

            async Task CreateJournal(Trade newTrade)
            {
                var journal = new Journal();
                _unitOfWork.Journal.Add(journal);
                await _unitOfWork.SaveAsync();
                newTrade.JournalId = journal.Id;
            }

            async Task<Trade> SetNewTradeData(ResearchFirstBarPullback researchData, IFormFile[] files)
            {
                var newTrade = EntityMapper.ViewModelDisplayToEntity<Trade, TradeDisplay>(NewTradeVM.TradeData, existingEntity: null);
                await ScreenshotsHelper.SaveFilesAsync(_webHostEnvironment.WebRootPath, NewTradeVM, newTrade, files);

                newTrade.ResearchId = researchData.Id;
                newTrade.SampleSizeId = researchData.SampleSizeId;
                newTrade.Status = NewTradeVM.Status;
                newTrade.SideType = NewTradeVM.SideType;
                newTrade.OrderType = NewTradeVM.OrderType;
                newTrade.SampleSize = researchData.SampleSize;

                return newTrade;
            }

            async Task<ResearchCandleBracketing> SaveCandleBracketingData(int maxTradesProSampleSize)
            {
                var viewData = NewTradeVM.ResearchData as ResearchCandleBracketing;
                viewData.SampleSizeId = (await ProcessSampleSize(maxTradesProSampleSize)).id;

                var researchData = new ResearchCandleBracketing();
                EntityMapper.ViewModelToEntity(researchData, viewData);
                researchData.SampleSizeId = (await ProcessSampleSize(maxTradesProSampleSize)).id;
                researchData.ScreenshotsUrls = await ScreenshotsHelper.SaveFilesAsync(_webHostEnvironment.WebRootPath, NewTradeVM, viewData, files);

                _unitOfWork.ResearchCandleBracketing.Add(researchData);
                try
                {
                    await _unitOfWork.SaveAsync();
                }
                catch (Exception ex)
                {

                }

                return viewData;
            }

            async Task<ResearchCradle> SaveResearchCradleData(int maxTradesProSampleSize)
            {
                var viewData = NewTradeVM.ResearchData as ResearchCradle;
                viewData.SampleSizeId = (await ProcessSampleSize(maxTradesProSampleSize)).id;

                var researchData = new ResearchCradle();
                EntityMapper.ViewModelToEntity(researchData, viewData);
                researchData.SampleSizeId = (await ProcessSampleSize(maxTradesProSampleSize)).id;
                researchData.ScreenshotsUrls = await ScreenshotsHelper.SaveFilesAsync(_webHostEnvironment.WebRootPath, NewTradeVM, viewData, files);

                _unitOfWork.ResearchCradle.Add(researchData);
                await _unitOfWork.SaveAsync();

                return viewData;
            }

            async Task<ResearchFirstBarPullback> SaveResearchDataFirstbarPullback(int maxTradesProSampleSize)
            {
                var viewData = NewTradeVM.ResearchData as ResearchFirstBarPullbackDisplay;
                var researchData = EntityMapper.ViewModelDisplayToEntity<ResearchFirstBarPullback, ResearchFirstBarPullbackDisplay>(viewData, existingEntity: null);

                researchData.SampleSizeId = (await ProcessSampleSize(maxTradesProSampleSize)).id;
                if (maxTradesProSampleSize == 100)
                {
                    researchData.ScreenshotsUrls = await ScreenshotsHelper.SaveFilesAsync(_webHostEnvironment.WebRootPath, NewTradeVM, researchData, files);
                }

                _unitOfWork.ResearchFirstBarPullback.Add(researchData);
                await _unitOfWork.SaveAsync();

                return researchData;
            }

            #endregion
        }

        private async Task<(int id, bool wasFull)> ProcessSampleSize(int maxTradesProSampleSize)
        {
            var sampleSizeData = await CheckLastSampleSize(maxTradesProSampleSize);

            if (sampleSizeData.isFull || sampleSizeData.id == 0)
            {
                Review review = null;

                if (NewTradeVM.TradeType != ETradeType.Research)
                {
                    review = new Review();
                    _unitOfWork.Review.Add(review);
                    await _unitOfWork.SaveAsync();
                }

                var newSampleSize = new SampleSize
                {
                    Strategy = NewTradeVM.Strategy,
                    TimeFrame = NewTradeVM.TimeFrame,
                    TradeType = NewTradeVM.TradeType,
                    ReviewId = review?.Id
                };

                _unitOfWork.SampleSize.Add(newSampleSize);
                await _unitOfWork.SaveAsync();
                sampleSizeData.id = newSampleSize.Id;
            }

            return sampleSizeData;
        }

        private async Task<(int id, bool isFull)> CheckLastSampleSize(int maxTradesProSampleSize)
        {
            int id = 0;
            bool isFull = false;

            var listSampleSizes = await _unitOfWork.SampleSize.GetAllAsync(x =>
                x.TimeFrame == NewTradeVM.TimeFrame &&
                x.Strategy == NewTradeVM.Strategy &&
                x.TradeType == NewTradeVM.TradeType);

            if (!listSampleSizes.Any()) return (0, false);

            id = listSampleSizes.Last().Id;

            int numberTradesInSampleSize = NewTradeVM.TradeType switch
            {
                ETradeType.Research when NewTradeVM.Strategy == EStrategy.FirstBarPullback =>
                    (await _unitOfWork.ResearchFirstBarPullback.GetAllAsync(x => x.SampleSizeId == id)).Count,

                ETradeType.Research when NewTradeVM.Strategy == EStrategy.Cradle =>
                    (await _unitOfWork.ResearchCradle.GetAllAsync(x => x.SampleSizeId == id)).Count,

                ETradeType.PaperTrade or ETradeType.Trade =>
                    (await _unitOfWork.Trade.GetAllAsync(x => x.SampleSizeId == id)).Count,

                _ => 0
            };

            if (numberTradesInSampleSize == maxTradesProSampleSize) isFull = true;

            return (id, isFull);
        }

        #endregion
    }
}
