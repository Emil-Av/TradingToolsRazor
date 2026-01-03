using Models;
using Models.ViewModels;
using SharedEnums.Enums;

namespace TradingToolsRazor.Services.Interfaces
{
    public interface ITradesService
    {
        TradesVM InitializeNewTradeTradesViewModel();
        Task<TradesVM> InitializeTradesViewModelAsync();
        Task<TradesVM> LoadTradeAsync(LoadTradeParams tradeParams);
        Task UpdateTradeDataAsync(Trade tradeData);
        Task UpdateReviewAsync(TradesVM data);
        Task UpdateJournalAsync(TradesVM data);
    }
}
