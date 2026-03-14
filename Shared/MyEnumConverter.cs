using SharedEnums.Enums;

namespace Shared
{
    public class MyEnumConverter
    {
        public static string OrderTypeFromEnum(EOrderType orderType)
        {
            Dictionary<EOrderType, string> statusType = new()
            {
                { EOrderType.Market, "Market" },
                { EOrderType.Limit, "Limit" },
                { EOrderType.StopLoss, "Stop Loss" }
            };

            return statusType[orderType];
        }

        public static Result<EOrderType> OrderTypeFromString(string orderType)
        {
            Dictionary<string, EOrderType> orderTypes = new()
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
            Dictionary<string, EStatus> statusTypes = new()
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

        public static Result<EDirection> SideTypeFromString(string sideType)
        {
            Dictionary<string, EDirection> sideTypes = new() 
            {
                { "Long", EDirection.Long },
                { "Short", EDirection.Short }
            };

            try
            {
                return Result<EDirection>.SuccessResult(sideTypes[sideType]);
            }
            catch
            {
                return Result<EDirection>.ErrorResult($"Error converting side type from a string. Value given: {sideType}");
            }
        }

        public static Result<SampleSizeType> TradeTypeFromString(string tradeType)
        {
            Dictionary<string, SampleSizeType> tradeTypes = new()
            {
                { "Trade", SampleSizeType.Trade },
                { "Research", SampleSizeType.Research },
                { "Paper Trade", SampleSizeType.PaperTrade },
                { "Demo Trading", SampleSizeType.DemoTrading }
            };

            try
            {
                return Result<SampleSizeType>.SuccessResult(tradeTypes[tradeType]);
            }
            catch
            {
                return Result<SampleSizeType>.ErrorResult($"Error converting the trade type from a string. Value given: {tradeType}");
            }
        }

        public static string TradeTypeFromEnum(SampleSizeType tradeType)
        {
            Dictionary<SampleSizeType, string> tradeTypes = new()
            {
                { SampleSizeType.Trade , "Trade"},
                { SampleSizeType.Research, "Research" },
                { SampleSizeType.PaperTrade, "Paper Trade" },
                { SampleSizeType.DemoTrading, "Demo Trading" }
            };

            return tradeTypes[tradeType];
        }

        public static Result<TimeFrame> TimeFrameFromString(string timeFrame)
        {

            Dictionary<string, TimeFrame> timeFrames = new()
            {
                { "5M", TimeFrame.M5 },
                { "10M", TimeFrame.M10 },
                { "15M", TimeFrame.M15 },
                { "30M", TimeFrame.M30 },
                { "1H", TimeFrame.H1 },
                { "2H", TimeFrame.H2 },
                { "4H", TimeFrame.H4 },
                { "D", TimeFrame.D }

            };

            try
            {
                return Result<TimeFrame>.SuccessResult(timeFrames[timeFrame]);
            }
            catch
            {
                return Result<TimeFrame>.ErrorResult($"Error converting the time frame from as string. Value given: {timeFrame}");
            }
        }

        public static string TimeFrameFromEnum(TimeFrame timeFrame)
        {
            Dictionary<TimeFrame, string> timeFrames = new()
            {
                { TimeFrame.M5, "5M" },
                { TimeFrame.M10 , "10M" },
                { TimeFrame.M15 , "15M" },
                { TimeFrame.M30 , "30M" },
                { TimeFrame.H1 , "1H" },
                { TimeFrame.H2 , "2H" },
                { TimeFrame.H4 , "4H" },
                { TimeFrame.D , "D" }

            };

            return timeFrames[timeFrame];
        }

        public static Result<Strategy> StrategyFromString(string strategy)
        {
            Dictionary<string, Strategy> strategies = new()
            {
                { "Cradle", Strategy.Cradle },
                { "First Bar Pullback", Strategy.FirstBarPullback },
                { "Candle Bracketing", Strategy.CandleBracketing },
                { "SRS", Strategy.SRS },
                { "BrunchBreak", Strategy.BrunchBreak }
            };

            try
            {
                return Result<Strategy>.SuccessResult(strategies[strategy]);
            }
            catch
            {
                return Result<Strategy>.ErrorResult($"Error converting the strategy from a string. Value given: {strategy}");
            }
        }

        public static string StrategyFromEnum(Strategy strategy)
        {
            Dictionary<Strategy, string> strategies = new()
            {
                { Strategy.Cradle, "Cradle" },
                { Strategy.FirstBarPullback, "First Bar Pullback" },
                { Strategy.CandleBracketing, "Candle Bracketing" },
                { Strategy.SRS, "SRS" },
                { Strategy.BrunchBreak, "BrunchBreak" }
            };

            return strategies[strategy];
        }
    }
}
