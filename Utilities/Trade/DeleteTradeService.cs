using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Models;
using SharedEnums.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Asn1;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Utilities.Trade
{
    public class DeleteTradeService(IUnitOfWork unitOfWork)
    {
        private readonly IUnitOfWork _unitOfWork = unitOfWork;

        public async Task DeleteTrade(Strategy strategy, int id, string webRootPath)
        {
            BaseTrade removedTrade = await DeleteTrade(strategy, id);

            await DeleteJournal(removedTrade.JournalId);

            var tradesInSampleSize = await _unitOfWork.BaseTrade.GetAllAsync(trade => trade.SampleSizeId == removedTrade.SampleSizeId);
            if (!tradesInSampleSize.Any())
            {
                int reviewId = await DeleteSampleSize(removedTrade.SampleSizeId);
                await DeleteReview(reviewId);
            }

            await UpdateScreenshotPathsAfterDeletion(removedTrade.ScreenshotsUrls!.First(), tradesInSampleSize, webRootPath);
        }

        private async Task DeleteJournal(int? journalId)
        {
            Journal journal = await _unitOfWork.Journal.GetAsync(journal => journal.Id == journalId);
            _unitOfWork.Journal.Remove(journal);
            await _unitOfWork.SaveAsync();
        }

        private async Task DeleteReview(int reviewId)
        {
            Review review = await _unitOfWork.Review.GetAsync(review => review.Id == reviewId);
            _unitOfWork.Review.Remove(review);
            await _unitOfWork.SaveAsync();
        }

        private async Task<BaseTrade> DeleteTrade(Strategy strategy, int id)
        {
            return strategy switch
            {
                Strategy.SRS => await RemoveSRSTrade(id),
                Strategy.BrunchBreak => await RemoveBrunchBreakTrade(id),
                _ => new()
            };
        }

        private async Task<BrunchBreak> RemoveBrunchBreakTrade(int id)
        {
            BrunchBreak? trade = await _unitOfWork.BrunchBreak.GetAsync(trade => trade.Id == id) ?? throw new ArgumentException($"Failed to find trade with id {id}");
            _unitOfWork.BrunchBreak.Remove(trade);
            await _unitOfWork.SaveAsync();

            return trade;
        }

        private async Task<SRS> RemoveSRSTrade(int Id)
        {
            SRS? trade = await _unitOfWork.SRS.GetAsync(trade => trade.Id == Id) ?? throw new ArgumentException($"Failed to find trade with id {Id}");

            _unitOfWork.SRS.Remove(trade);
            await _unitOfWork.SaveAsync();

            return trade;
        }

        private async Task<int> DeleteSampleSize(int sampleSizeId)
        {
            SampleSize sampleSize = await _unitOfWork.SampleSize.GetAsync(sampleSize => sampleSize.Id == sampleSizeId);
            _unitOfWork.SampleSize.Remove(sampleSize);
            await _unitOfWork.SaveAsync();

            return (int)sampleSize.ReviewId!;
        }

        public async Task UpdateScreenshotPathsAfterDeletion(string screenshotPath, List<BaseTrade> tradesInSampleSize, string webRootPath)
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

                List<string> updatedScreenshotUrls = [];
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
                await _unitOfWork.BaseTrade.UpdateAsync(trade);
            }
            await _unitOfWork.SaveAsync();
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
            if (File.Exists(oldFilePath))
            {
                File.Move(oldFilePath, newFilePath, overwrite: true);
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
