using Models;
using SharedEnums.Enums;
using Statistics.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Statistics.Services
{
    /// <summary>
    ///  Generates statistics for candle bracketing trades. Candle bracketing means putting buy/sell orders above/below a candle at specific time.
    /// </summary>
    public class CandleBracketingStatistics
    {
        public static List<CandleBracketingStatisticItem> GetAllStats(List<ResearchCandleBracketing> trades)
        {
            var statisticItems = new List<CandleBracketingStatisticItem>
            {
                new CandleBracketingStatisticItem(nameof(TotalTrades), "Total trades:", TotalTrades(trades), 1),
                new CandleBracketingStatisticItem(nameof(TotalWins), "Total Wins:", TotalWins(trades), 1),
                new CandleBracketingStatisticItem(nameof(TotalLosses), "Total losses:", TotalLosses(trades), 1),
                new CandleBracketingStatisticItem(nameof(TotalBreakevensTrades), "Total breakeven trades:", TotalBreakevensTrades(trades), 1),
                new CandleBracketingStatisticItem(nameof(TotalUniqueDates), "Total unique dates in the sample size:", TotalUniqueDates(trades), 1),

                new CandleBracketingStatisticItem(nameof(WinsWithoutFTS), "Wins without FTS:", WinsWithoutFTS(trades), 2),
                new CandleBracketingStatisticItem(nameof(FTSWins), "FTS wins:", FTSWins(trades), 2),

                new CandleBracketingStatisticItem(nameof(LossesWithoutFTS), "Losses without FTS:", LossesWithoutFTS(trades), 3),
                new CandleBracketingStatisticItem(nameof(FTSLosses), "FTS losses:", FTSLosses(trades), 3),

                new CandleBracketingStatisticItem(nameof(TradesWithoutFTS), "Trades without FTS:", TradesWithoutFTS(trades), 4),
                new CandleBracketingStatisticItem(nameof(TotalFTSTrades), "Total FTS trades:", TotalFTSTrades(trades), 4),

                new CandleBracketingStatisticItem(nameof(BreakevensWithoutFTS), "No FTS breakeven trades:", BreakevensWithoutFTS(trades), 5),
                new CandleBracketingStatisticItem(nameof(BreakevensFTSTrades), "Breakeven FTS trades:", BreakevensFTSTrades(trades), 5),

                new CandleBracketingStatisticItem(nameof(TotalWeekendTrades), "Total trades on the weekend:", TotalWeekendTrades(trades), 6),
                new CandleBracketingStatisticItem(nameof(WeekendWins), "Wins on the weekend:", WeekendWins(trades), 6),
                new CandleBracketingStatisticItem(nameof(WeekendLosses), "Losses on the weekend:", WeekendLosses(trades), 6),
                new CandleBracketingStatisticItem(nameof(WeekendBreakevens), "Breakeven trades on the weekend:", WeekendBreakevens(trades), 6),

                new CandleBracketingStatisticItem(nameof(AveragePercentageWin), "Average percentage win:", AveragePercentageWin(trades), 7),
                new CandleBracketingStatisticItem(nameof(AveragePercentageLoss), "Average percentage loss:", AveragePercentageLoss(trades), 7),
                new CandleBracketingStatisticItem(nameof(RiskToRewardRatio), "R:R ratio:", AveragePercentageLoss(trades), 7),
                new CandleBracketingStatisticItem(nameof(ProfitFactor), "Profit factor:", ProfitFactor(trades), 7),
                new CandleBracketingStatisticItem(nameof(ExpectancyPercent), "Expectancy percent:", ExpectancyPercent(trades), 7),
                new CandleBracketingStatisticItem(nameof(MedianFavorableMoveAtrMultiple), "Median favorable move. ATR multiple:", MedianFavorableMoveAtrMultiple(trades), 7),
            };

            return statisticItems;
        }

        private static int TotalUniqueDates(List<ResearchCandleBracketing> trades)
        {
            return trades.Select(trade => trade.Date).Distinct().Count();
        }

        #region Totals
        public static int TotalTrades(List<ResearchCandleBracketing> trades)
        {
            return trades.Count;
        }

        public static int BreakevensFTSTrades(List<ResearchCandleBracketing> trades)
        {
            return trades.Count(trade => trade.IsBreakeven && trade.IsFlippedTheSwitch);
        }

        public static int BreakevensWithoutFTS(List<ResearchCandleBracketing> trades)
        {
            return trades.Count(trade => trade.IsBreakeven && !trade.IsFlippedTheSwitch);
        }

        public static int TotalBreakevensTrades(List<ResearchCandleBracketing> trades)
        {
            return trades.Count(trade => trade.IsBreakeven);
        }

        public static int TotalLosses(List<ResearchCandleBracketing> trades)
        {
            return trades.Count(trade => trade.IsLoss);
        }
        public static int TotalWins(List<ResearchCandleBracketing> trades)
        {
            return trades.Count(trade => trade.IsWin);
        }
        public static int TotalFTSTrades(List<ResearchCandleBracketing> trades)
        {
            return trades.Count(trade => trade.IsFlippedTheSwitch);
        }
        public static int TradesWithoutFTS(List<ResearchCandleBracketing> trades)
        {
            return trades.Count(trade => !trade.IsFlippedTheSwitch);
        }

        public static int WinsWithoutFTS(List<ResearchCandleBracketing> trades)
        {
            return trades.Count(trade => trade.IsWin && !trade.IsFlippedTheSwitch);
        }

        public static int FTSWins(List<ResearchCandleBracketing> trades)
        {
            return trades.Count(trade => trade.IsWin && trade.IsFlippedTheSwitch);
        }

        public static int FTSLosses(List<ResearchCandleBracketing> trades)
        {
            return trades.Count(trade => trade.IsLoss && trade.IsFlippedTheSwitch);
        }

        public static int LossesWithoutFTS(List<ResearchCandleBracketing> trades)
        {
            return trades.Count(trade => trade.IsLoss && !trade.IsFlippedTheSwitch);
        }

        public static int WeekendBreakevens(List<ResearchCandleBracketing> trades)
        {
            return trades.Where(trade => trade.IsWeekend && trade.IsBreakeven).Count();
        }

        public static int WeekendLosses(List<ResearchCandleBracketing> trades)
        {
            return trades.Where(trade => trade.IsWeekend && trade.IsLoss).Count();
        }

        public static int WeekendWins(List<ResearchCandleBracketing> trades)
        {
            return trades.Where(trade => trade.IsWeekend && trade.IsWin).Count();
        }

        public static int TotalWeekendTrades(List<ResearchCandleBracketing> trades)
        {
            return trades.Count(trade => trade.IsWeekend);
        }

        #endregion

        #region Averages

        public static double AveragePercentageWin(List<ResearchCandleBracketing> trades)
        {
            var winners = trades.Where(trade => trade.IsWin).ToList();

            if (!winners.Any())
                return 0;

            winners.ForEach(winner =>
            {
                if (winner.MaxPrice <= 0)
                {
                    throw new Exception($"Invalid MaxPrice for a winner: {winner.Date}");
                }
            });

            var percentMoves = winners.Select(trade =>
            {
                double move = (double)(trade.Direction == EDirection.Long
                                                               ? (trade.MaxPrice - trade.EntryPriceForResearch) / trade.EntryPriceForResearch * 100
                                                               : (trade.EntryPriceForResearch - trade.MaxPrice) / trade.EntryPriceForResearch * 100)!;
                return move;
            });

            return Math.Round(percentMoves.Average(), 2);
        }

        public static double AveragePercentageLoss(List<ResearchCandleBracketing> trades)
        {
            var losers = trades.Where(trade => trade.IsLoss).ToList();

            losers.ForEach(loser =>
            {
                if (loser.ExitPriceForResearch <= 0)
                {
                    throw new Exception($"Invalid ExitPriceForResearch for a loser: {loser.Date}");
                }
            });

            var percentMoves = losers.Select(trade =>
            {
                double move = (double)(trade.Direction == EDirection.Long
                                                                ? (trade.EntryPriceForResearch - trade.ExitPriceForResearch) / trade.EntryPriceForResearch * 100
                                                                : (trade.ExitPriceForResearch - trade.EntryPriceForResearch) / trade.EntryPriceForResearch * 100)!;

                return move;
            }).ToList();

            percentMoves.ForEach(move =>
            {
                if (move <= 0)
                {
                    int index = percentMoves.IndexOf(move);
                    throw new Exception($"Calculated percentage move for a losing trade is not positive. Trade from {losers[index].Date} at index {index}.");
                }
            });

            return Math.Round(percentMoves.Average(), 2);
        }

        #endregion

        #region Others

        public static double RiskToRewardRatio(List<ResearchCandleBracketing> trades)
        {
            double averageWin = AveragePercentageWin(trades);
            double averageLoss = AveragePercentageLoss(trades);

            return Math.Round(averageWin / averageLoss, 2);
        }

        public static double ProfitFactor(List<ResearchCandleBracketing> trades)
        {
            double averageWin = AveragePercentageWin(trades);
            double averageLoss = AveragePercentageLoss(trades);

            int totalWins = trades.Count(trade => trade.IsWin);
            int totalLosses = trades.Count(trade => trade.IsLoss);

            if (totalWins == 0 || totalLosses == 0)
                return 0;

            double totalProfit = averageWin * totalWins;
            double totalLoss = averageLoss * totalLosses;

            return Math.Round(totalProfit / totalLoss, 2);
        }

        public static double ExpectancyPercent(List<ResearchCandleBracketing> trades)
        {
            double averageWin = AveragePercentageWin(trades);
            double averageLoss = AveragePercentageLoss(trades);

            int totalWins = trades.Count(trade => trade.IsWin);
            int totalLosses = trades.Count(trade => trade.IsLoss);

            int totalTrades = totalWins + totalLosses;
            if (totalTrades == 0)
                return 0;

            double winRate = (double)totalWins / totalTrades;
            double lossRate = (double)totalLosses / totalTrades;

            double expectancy = (winRate * averageWin) - (lossRate * averageLoss);

            return Math.Round(expectancy, 2);
        }

        public static double MedianFavorableMoveAtrMultiple(List<ResearchCandleBracketing> trades)
        {
            // Get ATR multiples for losing trades
            var atrMultiples = trades
                .Where(trade => trade.IsLoss && trade.MaxPrice > 0 && trade.ATR > 0)
                .Select(trade =>
                {
                    double favorableMove = 0;

                    if (trade.Direction == EDirection.Long)
                        favorableMove = (double)(trade.MaxPrice - trade.EntryPriceForResearch)!;
                    else if (trade.Direction == EDirection.Short)
                        favorableMove = (double)(trade.EntryPriceForResearch - trade.MaxPrice)!;

                    return favorableMove / trade.ATR;
                })
                .Where(multiple => multiple > 0)
                .OrderBy(multiple => multiple) // sort ascending
                .ToList();

            if (!atrMultiples.Any())
                return 0;

            int count = atrMultiples.Count;
            // Median calculation
            if (count % 2 == 1)
            {
                return atrMultiples[count / 2]; // middle value
            }
            else
            {
                return (atrMultiples[(count / 2) - 1] + atrMultiples[count / 2]) / 2.0;
            }
        }

        public static void AnalyzeFavorableMovesBeforeStop(List<ResearchCandleBracketing> trades) // Validated
        {
            var losingMoves = trades
                                .Where(trade => trade.IsLoss && trade.MaxPrice > 0)
                                .Select(trade =>
                                {
                                    double movePercent = 0;

                                    if (trade.Direction == EDirection.Long)
                                        movePercent = (double)((trade.MaxPrice - trade.EntryPriceForResearch) / trade.EntryPriceForResearch * 100)!;
                                    else if (trade.Direction == EDirection.Short)
                                        movePercent = (double)((trade.EntryPriceForResearch - trade.MaxPrice) / trade.EntryPriceForResearch * 100)!;

                                    return new
                                    {
                                        trade.Date,
                                        Direction = trade.Direction.ToString(),
                                        MovePercent = movePercent
                                    };
                                })
                                .Where(result => result.MovePercent > 0)
                                .OrderBy(result => result.MovePercent)
                                .ToList();

            if (!losingMoves.Any())
            {
                Console.WriteLine("No valid losing trades found for analysis.");
                return;
            }

            double mean = losingMoves.Average(result => result.MovePercent);
            var belowMean = losingMoves.Where(result => result.MovePercent < mean).ToList();
            double averageBelowMean = belowMean.Any() ? belowMean.Average(result => result.MovePercent) : 0;

            Console.WriteLine("----------------------------------");
            Console.WriteLine("Lowest 15 favorable moves before stop :");
            var sortedMoves = losingMoves.OrderBy(result => result.MovePercent).Take(15).ToList();
            foreach (var result in sortedMoves)
            {
                Console.WriteLine($"Date: {result.Date}, Direction: {result.Direction},Move: {result.MovePercent:F4}%");
            }
            Console.WriteLine("----------------------------------");
            Console.WriteLine($"Mean favorable move before stop: {mean:F4}%");
            Console.WriteLine($"Average of moves below mean: {averageBelowMean:F4}%");
            Console.WriteLine($"Count below mean: {belowMean.Count}");
            Console.WriteLine("----------------------------------");
        }


        #endregion
    }
}
