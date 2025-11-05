using DataAccess.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Threading;

namespace Utilities
{
    public class DatabaseBackupHelper
    {
        #region Fields

        private static string backupFolder = @"C:\Backups";

        private static string dbBackupFilePath = Path.Combine(backupFolder, "dbBackup.bak");

        private static string zipFilePath = Path.Combine(backupFolder, DateTime.Now.ToShortDateString() + "_Backup.zip");

        #endregion

        #region Methods
        public static async Task<string> CreateBackupZipFile(ApplicationDbContext db, string screenshotsSourceFolder)
        {
            await CreateDBBackup(db);
            await CreateZipFile(zipFilePath, dbBackupFilePath, screenshotsSourceFolder);

            return zipFilePath;
        }

        private static async Task CreateZipFile(string zipFilePath, string singleFilePath, string folderPath)
        {
            using (FileStream zipStream = new FileStream(zipFilePath, FileMode.Create, FileAccess.Write))
            {
                using (ZipArchive zipArchive = new ZipArchive(zipStream, ZipArchiveMode.Create, leaveOpen: true))
                {
                    await AddDbBackupFileToZip(zipArchive);
                    await AddScreenshotsFolderToZip(zipArchive, folderPath, Path.GetFileName(folderPath));
                }
            }
        }

        private static async Task AddDbBackupFileToZip(ZipArchive zipArchive)
        {
            if (!File.Exists(dbBackupFilePath))
            {
                throw new FileNotFoundException($"Data base backup file not found in {dbBackupFilePath}");
            }

            string entryName = Path.GetFileName(dbBackupFilePath);
            ZipArchiveEntry zipEntry = zipArchive.CreateEntry(entryName);
            using (FileStream fileStream = new FileStream(dbBackupFilePath, FileMode.Open, FileAccess.Read))
            {
                using (Stream entryStream = zipEntry.Open())
                {
                    await fileStream.CopyToAsync(entryStream);
                }
            }

            File.Delete(dbBackupFilePath);
        }

        private static async Task AddScreenshotsFolderToZip(ZipArchive zipArchive, string sourceFolderPath, string entryName)
        {
            var directoryInfo = new DirectoryInfo(sourceFolderPath);

            // Add files in the current directory
            foreach (var file in directoryInfo.GetFiles())
            {
                var entry = zipArchive.CreateEntry($"{entryName}/{file.Name}");
                using (var fileStream = new FileStream(file.FullName, FileMode.Open, FileAccess.Read))
                using (var entryStream = entry.Open())
                {
                    await fileStream.CopyToAsync(entryStream);
                }
            }

            // Recursively add subdirectories
            foreach (var subDirectory in directoryInfo.GetDirectories())
            {
                await AddScreenshotsFolderToZip(zipArchive, subDirectory.FullName, $"{entryName}/{subDirectory.Name}");
            }
        }

        public static void CreateScreenshotsBackup(string sourceFolder, string destinationFolder)
        {
            if (!Directory.Exists(sourceFolder))
            {
                throw new DirectoryNotFoundException($"Source folder does not exist: {sourceFolder}");
            }

            Directory.CreateDirectory(destinationFolder);

            foreach (string file in Directory.GetFiles(sourceFolder))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(destinationFolder, fileName);
                File.Copy(file, destFile, overwrite: true);
            }

            foreach (string subFolder in Directory.GetDirectories(sourceFolder))
            {
                string folderName = Path.GetFileName(subFolder);
                string destSubFolder = Path.Combine(destinationFolder, folderName);
                CreateScreenshotsBackup(subFolder, destSubFolder);
            }
        }

        public static async Task CreateDBBackup(ApplicationDbContext db)
        {
            var databaseName = db.Database.GetDbConnection().Database;
            var backupSql = $@" BACKUP DATABASE [{databaseName}] TO DISK = '{dbBackupFilePath}' WITH FORMAT, INIT;";
            await db.Database.ExecuteSqlRawAsync(backupSql);
        }

        #endregion
    }
}
