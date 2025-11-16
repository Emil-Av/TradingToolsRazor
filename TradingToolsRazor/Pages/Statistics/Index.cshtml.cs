using DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Statistics.Models;
using Statistics.Services;

namespace TradingToolsRazor.Pages.Statistics
{
    public class IndexModel : PageModel
    {
        private readonly IUnitOfWork _unitOfWork;

        public List<CandleBracketingStatisticItem>? CandleBracketingStats { get; set; }

        public IndexModel(IUnitOfWork unitOfWork)
        {
           _unitOfWork = unitOfWork;
        }
        public async Task<IActionResult> OnGetAsync()
        {
            var researchTrades = await _unitOfWork.ResearchCandleBracketing.GetAllAsync();
            CandleBracketingStats = CandleBracketingStatistics.GetAllStats(researchTrades);

            return Page();
        }
    }
}
