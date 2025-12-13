using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using Models.ViewModels;
using Models.ViewModels.DisplayClasses;
using Newtonsoft.Json;
using Shared;
using Shared.Enums;
using SharedEnums.Enums;
using System.Diagnostics;
using System.IO.Compression;
using Utilities;
using System.Text.RegularExpressions;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Utilities.Trade;

namespace TradingToolsRazor.Pages.Research
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const int IndexMethod = 0;
        private readonly DeleteTradeHelper _deleteTradeHelper;

        public IndexModel(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment, DeleteTradeHelper deleteTradeHelper)
        {
            ResearchVM = new ();
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            _deleteTradeHelper = deleteTradeHelper;
        }

        public ResearchVM ResearchVM { get; set; }

        #region Handlers

        // Handler for GET /Research/Index
        public async Task<IActionResult> OnGetAsync()
        {
            var sampleSizes = await _unitOfWork.SampleSize.GetAllAsync(x => x.TradeType == ETradeType.Research);

            if (!sampleSizes.Any())
                return Page();

            string errorMsg = await LoadViewModelData(sampleSizes, 0);
            if (!string.IsNullOrEmpty(errorMsg))
                return new JsonResult(new { error = errorMsg });

            SetAvailableTimeframes(sampleSizes);
            SetAvailableStrategies(sampleSizes);

            return Page();
        }

        // DELETE handler (can be called via fetch to ?handler=Delete)
        public async Task<IActionResult> OnDeleteAsync(int id, EStrategy strategy)
        {
            try
            {
                if (strategy == EStrategy.FirstBarPullback)
                {
                    return new JsonResult(new { error = "Delete method not implemented for this strategy." });
                }
                else if (strategy == EStrategy.Cradle)
                {
                    return await DeleteCradle(id);
                }
                else if (strategy == EStrategy.CandleBracketing)
                {
                    return await DeleteCandleBracketing(id);
                }

                return new JsonResult(new { error = "Delete method not implemented for this strategy." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = $"Error in Delete(): {ex.Message})" });
            }
        }

        // POST: /Research?handler=LoadSampleSize
        public async Task<IActionResult> OnPostLoadSampleSizeAsync([FromForm] LoadResearchSampleSize viewData)
        {
            string errorMsg = ResearchVM.SetSampleSizeParams(viewData);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                return new JsonResult(new { error = errorMsg });
            }

            List<SampleSize> sampleSizes = await _unitOfWork.SampleSize.GetAllAsync(x => x.TradeType == ETradeType.Research && x.Strategy == ResearchVM.CurrentStrategy);

            if (!sampleSizes.Any())
            {
                return new JsonResult(new { error = "No sample sizes for those params." });
            }

            errorMsg = await LoadViewModelData(sampleSizes, ResearchVM.CurrentSampleSizeId);
            if (!string.IsNullOrEmpty(errorMsg))
            {
                return new JsonResult(new { error = errorMsg });
            }

            string researchVM = JsonConvert.SerializeObject(ResearchVM);

            return new JsonResult(new { researchVM });
        }

        public async Task<IActionResult> OnPostUpdateCandleBracketingResearchAsync([FromBody] ResearchCandleBracketing researchCandleBracketing)
        {
            var validationResult = ValidateModelState();
            if (validationResult != null)
                return validationResult;

            await _unitOfWork.ResearchCandleBracketing.UpdateAsync(researchCandleBracketing);
            try
            {
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = $"{ex.Message}" });
            }

            return new JsonResult(new { success = "Trade updated." });
        }

        public async Task<IActionResult> OnPostUpdateCradleResearchAsync([FromBody] ResearchCradle researchTrade)
        {
            var validationResult = ValidateModelState();
            if (validationResult != null)
                return validationResult;

            await _unitOfWork.ResearchCradle.UpdateAsync(researchTrade);
            try
            {
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = $"{ex.Message}" });
            }

            return new JsonResult(new { success = "Trade updated." });
        }

        public async Task<IActionResult> OnPostUpdateFirstBarResearchAsync([FromBody] ResearchFirstBarPullbackDisplay currentTrade)
        {
            var validationResult = ValidateModelState();
            if (validationResult != null)
                return validationResult;


            ResearchFirstBarPullback trade = EntityMapper.ViewModelDisplayToEntity<ResearchFirstBarPullback, ResearchFirstBarPullbackDisplay>(currentTrade, existingEntity: null);
            await _unitOfWork.ResearchFirstBarPullback.UpdateAsync(trade);
            try
            {
                await _unitOfWork.SaveAsync();
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = $"Error saving the data: {ex.Message}" });
            }
            return new JsonResult(new { success = "Trade was successfully updated" });
        }

        #endregion

        #region Private Methods

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

        private async Task<List<ResearchCandleBracketing>> CheckAndDeleteSampleSize(ResearchCandleBracketing trade)
        {
            List<ResearchCandleBracketing> tradesInSampleSize = await _unitOfWork.ResearchCandleBracketing.GetAllAsync(x => x.SampleSizeId == trade.SampleSizeId);
            if (!tradesInSampleSize.Any())
            {
                SampleSize sampleSize = await _unitOfWork.SampleSize.GetAsync(x => x.Id == trade.SampleSizeId);
                if (sampleSize != null)
                {
                    _unitOfWork.SampleSize.Remove(sampleSize);
                    await _unitOfWork.SaveAsync();
                }
            }
            return tradesInSampleSize;
        }

        private async Task<List<ResearchCradle>> CheckAndDeleteSampleSize(ResearchCradle trade)
        {
            List<ResearchCradle> tradesInSampleSize = await _unitOfWork.ResearchCradle.GetAllAsync(x => x.SampleSizeId == trade.SampleSizeId);
            if (!tradesInSampleSize.Any())
            {
                SampleSize sampleSize = await _unitOfWork.SampleSize.GetAsync(x => x.Id == trade.SampleSizeId);
                if (sampleSize != null)
                {
                    _unitOfWork.SampleSize.Remove(sampleSize);
                    await _unitOfWork.SaveAsync();
                }
            }
            return tradesInSampleSize;
        }

        private async Task<JsonResult> DeleteCandleBracketing(int id)
        {
            ResearchCandleBracketing trade = await DeleteCandleBracketingEntity(id);

            var tradesInSampleSize = await CheckAndDeleteSampleSize(trade);
            var samplesizes = await _unitOfWork.SampleSize
                .GetAllAsync(x => x.TradeType == ETradeType.Research && x.Strategy == EStrategy.CandleBracketing);

            await _deleteTradeHelper.UpdateScreenshotPathsAfterDeletion(trade.ScreenshotsUrls!.First(), [.. tradesInSampleSize.Cast<BaseTrade>()], _webHostEnvironment.WebRootPath);

            if (!TrySetLastSampleSizeId(tradesInSampleSize, samplesizes, trade, out int lastSampleSizeId))
                return new JsonResult(new { redirectUrl = Url.Page("/Research/Index") });

            if (samplesizes.Any())
            {
                await LoadViewModelData(samplesizes, lastSampleSizeId);
                string researchVM = JsonConvert.SerializeObject(ResearchVM);
                return new JsonResult(new { researchVM });
            }

            return new JsonResult(new { error = "No more trades for this strategy." });
        }

        private async Task<ResearchCandleBracketing> DeleteCandleBracketingEntity(int id)
        {
            ResearchCandleBracketing trade = await _unitOfWork.ResearchCandleBracketing.GetAsync(x => x.Id == id);
            _unitOfWork.ResearchCandleBracketing.Remove(trade);
            await _unitOfWork.SaveAsync();

            return trade;
        }

        private async Task<JsonResult> DeleteCradle(int id)
        {
            ResearchCradle trade = await DeleteCradleEntity(id);

            var tradesInSampleSize = await CheckAndDeleteSampleSize(trade);
            var sampleSizes = await _unitOfWork.SampleSize
                .GetAllAsync(x => x.TradeType == ETradeType.Research && x.Strategy == EStrategy.Cradle);

            await _deleteTradeHelper.UpdateScreenshotPathsAfterDeletion(trade.ScreenshotsUrls.First(), tradesInSampleSize.Cast<BaseTrade>().ToList(), _webHostEnvironment.WebRootPath);

            if (!TrySetLastSampleSizeId(tradesInSampleSize, sampleSizes, trade, out int lastSampleSizeId))
                return new JsonResult(new { redirectUrl = Url.Page("/Research/Index") });

            if (sampleSizes.Any())
            {
                await LoadViewModelData(sampleSizes, lastSampleSizeId);
                string researchVM = JsonConvert.SerializeObject(ResearchVM);
                return new JsonResult(new { researchVM });
            }

            return new JsonResult(new { error = "No more trades for this strategy." });
        }



        private async Task<ResearchCradle> DeleteCradleEntity(int id)
        {
            ResearchCradle trade = await _unitOfWork.ResearchCradle.GetAsync(x => x.Id == id);
            _unitOfWork.ResearchCradle.Remove(trade);
            await _unitOfWork.SaveAsync();
            return trade;
        }

        private async Task<JsonResult> DeleteFirstBarPullback(int id)
        {
            ResearchFirstBarPullback trade = await _unitOfWork.ResearchFirstBarPullback.GetAsync(x => x.Id == id);
            if (trade == null)
            {
                return new JsonResult(new { error = "No trade was found for this id." });
            }

            SampleSize sampleSize = await _unitOfWork.SampleSize.GetAsync(x => x.Id == trade.SampleSizeId);
            _unitOfWork.ResearchFirstBarPullback.Remove(trade);
            await _unitOfWork.SaveAsync();

            // Get the rest of the trades in this sample size
            List<ResearchFirstBarPullback> listAllTrades = await _unitOfWork.ResearchFirstBarPullback.GetAllAsync(x => x.SampleSizeId == trade.SampleSizeId);
            List<SampleSize> sampleSizes = null;
            string jsonTrades = string.Empty;

            // The sample size is empty now
            if (!listAllTrades.Any())
            {
                // Delete the empty sample size
                if (sampleSize != null)
                {
                    _unitOfWork.SampleSize.Remove(sampleSize);
                    await _unitOfWork.SaveAsync();
                }

                // Check if there are more sample sizes for the paramaters. If yes get the last
                sampleSizes = await _unitOfWork.SampleSize.GetAllAsync(x => x.Strategy == sampleSize!.Strategy && x.TimeFrame == sampleSize.TimeFrame && x.TradeType == ETradeType.Research);

                int lastSampleSizeId = 0;
                // No more sample sizes for these parameters. The trade that was deleted was the last for these paramaters
                if (!sampleSizes.Any())
                {
                    ResearchVM.AvailableTimeframes.Remove(sampleSize!.TimeFrame);
                }
                // Get the last sample size id for these paramaters
                else
                {
                    lastSampleSizeId = sampleSizes.LastOrDefault()!.Id;
                }

                // Get all trades for the last sample size id for these paramaters
                if (lastSampleSizeId != 0)
                {
                    listAllTrades = await _unitOfWork.ResearchFirstBarPullback.GetAllAsync(x => x.SampleSizeId == lastSampleSizeId);
                }
                // Check if there are any other sample sizes (any TF, any Strategy)
                else
                {
                    sampleSizes = await _unitOfWork.SampleSize.GetAllAsync(x => x.TradeType == ETradeType.Research);

                    if (sampleSizes.Any())
                    {
                        lastSampleSizeId = sampleSizes.LastOrDefault()!.Id;
                        listAllTrades = await _unitOfWork.ResearchFirstBarPullback.GetAllAsync(x => x.SampleSizeId == lastSampleSizeId);
                    }
                }
            }

            if (listAllTrades.Any())
            {
                if (sampleSizes == null)
                {
                    sampleSizes = await _unitOfWork.SampleSize.GetAllAsync(x => x.Id == listAllTrades.First().SampleSizeId);
                }
                foreach (ResearchFirstBarPullback researchFirstBarPullback in listAllTrades)
                {
                    ResearchVM.AllTrades.Add(EntityMapper.EntityToViewModel<ResearchFirstBarPullback, ResearchFirstBarPullbackDisplay>(researchFirstBarPullback));
                }
            }
            else
            {
                return new JsonResult(new { redirectUrl = Url.Page("/Research/Index") });
            }

            // Set the values for the view
            SampleSize currentSampleSize = sampleSizes.SingleOrDefault(x => x.Id == listAllTrades[0].SampleSizeId)!;
            ResearchVM.CurrentStrategy = currentSampleSize.Strategy;
            ResearchVM.CurrentTimeFrame = currentSampleSize.TimeFrame;
            ResearchVM.CurrentSampleSizeNumber = sampleSizes.Count;
            ResearchVM.TradesInSampleSize = listAllTrades.Count;
            ResearchVM.NumberSampleSizes = sampleSizes.Count;
            string researchVM = JsonConvert.SerializeObject(ResearchVM);
            // The method should be able to delete the sample size, and then get the trades from the last sample size for the given params.
            // Convert the trades and the new menu values in json and return that.
            return new JsonResult(new { researchVM });
        }

        private async Task<string> LoadViewModelData(List<SampleSize> sampleSizes, int sampleSizeNumber)
        {
            int lastSampleSizeId = GetLastSampleSizeId(sampleSizes, ref sampleSizeNumber);
            if (lastSampleSizeId == -1)
            {
                return $"Error in setting the lastSampleSizeId in {nameof(GetLastSampleSizeId)}";
            }
            SampleSize sampleSize = sampleSizes.FirstOrDefault(sampleSize => sampleSize.Id == lastSampleSizeId)!;
            await SetTrades();
            SetValuesForButtons();
            SetScreenShotsUrls();
            
            // Should not happen. Empty sample size should not exist. If so, probably error in delete trade method.
            return ResearchVM.AllTrades.Any() ? string.Empty : "No trades available for this sample size.";

            #region Helper Methods

            async Task SetTrades()
            {
                if (sampleSize.Strategy == EStrategy.Cradle)
                {
                    ResearchVM.AllTrades = (await _unitOfWork.ResearchCradle
                                                                            .GetAllAsync(x => x.SampleSizeId == lastSampleSizeId))
                                                                            .Cast<object>()
                                                                            .ToList();
                    ResearchVM.ResearchCradle = (ResearchVM.AllTrades.FirstOrDefault() as ResearchCradle)!;
                }
                else if (sampleSize.Strategy == EStrategy.CandleBracketing)
                {
                    ResearchVM.AllTrades = (await _unitOfWork.ResearchCandleBracketing
                                                                                    .GetAllAsync(x => x.SampleSizeId == lastSampleSizeId))
                                                                                    .OrderBy(x => x.Date)
                                                                                    .Cast<object>()
                                                                                    .ToList();
                    ResearchVM.CandleBracketing = (ResearchVM.AllTrades.FirstOrDefault() as ResearchCandleBracketing)!;
                }
                else if (sampleSize.Strategy == EStrategy.FirstBarPullback)
                {
                    ResearchVM.AllTrades = (await _unitOfWork.ResearchFirstBarPullback
                                            .GetAllAsync(x => x.SampleSizeId == lastSampleSizeId))
                                            .Select(EntityMapper.EntityToViewModel<ResearchFirstBarPullback, ResearchFirstBarPullbackDisplay>)
                                            .Cast<object>()
                                            .ToList();
                    ResearchVM.ResearchFirstBarPullbackDisplay = (ResearchVM.AllTrades.FirstOrDefault() as ResearchFirstBarPullbackDisplay)!;
                }
            }

            void SetValuesForButtons()
            {
                // Set the values for the button menus
                ResearchVM.CurrentSampleSize = sampleSizes.FirstOrDefault(x => x.Id == lastSampleSizeId)!;
                ResearchVM.CurrentTimeFrame = ResearchVM.CurrentSampleSize.TimeFrame;

                ResearchVM.CurrentSampleSizeId = lastSampleSizeId;
                ResearchVM.CurrentStrategy = ResearchVM.CurrentSampleSize.Strategy;
                SetCurrentSampleSizeNumber(sampleSizeNumber, lastSampleSizeId, sampleSizes);
                // Set the NumberSampleSizes for the button menu
                ResearchVM.NumberSampleSizes = sampleSizes.Count(x => x.TimeFrame == ResearchVM.CurrentTimeFrame && x.Strategy == ResearchVM.CurrentSampleSize.Strategy);
                ResearchVM.TradesInSampleSize = ResearchVM.AllTrades.Count;
                SetAvailableTimeframes(sampleSizes);
            }

            void SetScreenShotsUrls()
            {
                if (ResearchVM.AllTrades.Any())
                {

                    if (ResearchVM.CurrentSampleSize.Strategy == EStrategy.Cradle)
                    {
                        ResearchVM.TradeData.ScreenshotsUrls = (ResearchVM.AllTrades.FirstOrDefault()! as BaseTrade)!.ScreenshotsUrls!;
                    }
                    else if (ResearchVM.CurrentSampleSize.Strategy == EStrategy.CandleBracketing)
                    {
                        ResearchVM.TradeData.ScreenshotsUrls = (ResearchVM.AllTrades.FirstOrDefault()! as BaseTrade)!.ScreenshotsUrls!;
                    }
                    else
                    {
                        // Workaround - load the ScreenshotUrls from BaseTrade and map them to the IDs from TradeData...
                        ResearchVM.TradeData.ScreenshotsUrls = new List<string>((ResearchVM.AllTrades.FirstOrDefault()! as ResearchFirstBarPullbackDisplay)!.ScreenshotsUrls!);
                    }
                }
            }

            int GetLastSampleSizeId(List<SampleSize> sampleSizes, ref int sampleSizeNumber)
            {
                if (sampleSizeNumber == IndexMethod)
                    return sampleSizes.LastOrDefault()?.Id ?? -1;

                int lastSampleSizeId = -1;

                if (ResearchVM.HasStrategyChanged)
                {
                    lastSampleSizeId = sampleSizes.LastOrDefault()!.Id;
                    sampleSizeNumber = sampleSizes.Count(x => x.TimeFrame == sampleSizes.LastOrDefault()!.TimeFrame);
                }
                else if (ResearchVM.HasTimeFrameChanged && ResearchVM.HasSampleSizeChanged)
                {
                    var filtered = sampleSizes.Where(s => s.TimeFrame == ResearchVM.CurrentTimeFrame).ToList();
                    lastSampleSizeId = filtered.ElementAtOrDefault(sampleSizeNumber - 1)?.Id ?? -1;
                }
                else if (ResearchVM.HasTimeFrameChanged)
                {
                    var filtered = sampleSizes.Where(s => s.TimeFrame == ResearchVM.CurrentTimeFrame).ToList();
                    var last = filtered.LastOrDefault();
                    lastSampleSizeId = last?.Id ?? -1;
                    sampleSizeNumber = filtered.Count;
                }
                else if (ResearchVM.HasSampleSizeChanged)
                {
                    lastSampleSizeId = sampleSizes.Where(sampleSize => sampleSize.TimeFrame == ResearchVM.CurrentTimeFrame).ElementAt(sampleSizeNumber - 1).Id;
                }
                else
                {
                    return sampleSizeNumber;
                }

                return lastSampleSizeId;
            }

            #endregion
        }

        private void SetCurrentSampleSizeNumber(int sampleSizeNumber, int lastSampleSizeId, List<SampleSize> sampleSizes)
        {
            bool isDeletingCradle = sampleSizeNumber == lastSampleSizeId;
            if (isDeletingCradle)
            {
                // Finde alle SampleSizes mit dem aktuellen TimeFrame
                var matchingTimeFrames = sampleSizes
                    .Where(s => s.TimeFrame == ResearchVM.CurrentSampleSize.TimeFrame)
                    .ToList();

                // Ermittle die Position des aktuellen SampleSize in dieser Liste (1-basiert)
                int index = matchingTimeFrames.IndexOf(ResearchVM.CurrentSampleSize) + 1;

                ResearchVM.CurrentSampleSizeNumber = index > 0 ? index : sampleSizeNumber;
            }
            else
            {
                ResearchVM.CurrentSampleSizeNumber = sampleSizeNumber;
            }
        }

        private bool TrySetLastSampleSizeId(List<ResearchCandleBracketing> tradesInSampleSize, List<SampleSize> sampleSizes, ResearchCandleBracketing trade, out int lastSampleSizeId)
        {
            lastSampleSizeId = 0;
            if (tradesInSampleSize.Any())
            {
                lastSampleSizeId = trade.SampleSizeId;
                return true;
            }
            else if (sampleSizes.Any())
            {
                lastSampleSizeId = sampleSizes.Last().Id;
                return true;
            }

            return false;
        }

        private bool TrySetLastSampleSizeId(List<ResearchCradle> tradesInSampleSize, List<SampleSize> sampleSizes, ResearchCradle trade, out int lastSampleSizeId)
        {
            lastSampleSizeId = 0;
            if (tradesInSampleSize.Any())
            {
                lastSampleSizeId = trade.SampleSizeId;
                return true;
            }
            else if (sampleSizes.Any())
            {
                lastSampleSizeId = sampleSizes.Last().Id;
                return true;
            }

            return false;
        }

        private void SetAvailableStrategies(List<SampleSize> sampleSizes)
        {
            foreach (var sampleSize in sampleSizes)
            {
                if (!ResearchVM.AvailableStrategies.Contains(sampleSize.Strategy))
                    ResearchVM.AvailableStrategies.Add(sampleSize.Strategy);
            }

            ResearchVM.AvailableStrategies.Sort();
        }

        private void SetAvailableTimeframes(List<SampleSize> sampleSizes)
        {
            var currentStrategy = ResearchVM.CurrentSampleSize?.Strategy ?? EStrategy.FirstBarPullback;
            var filtered = sampleSizes.Where(x => x.Strategy == currentStrategy).ToList();

            foreach (var strategy in filtered)
                if (!ResearchVM.AvailableTimeframes.Contains(strategy.TimeFrame))
                    ResearchVM.AvailableTimeframes.Add(strategy.TimeFrame);

            ResearchVM.AvailableTimeframes.Sort();
        }

        #endregion
    }
}