using Microsoft.AspNetCore.Http;
using Models.ViewModels;

namespace TradingToolsRazor.Services.Interfaces
{
    public interface INewTradeService
    {
        Task SaveTradeAsync(NewTradeVM viewModel, IFormFile[] files);
    }
}
