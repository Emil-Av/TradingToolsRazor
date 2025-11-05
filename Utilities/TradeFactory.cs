using DataAccess.Data;
using DataAccess.Repository.IRepository;
using Models;
using SharedEnums.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class TradeFactory
    {
        private readonly IUnitOfWork _unitOfWork;

        public TradeFactory(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public Trade CreateTrade(EStrategy strategy)
        {
            Journal journal = new();
            _unitOfWork.Journal.Add(journal);
            _unitOfWork.SaveAsync();

            dynamic research = null;
            if (strategy == EStrategy.FirstBarPullback)
            {
                research = new ResearchFirstBarPullback();
            }
            if (research == null)
            {
                _unitOfWork.ResearchFirstBarPullback.Add((ResearchFirstBarPullback)research);
                _unitOfWork.SaveAsync();
            }

            Trade trade = new Trade();
            _unitOfWork.Trade.Add(trade);
            _unitOfWork.SaveAsync();

            return trade;
        }
    }
}
