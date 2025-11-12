using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Formats.Asn1;

namespace Utilities
{
    public class DeleteTradeHelper
    {
        private readonly IUnitOfWork _unitOfWork;

        public DeleteTradeHelper(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task CheckAndUpdateScreenshotPathsAfterDeletion(string screenshotPath, List<BaseTrade> tradesInSampleSize, string webRootPath)
        {
            DeleteTradeDirectory(screenshotPath, webRootPath);
            int tradeNumber = ParseTradeNumber(screenshotPath);
            bool isNotLastTrade = tradeNumber < tradesInSampleSize.Count + 1;
            if (tradeNumber == -1 && isNotLastTrade)
                return;

            for (int i = tradeNumber - 1; i < tradesInSampleSize.Count; i++)
            {
                var trade = tradesInSampleSize[i];
                if (trade.ScreenshotsUrls == null)
                    continue;

                List<string> updatedScreenshotUrls = new List<string>();
                foreach (string oldUrl in trade.ScreenshotsUrls)
                {
                    string newUrl = ReplaceTradeNumberInUrl(oldUrl, i + 1);
                    string oldFilePath = GetAbsolutePath(oldUrl, webRootPath);
                    string newFilePath = GetAbsolutePath(newUrl, webRootPath);
                    string oldDir = Path.GetDirectoryName(oldFilePath)!;
                    string newDir = Path.GetDirectoryName(newFilePath)!;

                    EnsureDirectoryExists(newDir);
                    MoveFileIfExists(oldFilePath, newFilePath);
                    DeleteDirectoryIfEmpty(oldDir);

                    updatedScreenshotUrls.Add(newUrl);
                }
                trade.ScreenshotsUrls = updatedScreenshotUrls;
                await UpdateEntityAsync(trade);
            }
            await _unitOfWork.SaveAsync();
        }

        private async Task UpdateEntityAsync(BaseTrade trade)
        {
            switch (trade)
            {
                case ResearchCandleBracketing candleBracketing:
                    await _unitOfWork.ResearchCandleBracketing.UpdateAsync(candleBracketing);
                    break;
                case ResearchCradle researchCradle:
                    await _unitOfWork.ResearchCradle.UpdateAsync(researchCradle);
                    break;
                // Add other cases for different BaseTrade derived types if needed
                default:
                    throw new InvalidOperationException("Unsupported trade type");
            }
        }

        private void DeleteTradeDirectory(string screenshotPath, string webRootPath)
        {
            string directoryToDelete = Path.GetDirectoryName(Path.Combine(webRootPath, screenshotPath)!)!;
            if (Directory.Exists(directoryToDelete))
                Directory.Delete(directoryToDelete, true);
        }

        private int ParseTradeNumber(string screenshotPath)
        {
            var match = Regex.Match(screenshotPath, @"Trade (\d+)");
            return match.Success && int.TryParse(match.Groups[1].Value, out int number) ? number : -1;
        }

        private string ReplaceTradeNumberInUrl(string url, int newTradeNumber)
        {
            return Regex.Replace(url, @"Trade (\d+)", $"Trade {newTradeNumber}");
        }

        private string GetAbsolutePath(string relativeUrl, string webRootPath)
        {
            string wwwRootPath = webRootPath;
            string relativePath = relativeUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
            return Path.Combine(wwwRootPath, relativePath);
        }

        private void EnsureDirectoryExists(string? directory)
        {
            if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }
        }

        private void MoveFileIfExists(string oldFilePath, string newFilePath)
        {
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Move(oldFilePath, newFilePath, overwrite: true);
            }
        }

        private void DeleteDirectoryIfEmpty(string directory)
        {
            if (Directory.Exists(directory) && !Directory.EnumerateFileSystemEntries(directory).Any())
            {
                Directory.Delete(directory);
            }
        }
    }
}
