using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;

namespace TradingToolsRazor.Pages
{
    [AllowAnonymous]
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    [IgnoreAntiforgeryToken]
    public class ErrorModel : PageModel
    {
        public string? RequestId { get; set; }
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        public int StatusCode { get; set; }
        public string? ErrorMessage { get; set; }
        public string? ExceptionMessage { get; set; }
        public string? StackTrace { get; set; }

        private readonly ILogger<ErrorModel> _logger;

        public ErrorModel(ILogger<ErrorModel> logger)
        {
            _logger = logger;
        }

        public void OnGet(int? statusCode = null)
        {
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            StatusCode = statusCode ?? HttpContext.Response.StatusCode;

            // In development, try to get exception details from features
            var exceptionFeature = HttpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>();
            if (exceptionFeature != null)
            {
                ExceptionMessage = exceptionFeature.Error.Message;
                StackTrace = exceptionFeature.Error.StackTrace;
            }

            if (StatusCode == 400)
            {
                _logger.LogWarning("Bad Request (400) occurred. Request ID: {RequestId}, Path: {Path}",
                    RequestId, HttpContext.Request.Path);
            }
            else if (StatusCode == 404)
            {
                _logger.LogWarning("Not Found (404) occurred. Request ID: {RequestId}, Path: {Path}",
                    RequestId, HttpContext.Request.Path);
            }
            else
            {
                _logger.LogError("Error {StatusCode} occurred. Request ID: {RequestId}, Path: {Path}",
                    StatusCode, RequestId, HttpContext.Request.Path);
            }
        }
    }
}
