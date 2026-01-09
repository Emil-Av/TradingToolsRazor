using Models;
using Models.RequestModels;
using Models.ViewModels;
using SharedEnums.Enums;

namespace TradingToolsRazor.Services.Interfaces
{
    public interface ITradesService
    {
        TradesVM InitializeNewTradeTradesViewModel();
        Task<TradesVM> InitializeTradesViewModelAsync();
        Task<TradesVM> LoadTradeAsync(LoadTradeParams tradeParams);
        Task UpdateTradeDataAsync(BaseTrade tradeData);
        Task UpdateReviewAsync(TradesVM data);
        Task UpdateJournalAsync(Journal journal);
        Task UpdateResearchData(UpdateResearchDataModel updateResearchData);
    }
}
