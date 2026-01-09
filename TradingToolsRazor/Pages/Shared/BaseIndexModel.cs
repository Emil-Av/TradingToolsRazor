using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TradingToolsRazor.Pages.Shared
{
    public class BaseIndexModel : PageModel
    {
        public JsonResult? ValidateModel()
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState
                    .Where(x => x.Value!.Errors.Any())
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value!.Errors.Select(e => e.ErrorMessage).ToArray()
                    );
                return new JsonResult(new { error = "Invalid model state: " + string.Join(", ", errors) });
            }

            return null;
        }
    }
}
