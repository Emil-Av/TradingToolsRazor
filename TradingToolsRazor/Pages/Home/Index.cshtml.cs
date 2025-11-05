using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models;
using SharedEnums.Enums;
using Utilities;

namespace TradingTools.Pages.Home
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _db;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public IndexModel(IUnitOfWork unitOfWork, ApplicationDbContext db, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _db = db;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: /Home/Index
        public async Task<IActionResult> OnGetAsync()
        {
            await CheckOrCreateUserSettingsAsync();
            return Page(); // equivalent to 'return View();'
        }

        // POST (or GET) for creating database backup
        public async Task<IActionResult> OnGetCreateBackupAsync()
        {
            string screenshotsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Screenshots");
            string zipFile = await DatabaseBackupHelper.CreateBackupZipFile(_db, screenshotsFolder);
            FileStream zipStream = null;

            try
            {
                zipStream = new FileStream(zipFile, FileMode.Open, FileAccess.Read, FileShare.Read);

                // Delete the backup file after response completes
                HttpContext.Response.OnCompleted(() =>
                {
                    System.IO.File.Delete(zipFile);
                    return Task.CompletedTask;
                });

                return File(zipStream, "application/zip", Path.GetFileName(zipFile));
            }
            catch (Exception ex)
            {
                return new JsonResult(new
                {
                    error = $"Error in {GetType().Name}.{nameof(OnGetCreateBackupAsync)}: {ex.Message}\r\n{ex.StackTrace}"
                });
            }
        }

        /// <summary>
        /// Checks if user settings exist; creates defaults if not.
        /// </summary>
        private async Task CheckOrCreateUserSettingsAsync()
        {
            int userSettingsCount = (await _unitOfWork.UserSettings.GetAllAsync()).Count;
            if (userSettingsCount == 0)
            {
                _unitOfWork.UserSettings.Add(new UserSettings
                {
                    PTTimeFrame = ETimeFrame.M10,
                    PTStrategy = EStrategy.FirstBarPullback
                });

                await _unitOfWork.SaveAsync();
            }
        }
    }
}
