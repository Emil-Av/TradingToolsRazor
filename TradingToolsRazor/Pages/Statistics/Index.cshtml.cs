using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Models.RequestModels;
using Statistics.Services;
using Statistics.Models;
using Statistics.Interfaces;

namespace TradingToolsRazor.Pages.Statistics
{
    public class IndexModel(IStatisticsService statisticsService) : PageModel
    {
        private readonly IStatisticsService _statisticsService = statisticsService;

        public StatisticsPageViewModel ViewModel { get; private set; } = new();

        public async Task<IActionResult> OnGetAsync([FromQuery] StatisticsQueryModel query)
        {
            ViewModel = await _statisticsService.BuildViewModelAsync(query);
            return Page();
        }
    }
}
