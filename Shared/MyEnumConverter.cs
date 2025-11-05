using SharedEnums.Enums;

namespace Shared
{
    public class MyEnumConverter
    {
        public static string OrderTypeFromEnum(EOrderType orderType)
        {
            Dictionary<EOrderType, string> statusType = new Dictionary<EOrderType, string>()
            {
                { EOrderType.Market, "Market" },
                { EOrderType.Limit, "Limit" },
                { EOrderType.StopLoss, "Stop Loss" }
            };

            return statusType[orderType];
        }

        public static Result<EOrderType> OrderTypeFromString(string orderType)
        {
            Dictionary<string, EOrderType> orderTypes = new Dictionary<string, EOrderType>()
            {
                { "Market", EOrderType.Market },
                { "Limit", EOrderType.Limit },
                { "Stop Loss", EOrderType.StopLoss }
            };
            try
            {
                return Result<EOrderType>.SuccessResult(orderTypes[orderType]);
            }
            catch
            {
                return Result<EOrderType>.ErrorResult($"Error converting order type from a string. Value given: {orderType}");
            }
        }

        public static Result<EStatus> StatusFromString(string status)
        {
            Dictionary<string, EStatus> statusTypes = new Dictionary<string, EStatus>()
            {
                { "Pending", EStatus.Pending },
                { "Opened", EStatus.Opened },
                { "Closed", EStatus.Closed },
                { "All", EStatus.All }
            };

            try
            {
                return Result<EStatus>.SuccessResult(statusTypes[status]);
            }
            catch
            {
                return Result<EStatus>.ErrorResult($"Error converting status from a string. Value given: {status}");
            }
        }

        public static Result<ESideType> SideTypeFromString(string sideType)
        {
            Dictionary<string, ESideType> sideTypes = new Dictionary<string, ESideType>() 
            {
                { "Long", ESideType.Long },
                { "Short", ESideType.Short }
            };

            try
            {
                return Result<ESideType>.SuccessResult(sideTypes[sideType]);
            }
            catch
            {
                return Result<ESideType>.ErrorResult($"Error converting side type from a string. Value given: {sideType}");
            }
        }

        public static Result<ETradeType> TradeTypeFromString(string tradeType)
        {
            Dictionary<string, ETradeType> tradeTypes = new Dictionary<string, ETradeType>()
            {
                { "Trade", ETradeType.Trade },
                { "Paper Trade" , ETradeType.PaperTrade },
                { "Research", ETradeType.Research },
            };

            try
            {
                return Result<ETradeType>.SuccessResult(tradeTypes[tradeType]);
            }
            catch
            {
                return Result<ETradeType>.ErrorResult($"Error converting the trade type from a string. Value given: {tradeType}");
            }
        }

        public static string TradeTypeFromEnum(ETradeType tradeType)
        {
            Dictionary<ETradeType, string> tradeTypes = new Dictionary<ETradeType, string>()
            {
                { ETradeType.Trade , "Trade"},
                { ETradeType.PaperTrade, "Paper Trade" },
                { ETradeType.Research, "Research" },
            };

            return tradeTypes[tradeType];
        }

        public static Result<ETimeFrame> TimeFrameFromString(string timeFrame)
        {

            Dictionary<string, ETimeFrame> timeFrames = new Dictionary<string, ETimeFrame>()
            {
                { "5M", ETimeFrame.M5 },
                { "10M", ETimeFrame.M10 },
                { "15M", ETimeFrame.M15 },
                { "30M", ETimeFrame.M30 },
                { "1H", ETimeFrame.H1 },
                { "2H", ETimeFrame.H2 },
                { "4H", ETimeFrame.H4 },
                { "D", ETimeFrame.D }

            };

            try
            {
                return Result<ETimeFrame>.SuccessResult(timeFrames[timeFrame]);
            }
            catch
            {
                return Result<ETimeFrame>.ErrorResult($"Error converting the time frame from as string. Value given: {timeFrame}");
            }
        }

        public static string TimeFrameFromEnum(ETimeFrame timeFrame)
        {
            Dictionary<ETimeFrame, string> timeFrames = new Dictionary<ETimeFrame, string>()
            {
                { ETimeFrame.M5, "5M" },
                { ETimeFrame.M10 , "10M" },
                { ETimeFrame.M15 , "15M" },
                { ETimeFrame.M30 , "30M" },
                { ETimeFrame.H1 , "1H" },
                { ETimeFrame.H2 , "2H" },
                { ETimeFrame.H4 , "4H" },
                { ETimeFrame.D , "D" }

            };

            return timeFrames[timeFrame];
        }

        public static Result<EStrategy> StrategyFromString(string strategy)
        {
            Dictionary<string, EStrategy> strategies = new Dictionary<string, EStrategy>()
            {
                { "Cradle", EStrategy.Cradle },
                { "First Bar Pullback", EStrategy.FirstBarPullback }
            };

            try
            {
                return Result<EStrategy>.SuccessResult(strategies[strategy]);
            }
            catch
            {
                return Result<EStrategy>.ErrorResult($"Error converting the strategy from a string. Value given: {strategy}");
            }
        }

        public static string StrategyFromEnum(EStrategy? strategy)
        {
            Dictionary<EStrategy?, string> strategies = new Dictionary<EStrategy?, string>()
            {
                { EStrategy.Cradle, "Cradle" },
                { EStrategy.FirstBarPullback, "First Bar Pullback" }
            };

            return strategies[strategy];
        }
    }
}
