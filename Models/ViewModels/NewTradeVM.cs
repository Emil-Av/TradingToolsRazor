using Microsoft.AspNetCore.Mvc.Rendering;
using Models.ViewModels.DisplayClasses;
using Newtonsoft.Json;
using Shared;
using SharedEnums.Enums;

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

        /// <summary>
        /// Obsolete. Should use SampleSizeViewData instead.
        /// </summary>
        public ETimeFrame TimeFrame { get; set; }

        /// <summary>
        /// Obsolete. Should use SampleSizeViewData instead.
        /// </summary>
        public EStrategy Strategy { get; set; }

        /// <summary>
        /// Obsolete. Should use SampleSizeViewData instead.
        /// </summary>
        public ETradeType TradeType { get; set; }

        /// <summary>
        /// Obsolete. Should use SampleSizeViewData instead.
        /// </summary>
        public EOrderType OrderType { get; set; }

        public object ResearchData { get; set; }

        public ResearchFirstBarPullbackDisplay ResearchFirstBarPullbackDisplay { get; set; }

        public Trade CurrentTrade { get; set; }

        public TradeDisplay TradeData { get; set; }

        public ResearchCradle ResearchCradle { get; set; }

        public List<SelectListItem> YesNoOptions { get; set; }

        public List<SelectListItem> TradeRating { get; set; }

        public SRS SRSTrade { get; set; }

        public SampleSizeViewData SampleSizeViewData { get; set; }

        #endregion

        #region Method

        public string ParseViewData(string viewData, string sampleSizeViewData)
        {

            try
            {
                SampleSizeViewData = JsonConvert.DeserializeObject<SampleSizeViewData>(sampleSizeViewData)!;
                
                if (SampleSizeViewData.Strategy == EStrategy.SRS)
                {
                    SRSTrade = JsonConvert.DeserializeObject<SRS>(viewData)!;
                }
            }
            catch (Exception ex)
            {
                return $"Error in NewTradeVM.ParseViewData(): {ex.Message}";
            }


            return string.Empty;
        }

        /// <summary>
        ///  Sets the trade paramaters and the NewTradeVM.ResearchData proprerty.
        /// </summary>
        /// <param name="tradeParams"></param>
        /// <param name="researchData"></param>
        /// <param name="tradeData"></param>
        /// <returns></returns>
        public string SetValues(string tradeParams, string researchData, string tradeData)
        {
            try
            {
                var validationResult = ValidateAndSetTradeParameters(tradeParams);
                if (!string.IsNullOrEmpty(validationResult))
                {
                    return validationResult;
                }

                SetResearchData(researchData);
                TradeData = JsonConvert.DeserializeObject<TradeDisplay>(tradeData)!;

                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Error in NewTradeVM.SetValues(): {ex.Message}";
            }
        }

        private string ValidateAndSetTradeParameters(string tradeParams)
        {
            var errors = new List<string>();
            var tradeDataObject = JsonConvert.DeserializeObject<Dictionary<string, string>>(tradeParams)!;

            var timeFrameResult = MyEnumConverter.TimeFrameFromString(tradeDataObject["timeFrame"]);
            var strategyResult = MyEnumConverter.StrategyFromString(tradeDataObject["strategy"]);
            var typeResult = MyEnumConverter.TradeTypeFromString(tradeDataObject["tradeType"]);

            ValidateResult(timeFrameResult, "Time frame", errors);
            ValidateResult(strategyResult, "Strategy", errors);
            ValidateResult(typeResult, "Type", errors);

            Result<EStatus>? statusResult = null;
            Result<EOrderType>? orderTypeResult = null;

            // Status and OrderType are only required for non-Research trades
            if (typeResult.Success && typeResult.Value != ETradeType.Research)
            {
                statusResult = MyEnumConverter.StatusFromString(tradeDataObject["status"]);
                orderTypeResult = MyEnumConverter.OrderTypeFromString(tradeDataObject["orderType"]);

                ValidateResult(statusResult, "Status", errors);
                ValidateResult(orderTypeResult, "OrderType", errors);
            }

            if (errors.Any())
            {
                return string.Join("<br>", errors);
            }

            TimeFrame = timeFrameResult.Value;
            Strategy = strategyResult.Value;
            TradeType = typeResult.Value;

            if (TradeType != ETradeType.Research)
            {
                Status = statusResult!.Value;
                OrderType = orderTypeResult!.Value;
            }

            return string.Empty;
        }

        private void SetResearchData(string researchData)
        {
            ResearchData = Strategy switch
            {
                EStrategy.FirstBarPullback => JsonConvert.DeserializeObject<ResearchFirstBarPullbackDisplay>(researchData)!,
                EStrategy.Cradle => JsonConvert.DeserializeObject<ResearchCradle>(researchData)!,
                EStrategy.CandleBracketing => JsonConvert.DeserializeObject<ResearchCandleBracketing>(researchData)!,
                EStrategy.SRS => JsonConvert.DeserializeObject<SRS>(researchData)!,
                _ => throw new ArgumentException($"Unknown strategy: {Strategy}")
            };
        }

        private static void ValidateResult<T>(Result<T> result, string paramName, List<string> errors)
        {
            if (!result.Success)
            {
                errors.Add($"{paramName} not selected.");
            }
        }

        #endregion
    }
}
