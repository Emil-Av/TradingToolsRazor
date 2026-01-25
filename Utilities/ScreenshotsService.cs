using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Models.ViewModels;
using Shared;
using System.Diagnostics;
using Models;
using Shared.Enums;

namespace Utilities
{
    /// <summary>
    ///  Provides static methods for creating folders and saving screenshot files.
    /// </summary>
    public static class ScreenshotsService
    {
        /// <summary>
        ///  Creates the folder for the new trade and saves the screenshot files in it.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="webRootPath"></param>
        /// <param name="vm"></param>
        /// <param name="newTrade"></param>
        /// <param name="files"></param>
        /// <returns></returns>
        public static async Task<List<string>> SaveFilesAsync<T>(string webRootPath, T vm, object newTrade, IFormFile[] files, bool isSampleSizeFull)
        {
            if (vm is not NewTradeVM viewModel)
            {
                return new List<string>();
            }

            string screenshotsDir = GetScreenshotsDir(webRootPath);
            EnsureDirectoryExists(screenshotsDir);

            string tradeFolderPath = BuildTradeFolderPath(screenshotsDir, viewModel, newTrade, isSampleSizeFull);

            return await SaveFilesToDiskAsync(webRootPath, tradeFolderPath, files);
        }

        private static string BuildTradeFolderPath(string screenshotsDir, NewTradeVM viewModel, object newTrade, bool isSampleSizeFull)
        {
            string pathToSaveFiles = Path.Combine(screenshotsDir, MyEnumConverter.TradeTypeFromEnum(viewModel.SampleSizeViewData.SampleSizeType));
            EnsureDirectoryExists(pathToSaveFiles);

            pathToSaveFiles = Path.Combine(pathToSaveFiles, newTrade.GetType().Name);
            EnsureDirectoryExists(pathToSaveFiles);

            string timeFrame = MyEnumConverter.TimeFrameFromEnum(viewModel.TimeFrame);
            pathToSaveFiles = Path.Combine(pathToSaveFiles, timeFrame);
            EnsureDirectoryExists(pathToSaveFiles);

            return DetermineTradeFolderPath(pathToSaveFiles, isSampleSizeFull);
        }

        private static string DetermineTradeFolderPath(string basePath, bool isSampleSizeFull)
        {
            string[] sampleSizeFolders = Directory.GetDirectories(basePath);

            if (sampleSizeFolders.Length == 0)
            {
                string newPath = Path.Combine(basePath, "Sample Size 1", "Trade 1");
                Directory.CreateDirectory(newPath);
                return newPath;
            }

            string lastSampleSizeFolder = sampleSizeFolders.Last();
            string[] tradesFolderInLastSampleSize = Directory.GetDirectories(lastSampleSizeFolder);

            if (!isSampleSizeFull)
            {
                return CreateTradeFolder(lastSampleSizeFolder, tradesFolderInLastSampleSize.Length + 1);
            }

            return CreateNewSampleSizeFolder(basePath, sampleSizeFolders.Length + 1);
        }

        private static string CreateTradeFolder(string sampleSizeFolder, int tradeNumber)
        {
            string tradeFolderPath = Path.Combine(sampleSizeFolder, $"Trade {tradeNumber}");
            Directory.CreateDirectory(tradeFolderPath);
            return tradeFolderPath;
        }

        private static string CreateNewSampleSizeFolder(string basePath, int sampleSizeNumber)
        {
            string sampleSizePath = Path.Combine(basePath, $"Sample Size {sampleSizeNumber}");
            Directory.CreateDirectory(sampleSizePath);

            string firstTradePath = Path.Combine(sampleSizePath, "Trade 1");
            Directory.CreateDirectory(firstTradePath);

            return firstTradePath;
        }

        private static async Task<List<string>> SaveFilesToDiskAsync(string webRootPath, string destinationPath, IFormFile[] files)
        {
            List<string> screenshotsPaths = new List<string>();
            string downloadFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

            try
            {
                foreach (IFormFile file in files)
                {
                    string filePath = Path.Combine(destinationPath, file.FileName);

                    using (Stream stream = new FileStream(filePath, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }

                    string dbFilePath = Path.GetRelativePath(webRootPath, filePath).Replace("\\", "/");
                    screenshotsPaths.Add(dbFilePath);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error in saving uploaded files: {ex.Message}");
            }

            return screenshotsPaths;
        }

        private static void EnsureDirectoryExists(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public static string GetScreenshotsDir(string webRootPath)
        {
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == EEnvironment.Production.ToString())
            {
                return Path.Combine(webRootPath, "Screenshots");
            }

            return Path.Combine(webRootPath, "ScreenshotsDev");
        }

        /// <summary>
        ///  Creates the folders in wwwroot\Screenshots for the screenshots when uploading a .zip file for Trades or Research
        /// </summary>
        /// <param name="tradeInfo"></param>
        /// <param name="currentFolder"></param>
        /// <param name="entryFullName"></param>
        /// <param name="wwwRootPath"></param>
        /// <param name="numberFolderToCreate"></param>
        /// <returns></returns>
        public static string CreateScreenshotFolders(string[] tradeInfo, string currentFolder, string entryFullName, string wwwRootPath, int numberFolderToCreate)
        {
            List<string> folders = new List<string>();
            // wwwroot\Screenshots
            string screenshotsFolder = Path.Combine(wwwRootPath, "Screenshots");
            if (!Directory.Exists(screenshotsFolder))
            {
                // Create wwwroot\Screenshots
                Directory.CreateDirectory(screenshotsFolder);
            }
            string tradeType = string.Empty;
            if (tradeInfo[0].Contains("Research"))
            {
                tradeType = "Research\\" + tradeInfo[0];
            }
            else
            {
                tradeType = tradeInfo[0];
            }
            currentFolder = Path.Combine(screenshotsFolder, tradeType);
            if (!Directory.Exists(currentFolder))
            {
                // Create View folder (e.g. wwwroot\Screenshots\Trades
                Directory.CreateDirectory(currentFolder);
            }
            // Get all subfolders
            for (int i = 1; i <= numberFolderToCreate; i++)
            {
                // No need for "Reviews" folder (when the method is called from PapersView)
                if (!tradeInfo[i].Contains("Reviews"))
                {
                    folders.Add(tradeInfo[i]);
                }
            }
            // Create all subfolders
            for (int i = 0; i < folders.Count; i++)
            {
                currentFolder = Path.Combine(currentFolder, folders[i]);
                if (!Directory.Exists(currentFolder))
                {
                    Directory.CreateDirectory(currentFolder);
                }
            }

            return currentFolder;
        }
    }
}
