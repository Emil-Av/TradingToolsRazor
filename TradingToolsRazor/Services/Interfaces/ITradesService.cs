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
        Task UpdateTradeDataAsync(BaseTrade tradeData);
        Task UpdateReviewAsync(Review review);
        Task UpdateJournalAsync(Journal journal);
        Task UpdateResearchData(UpdateResearchDataModel updateResearchData);
        Task DeleteTrade(DeleteTradeRequestModel deleteTradeRequest);
        Task<TradesVM> LoadSampleSizeNumberAsync(int sampleSizeId);
        Task<TradesVM> LoadTimeFrameAsync(TradesLoadRequestModel requestModel);
        Task<TradesVM> LoadStrategyAsync(Strategy strategy);
    }
}
