using Microsoft.AspNetCore.Mvc.Rendering;
using Models.ViewModels.DisplayClasses;
using Newtonsoft.Json;
using Shared;
using SharedEnums.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.ViewModels
{
    public class NewTradeVM : IResearchDisplayModel
    {
        public NewTradeVM()
        {
            YesNoOptions = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "No" },
                new SelectListItem { Value = "1", Text = "Yes" }
            };

            TradeRating = new List<SelectListItem>
            {
                new SelectListItem { Value = "0", Text = "A+"},
                new SelectListItem { Value = "1", Text = "A"},
                new SelectListItem { Value = "2", Text = "A-"},
                new SelectListItem { Value = "3", Text = "Book of Horror"}
            };


            ResearchData = new();
            ResearchFirstBarPullbackDisplay = new();
            TradeData = new();
            CurrentTrade = new();
        }

        #region Properties
        public EStatus Status { get; set; }
        public ETimeFrame TimeFrame { get; set; }

        public EStrategy Strategy { get; set; }

        public ETradeType TradeType { get; set; }

        public EDirection SideType { get; set; }

        public EOrderType OrderType { get; set; }

        public object ResearchData { get; set; }

        public ResearchFirstBarPullbackDisplay ResearchFirstBarPullbackDisplay { get; set; }

        public Trade CurrentTrade { get; set; }

        public TradeDisplay TradeData { get; set; }

        public ResearchCradle ResearchCradle { get; set; }

        public List<SelectListItem> YesNoOptions { get; set; }

        public List<SelectListItem> TradeRating { get; set; }

        #endregion

        #region Method

        /// <summary>
        ///  Sets the trade paramaters and the NewTradeVM.ResearchData proprerty.
        /// </summary>
        /// <param name="tradeParams"></param>
        /// <param name="researchData"></param>
        /// <returns></returns>
        public string SetValues(string tradeParams, string researchData, string tradeData)
        {
            string error = string.Empty;
            try
            {
                List<string> errors = new List<string>();
                Result<EStatus>? statusResult = null;
                Result<EOrderType>? orderTypeResult = null;

                Dictionary<string, string> tradeDataObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(tradeParams);

                Result<ETimeFrame> timeFrameResult = MyEnumConverter.TimeFrameFromString(tradeDataObject["timeFrame"]);
                Result<EStrategy> strategyResult = MyEnumConverter.StrategyFromString(tradeDataObject["strategy"]);
                Result<ETradeType> typeResult = MyEnumConverter.TradeTypeFromString(tradeDataObject["tradeType"]);
                Result<EDirection> sideResult = MyEnumConverter.SideTypeFromString(tradeDataObject["tradeSide"]);

                // For a research trade there is no need for Status or OrderType
                if (!typeResult.Success || typeResult.Success && typeResult.Value != ETradeType.Research)
                {
                    statusResult = MyEnumConverter.StatusFromString(tradeDataObject["status"]);
                    orderTypeResult = MyEnumConverter.OrderTypeFromString(tradeDataObject["orderType"]);

                    ValidateResult(statusResult, "Status");
                    ValidateResult(orderTypeResult, "OrderType");
                }

                ValidateResult(timeFrameResult, "Time frame");
                ValidateResult(strategyResult, "Strategy");
                ValidateResult(typeResult, "Type");
                ValidateResult(sideResult, "Side");

                if (errors.Any())
                {
                    return error = string.Join("<br>", errors);
                }

                // Menu buttons
                TimeFrame = timeFrameResult.Value;
                Strategy = strategyResult.Value;
                TradeType = typeResult.Value;
                SideType = sideResult.Value;

                // Status and OrderType are not required for Research
                if (TradeType != ETradeType.Research)
                {
                    Status = statusResult!.Value;
                    OrderType = orderTypeResult!.Value;
                }

                if (Strategy == EStrategy.FirstBarPullback)
                {
                    // ResearchData is of type object because it can contain research data for different strategies. See NewTradeController -> SaveTrade()
                    ResearchData = JsonConvert.DeserializeObject<ResearchFirstBarPullbackDisplay>(researchData);
                }
                else if (Strategy == EStrategy.Cradle)
                {
                    ResearchData = JsonConvert.DeserializeObject<ResearchCradle>(researchData);
                }
                TradeData = JsonConvert.DeserializeObject<TradeDisplay>(tradeData);

                // Helper method to avoid duplicating code
                void ValidateResult<T>(Result<T> result, string tradeParam)
                {
                    if (!result.Success)
                    {
                        errors.Add($"{tradeParam} not selected.");
                    }
                }
            }
            catch (Exception ex)
            {
                error = $"Error in NewTradeVM.SetValues(): {ex.Message}";
            }

            return error;
        }

        #endregion
    }
}
