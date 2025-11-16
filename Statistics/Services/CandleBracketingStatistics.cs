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

namespace BackTestingStatistics.Services
{
    /// <summary>
    ///  Generates statistics for candle bracketing trades. Candle bracketing means putting buy/sell orders above/below a candle at specific time.
    /// </summary>
    public class CandleBracketingStatistics
    {

        #region Totals
        public static int TotalTrades(List<CandleBracketingModel> trades)
        {
            return trades.Count;
        }

        public static int TotalBreakevensWithFTS(List<CandleBracketingModel> trades)
        {
            return trades.Count(trade => trade.IsBreakeven && trade.FlippedTheSwitch);
        }

        public static int TotalBreakevensWithoutFTS(List<CandleBracketingModel> trades)
        {
            return trades.Count(trade => trade.IsBreakeven && !trade.FlippedTheSwitch);
        }

        public static int TotalBreakevens(List<CandleBracketingModel> trades)
        {
            return trades.Count(trade => trade.IsBreakeven);
        }

        public static int TotalLosses(List<CandleBracketingModel> trades)
        {
            return trades.Count(trade => trade.IsLoss);
        }
        public static int TotalWins(List<CandleBracketingModel> trades)
        {
            return trades.Count(trade => trade.IsWin);
        }
        public static int TotalTradesWithFTS(List<CandleBracketingModel> trades)
        {
            return trades.Count(trade => trade.FlippedTheSwitch);
        }
        public static int TotalTradesWithoutFTS(List<CandleBracketingModel> trades)
        {
            return trades.Count(trade => !trade.FlippedTheSwitch);
        }

        public static int TotalWinsWithoutFTS(List<CandleBracketingModel> trades)
        {
            return trades.Count(trade => trade.IsWin && !trade.FlippedTheSwitch);
        }

        public static int TotalWinsWithFTS(List<CandleBracketingModel> trades)
        {
            return trades.Count(trade => trade.IsWin && trade.FlippedTheSwitch);
        }

        public static int TotalLossesWithFTS(List<CandleBracketingModel> trades)
        {
            return trades.Count(trade => trade.IsLoss && trade.FlippedTheSwitch);
        }

        public static int TotalLossesWithoutFTS(List<CandleBracketingModel> trades)
        {
            return trades.Count(trade => trade.IsLoss && !trade.FlippedTheSwitch);
        }

        public static int TotalWeekendBreakevens(List<CandleBracketingModel> trades)
        {
            return trades.Where(trade => trade.IsWeekend && trade.IsBreakeven).Count();
        }

        public static int TotalWeekendLosses(List<CandleBracketingModel> trades)
        {
            return trades.Where(trade => trade.IsWeekend && trade.IsLoss).Count();
        }

        public static int TotalWeekendWins(List<CandleBracketingModel> trades)
        {
            return trades.Where(trade => trade.IsWeekend && trade.IsWin).Count();
        }

        public static int TotalWeekendTrades(List<CandleBracketingModel> trades)
        {
            return trades.Count(trade => trade.IsWeekend);
        }

        #endregion

        #region Averages

        public static double AveragePercentageWin(List<CandleBracketingModel> trades)
        {
            var winners = trades.Where(trade => trade.IsWin).ToList();

            winners.ForEach(winner =>
            {
                if (winner.MaxPrice <= 0)
                {
                    throw new Exception($"Invalid MaxPrice for a winner: {winner.Date}");
                }
            });

            var percentMoves = winners.Select(trade =>
            {
                double move = trade.Direction == EDirection.Long
                                                                ? (trade.MaxPrice - trade.EntryPrice) / trade.EntryPrice * 100
                                                                : (trade.EntryPrice - trade.MaxPrice) / trade.EntryPrice * 100;

                return move;

            });

            return Math.Round(percentMoves.Average(), 2);
        }

        public static double AveragePercentageLoss(List<CandleBracketingModel> trades)
        {
            var losers = trades.Where(trade => trade.IsLoss).ToList();

            losers.ForEach(loser =>
            {
                if (loser.ExitPrice <= 0)
                {
                    throw new Exception($"Invalid ExitPrice for a loser: {loser.Date}");
                }
            });

            var percentMoves = losers.Select(trade =>
            {
                double move = trade.Direction == EDirection.Long
                                                                ? (trade.EntryPrice - trade.ExitPrice) / trade.EntryPrice * 100
                                                                : (trade.ExitPrice - trade.EntryPrice) / trade.EntryPrice * 100;

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

        public static double RiskToRewardRatio(List<CandleBracketingModel> trades)
        {
            double averageWin = AveragePercentageWin(trades);
            double averageLoss = AveragePercentageLoss(trades);

            return Math.Round(averageWin / averageLoss, 2);
        }

        public static double ProfitFactor(List<CandleBracketingModel> trades)
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

        public static double ExpectancyPercent(List<CandleBracketingModel> trades)
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

        public static double MedianFavorableMoveAtrMultiple(List<CandleBracketingModel> trades)
        {
            // Get ATR multiples for losing trades
            var atrMultiples = trades
                .Where(trade => trade.IsLoss && trade.MaxPrice > 0 && trade.ATR > 0)
                .Select(trade =>
                {
                    double favorableMove = 0;

                    if (trade.Direction == EDirection.Long)
                        favorableMove = trade.MaxPrice - trade.EntryPrice;
                    else if (trade.Direction == EDirection.Short)
                        favorableMove = trade.EntryPrice - trade.MaxPrice;

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

        public static void AnalyzeFavorableMovesBeforeStop(List<CandleBracketingModel> trades) // Validated
        {
            var losingMoves = trades
                                .Where(trade => trade.IsLoss && trade.MaxPrice > 0)
                                .Select(trade =>
                                {
                                    double movePercent = 0;

                                    if (trade.Direction == EDirection.Long)
                                        movePercent = (trade.MaxPrice - trade.EntryPrice) / trade.EntryPrice * 100;
                                    else if (trade.Direction == EDirection.Short)
                                        movePercent = (trade.EntryPrice - trade.MaxPrice) / trade.EntryPrice * 100;

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
