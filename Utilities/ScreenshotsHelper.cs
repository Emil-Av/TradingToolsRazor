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
    public static class ScreenshotsHelper
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
        public static async Task<List<string>> SaveFilesAsync<T>(string webRootPath, T vm, object newTrade, IFormFile[] files)
        {
            #region Create folders
            // /Screenshots
            string screenshotsDir = GetScreenshotsDir(webRootPath);
            List<string> screenshotsPaths = new List<string>();
            if (!Directory.Exists(screenshotsDir))
            {
                Directory.CreateDirectory(screenshotsDir);
            }

            if (vm is NewTradeVM viewModel)
            {
                string pathToSaveFiles = string.Empty;
                string tradeType = MyEnumConverter.TradeTypeFromEnum(viewModel.TradeType);
                pathToSaveFiles = Path.Combine(screenshotsDir, tradeType);
                if (!Directory.Exists(pathToSaveFiles))
                {
                    // /Screenshots/Research(e.g.)
                    Directory.CreateDirectory(pathToSaveFiles);
                }
                if (tradeType == MyEnumConverter.TradeTypeFromEnum(SharedEnums.Enums.ETradeType.Research))
                {
                    // /Screenshots/Research/FirstBarPullback(e.g.)
                    pathToSaveFiles = Path.Combine(pathToSaveFiles, newTrade.GetType().Name);
                    if (!Directory.Exists(pathToSaveFiles))
                    {
                        Directory.CreateDirectory(pathToSaveFiles);
                    }
                }

                CreateTimeFrameFolder();

                // /Screenshots/Research/(typeResearch/)TimeFrame/Sample Size 1(e.g.)
                string[] sampleSizeFolders = Directory.GetDirectories(pathToSaveFiles);
                if (sampleSizeFolders.Length > 0)
                {
                    string lastSampleSizeFolder = sampleSizeFolders.Last();
                    // Trade directories in the sample size e.g. Screenshots/Research/FirstBarPullback/10M/Sample Size 1/Trade 2
                    string[] tradesFolderInLastSampleSize = Directory.GetDirectories(lastSampleSizeFolder);
                    // Check the number of trades of the last sample size
                    if (tradesFolderInLastSampleSize.Length < 100)
                    {
                        // create a folder for the trade
                        pathToSaveFiles = lastSampleSizeFolder;
                        pathToSaveFiles = Path.Combine(Path.Combine(pathToSaveFiles, $"Trade {tradesFolderInLastSampleSize.Length + 1}"));
                        Directory.CreateDirectory(pathToSaveFiles);
                    }
                    // Last sample size is full, create new one
                    else
                    {
                        pathToSaveFiles = Path.Combine(pathToSaveFiles, $"Sample Size {sampleSizeFolders.Length + 1}");
                        Directory.CreateDirectory(pathToSaveFiles);
                        // Create the folder for the first trade of the new sample size
                        pathToSaveFiles = Path.Combine(pathToSaveFiles, "Trade 1");
                        Directory.CreateDirectory(pathToSaveFiles);
                    }
                }
                else
                {
                    // Create the 1st sample size and the first trade directories
                    pathToSaveFiles = Path.Combine(pathToSaveFiles, "Sample Size 1", "Trade 1");
                    Directory.CreateDirectory(pathToSaveFiles);
                }

                #endregion

                // Save the files
                try
                {
                    string downloadFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
                    foreach (IFormFile file in files)
                    {
                        string filePath = Path.Combine(pathToSaveFiles, file.FileName);
                        using (Stream stream = new FileStream(filePath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                            var dbFilePath = Path.GetRelativePath(webRootPath, filePath).Replace("\\", "/");
                            //BaseTrade trade = newTrade as BaseTrade; wtf?
                            //trade.ScreenshotsUrls!.Add(dbFilePath); wtf?
                            screenshotsPaths.Add(dbFilePath);
                        }
                        string downloadedFilePath = Path.Combine(downloadFolder, file.FileName);
                        // Delete the file from the download directory
                        if (File.Exists(downloadedFilePath))
                        {
                            try
                            {
                                File.Delete(downloadedFilePath);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"Error deleting file {downloadedFilePath}: {ex.Message}");
                            }
                        }
                    }

                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error in saving uploaded files: {ex.Message}");
                }

                void CreateTimeFrameFolder()
                {
                    string timeFrame = MyEnumConverter.TimeFrameFromEnum(viewModel.TimeFrame);
                    pathToSaveFiles = Path.Combine(pathToSaveFiles, timeFrame);
                    if (!Directory.Exists(pathToSaveFiles))
                    {
                        Directory.CreateDirectory(pathToSaveFiles);
                    }
                }


            }
            return screenshotsPaths;



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
