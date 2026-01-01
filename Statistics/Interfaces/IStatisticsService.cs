using Models.RequestModels;
using Statistics.Models;

namespace Statistics.Interfaces
{
    public interface IStatisticsService
    {
        Task<StatisticsPageViewModel> BuildViewModelAsync(StatisticsQueryModel query);
    }
}
