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

namespace TradingToolsRazor.Pages.Research
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private const int IndexMethod = 0;

        public IndexModel(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
            ResearchVM = new ResearchVM();
            ResearchVM.AllTrades = new List<object>();
        }

        [BindProperty]
        public ResearchVM ResearchVM { get; set; }

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

        // Handler for DELETE requests
        public async Task<IActionResult> OnDeleteAsync(int id, EStrategy strategy)
        {
            try
            {
                if (strategy == EStrategy.FirstBarPullback)
                {
                    return new JsonResult(new { error = "Delete method not implemented for this strategy." });
                    // return await DeleteFirstBarPullback(id);
                }
                else if (strategy == EStrategy.Cradle)
                {
                    return await DeleteCradle(id);
                }

                return new JsonResult(new { error = "Delete method not implemented for this strategy." });
            }
            catch (Exception ex)
            {
                return new JsonResult(new { error = $"Error in Delete(): {ex.Message})" });
            }
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
            return tradesInSampleSize; // +1 because of the deleted trade
        }
        private async Task<JsonResult> DeleteCradle(int id)
        {
            // Implementation remains mostly the same as in the controller
            ResearchCradle trade = await DeleteEntity(id);
            DeleteTradeDirectory(trade.ScreenshotsUrls!.First());

            var tradesInSampleSize = await CheckAndDeleteSampleSize(trade);
            var sampleSizes = await _unitOfWork.SampleSize
                .GetAllAsync(x => x.TradeType == ETradeType.Research && x.Strategy == EStrategy.Cradle);

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

        private void DeleteTradeDirectory(string screenshotPath)
        {
            string directoryToDelete = Path.GetDirectoryName(Path.Combine(_webHostEnvironment.WebRootPath, screenshotPath)!)!;
            Directory.Delete(directoryToDelete, true);
        }

        private async Task<ResearchCradle> DeleteEntity(int id)
        {
            ResearchCradle trade = await _unitOfWork.ResearchCradle.GetAsync(x => x.Id == id);
            _unitOfWork.ResearchCradle.Remove(trade);
            await _unitOfWork.SaveAsync();
            return trade;
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

            foreach (var s in filtered)
                if (!ResearchVM.AvailableTimeframes.Contains(s.TimeFrame))
                    ResearchVM.AvailableTimeframes.Add(s.TimeFrame);

            ResearchVM.AvailableTimeframes.Sort();
        }

        /// <summary>
        ///  The .zip file has to have the following format: Research/Sample Size 1/Trades folder and .csv file. The .csv file has to have format Research-EnumStrategy(e.g. 0)-EnumTimeFrame(e.g. 10M): e.g. Research-0-10M.csv
        /// </summary>
        /// <param name="zipFile"></param>
        /// <returns></returns>

        public async Task<IActionResult> UploadResearch(IFormFile zipFile)
        {
            if (zipFile == null || zipFile.Length == 0)
            {
                // return notification error
            }
            string wwwRootPath = _webHostEnvironment.WebRootPath;

            try
            {
                using (var zipStream = new MemoryStream())
                {
                    await zipFile.CopyToAsync(zipStream);
                    zipStream.Position = 0; // Reset the stream position to the beginning

                    using (var archive = new ZipArchive(zipStream))
                    {
                        // Sort the entries to have the folders in ascending order (Trade 1, Trade 2..)
                        List<ZipArchiveEntry> sortedEntries = [.. archive.Entries.OrderBy(e => e.FullName, new NaturalStringComparer())];
                        List<ResearchFirstBarPullback> researchTrades = new List<ResearchFirstBarPullback>();

                        ETimeFrame researchedTF;
                        int tradeIndex = 0;
                        int currentTrade = 0;
                        string currentSampleSize = string.Empty;
                        string currentFolder = string.Empty;
                        foreach (var entry in sortedEntries)
                        {
                            if (entry.FullName.Contains("Sample Size") && string.IsNullOrEmpty(currentSampleSize))
                            {
                                string[] researchInfo = entry.FullName.Split('/');
                                if (researchInfo[1].Contains("Sample Size"))
                                {
                                    currentSampleSize = researchInfo[1];
                                    continue;
                                }

                            }
                            if (entry.FullName.Contains(".csv"))
                            {
                                string[] researchInfo = entry.FullName.Split('-');
                                if (!Int32.TryParse(researchInfo[1], out int strategy))
                                {
                                    TempData["error"] = "Error parsing the strategy number from the csv file name. Check the file name.";
                                    // In a controller, to redirect to the Index method, use RedirectToAction:
                                    return RedirectToAction(nameof(Index));
                                }
                                string tempTF = researchInfo[2].Replace(".csv", "");
                                researchedTF = MyEnumConverter.TimeFrameFromString(tempTF).Value;
                                // Set the sample size for the research
                                SampleSize sampleSize = new SampleSize();
                                sampleSize.TradeType = ETradeType.Research;
                                sampleSize.Strategy = (EStrategy)strategy;
                                sampleSize.TimeFrame = researchedTF;
                                _unitOfWork.SampleSize.Add(sampleSize);
                                await _unitOfWork.SaveAsync();

                                // Parse the .csv data
                                using (var csvStream = entry.Open())
                                {
                                    var csvData = await ReadCsvFileAsync(csvStream);
                                    if (csvData != null)
                                    {
                                        ResearchFirstBarPullback researchTrade = new ResearchFirstBarPullback();
                                        for (int i = 0; i < csvData.Count; i++)
                                        {
                                            // First row is column names
                                            if (i == 0)
                                            {
                                                continue;
                                            }

                                            // Half ATR
                                            if (i % 2 != 0)
                                            {
                                                researchTrade.SampleSizeId = sampleSize.Id;
                                                researchTrade.OneToOneHitOn = csvData[i][1].Length > 0 ? int.Parse(csvData[i][1]) : 0;
                                                researchTrade.IsOneToThreeHit = csvData[i][2] == "Yes" ? true : false;
                                                researchTrade.IsOneToFiveHit = csvData[i][3] == "Yes" ? true : false;
                                                researchTrade.IsBreakeven = csvData[i][4] == "Yes" ? true : false;
                                                researchTrade.Outcome = csvData[i][5] == "Yes" ? EOutcome.Loss : EOutcome.Win;
                                                // Format in csvData[i][6] is 1-4. Split the string at '-` and get the second item. Then parse that into int.
                                                researchTrade.MaxRR = csvData[i][6].Length > 0 ? int.Parse(csvData[i][6].Split('-')[1]) : 0;
                                                researchTrade.MarketGaveSmth = csvData[i][7].Length > 0 ? true : false;
                                                researchTrade.IsEntryAfter3To5Bars = csvData[i][8] == "Yes" ? true : false;
                                                researchTrade.IsEntryAfter5Bars = csvData[i][9] == "Yes" ? true : false;
                                                researchTrade.IsEntryAtPreviousSwingOnTrigger = csvData[i][10] == "Yes" ? true : false;
                                                researchTrade.IsEntryBeforePreviousSwingOnTrigger = csvData[i][11] == "Yes" ? true : false;
                                                researchTrade.IsEntryBeforePreviousSwingOn4H = csvData[i][12] == "Yes" ? true : false;
                                                researchTrade.IsEntryBeforePreviousSwingOnD = csvData[i][13] == "Yes" ? true : false;
                                                researchTrade.IsMomentumTrade = csvData[i][14] == "Yes" ? true : false;
                                                researchTrade.IsTriggerTrending = csvData[i][15] == "Yes" ? true : false;
                                                researchTrade.Is4HTrending = csvData[i][16] == "Yes" ? true : false;
                                                researchTrade.IsDTrending = csvData[i][17] == "Yes" ? true : false;
                                                researchTrade.IsEntryAfteriBar = csvData[i][18] == "Yes" ? true : false;
                                                researchTrade.IsSignalBarStrongReversal = csvData[i][18] == "Yes" ? true : false;
                                                researchTrade.IsSignalBarInTradeDirection = csvData[i][19] == "Yes" ? true : false;
                                                researchTrade.Comment = csvData[i][22];
                                            }
                                            // Full ATR
                                            else
                                            {
                                                researchTrade.FullATROneToOneHitOn = csvData[i][1].Length > 0 ? int.Parse(csvData[i][1]) : 0;
                                                researchTrade.IsFullATROneToThreeHit = csvData[i][2] == "Yes" ? true : false;
                                                researchTrade.IsFullATROneToFiveHit = csvData[i][3] == "Yes" ? true : false;
                                                researchTrade.IsFullATRBreakeven = csvData[i][4] == "Yes" ? true : false;
                                                researchTrade.IsFullATRLoss = csvData[i][5] == "Yes" ? true : false;
                                                researchTrade.FullATRMaxRR = csvData[i][6].Length > 0 ? int.Parse(csvData[i][6].Split('-')[1]) : 0;
                                                researchTrade.MarketGaveSmth = csvData[i][7].Length > 0 ? true : false;
                                                researchTrades.Add(researchTrade);
                                                researchTrade = new ResearchFirstBarPullback();
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                // Inside the folder with the screenshots
                                if (entry.FullName.EndsWith(".png"))
                                {
                                    // Split the name to get the folder's name
                                    string[] tradeInfo = entry.FullName.Split('/');
                                    // What is left is "Trade x", x is the number of the trade. Remove "Trade" to get the number.
                                    string tempTradeInfo = tradeInfo[3].Replace("Trade", "").Trim();
                                    if (Int32.TryParse(tempTradeInfo, out int tempTradeIndex))
                                    {
                                        tradeIndex = tempTradeIndex - 1;
                                    }
                                    currentFolder = ScreenshotsHelper.CreateScreenshotFolders(tradeInfo, currentFolder, entry.FullName, wwwRootPath, 3);

                                    if (researchTrades[tradeIndex].ScreenshotsUrls == null)
                                    {
                                        researchTrades[tradeIndex].ScreenshotsUrls = new List<string>();
                                    }

                                    entry.ExtractToFile(Path.Combine(currentFolder, entry.Name));
                                    string screenshotName = entry.FullName.Split('/').Last();
                                    string screenshotPath = currentFolder.Replace(wwwRootPath, "").Replace("\\", "/");
                                    researchTrades[tradeIndex].ScreenshotsUrls.Add(Path.Combine(screenshotPath, screenshotName));
                                }
                            }
                        }
                        _unitOfWork.ResearchFirstBarPullback.AddRange(researchTrades);
                        await _unitOfWork.SaveAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error parsing the csv file. Error message: {ex.Message}");
                TempData["error"] = $"Error parsing the csv file. Error message: {ex.Message}";
            }

            async Task<List<List<string>>> ReadCsvFileAsync(Stream csvStream)
            {
                var result = new List<List<string>>();
                using (var reader = new StreamReader(csvStream))
                {
                    while (await reader.ReadLineAsync() is string line)
                    {
                        var values = line.Split(',').ToList();
                        result.Add(values);
                    }
                }

                return result;
            }

            return RedirectToAction(nameof(Index));
        }


        #region Private Methods

        private async Task<string> LoadViewModelData(List<SampleSize> sampleSizes, int sampleSizeNumber)
        {
            int lastSampleSizeId = SetLastSampleSizeId(sampleSizes, ref sampleSizeNumber);
            if (lastSampleSizeId == -1)
            {
                return $"Error in setting the lastSampleSizeId in {nameof(SetLastSampleSizeId)}";
            }
            SampleSize sampleSize = sampleSizes.FirstOrDefault(sampleSize => sampleSize.Id == lastSampleSizeId)!;
            await SetTrades();
            SetValuesForButtons();
            SetScreenShotsUrls();

            return ResearchVM.AllTrades.Any() ? string.Empty : "No trades available for this sample size.";

            #region Helper Methods

            async Task SetTrades()
            {
                if (sampleSize.Strategy == EStrategy.Cradle)
                {
                    ResearchVM.AllTrades = (await _unitOfWork.ResearchCradle
                        .GetAllAsync(x => x.SampleSizeId == lastSampleSizeId)).Cast<object>().ToList();
                    ResearchVM.ResearchCradle = (ResearchVM.AllTrades.FirstOrDefault() as ResearchCradle)!;
                }
                else if (sampleSize.Strategy == EStrategy.FirstBarPullback)
                {
                    // Get all researched trades from the DB and project the instances into ResearchFirstBarPullbackDisplay
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
                if (ResearchVM.CurrentSampleSize.Strategy == EStrategy.Cradle)
                {
                    ResearchVM.TradeData.ScreenshotsUrls = [.. (ResearchVM.AllTrades.FirstOrDefault()! as BaseTrade)!.ScreenshotsUrls!];
                }
                else
                {
                    // Workaround - load the ScreenshotUrls from BaseTrade and map them to the IDs from TradeData...
                    ResearchVM.TradeData.ScreenshotsUrls = [.. (ResearchVM.AllTrades.FirstOrDefault()! as ResearchFirstBarPullbackDisplay)!.ScreenshotsUrls!];
                }
            }

            int SetLastSampleSizeId(List<SampleSize> sampleSizes, ref int sampleSizeNumber)
            {
                if (sampleSizeNumber == IndexMethod)
                    return sampleSizes.LastOrDefault()?.Id ?? -1;

                int lastSampleSizeId = -1;
                var currentTimeFrame = ResearchVM.CurrentTimeFrame;

                if (ResearchVM.HasStrategyChanged)
                {
                    lastSampleSizeId = sampleSizes.LastOrDefault()!.Id;
                    sampleSizeNumber = sampleSizes.Count(x => x.TimeFrame == sampleSizes.LastOrDefault()!.TimeFrame);
                }
                else if (ResearchVM.HasTimeFrameChanged && ResearchVM.HasSampleSizeChanged)
                {
                    var filtered = sampleSizes.Where(s => s.TimeFrame == currentTimeFrame).ToList();
                    lastSampleSizeId = filtered.ElementAtOrDefault(sampleSizeNumber - 1)?.Id ?? -1;
                }
                else if (ResearchVM.HasTimeFrameChanged)
                {
                    var filtered = sampleSizes.Where(s => s.TimeFrame == currentTimeFrame).ToList();
                    var last = filtered.LastOrDefault();
                    lastSampleSizeId = last?.Id ?? -1;
                    sampleSizeNumber = filtered.Count;
                }
                else if (ResearchVM.HasSampleSizeChanged)
                {
                    lastSampleSizeId = sampleSizes.ElementAtOrDefault(sampleSizeNumber - 1)?.Id ?? -1;
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

        #endregion
    }
}
