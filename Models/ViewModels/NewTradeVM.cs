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

        public object ResearchData { get; set; }

        public ResearchFirstBarPullbackDisplay ResearchFirstBarPullbackDisplay { get; set; }

        public BaseTrade CurrentTrade { get; set; }

        public TradeDisplay TradeData { get; set; }

        public ResearchCradle ResearchCradle { get; set; }

        public List<SelectListItem> YesNoOptions { get; set; }

        public List<SelectListItem> TradeRating { get; set; }

        public SRS SRSTrade { get; set; }

        public BrunchBreak BrunchBreakTrade { get; set; }
        public SampleSizeViewData SampleSizeViewData { get; set; }

        public TradesVM TradesVM { get; set; }

        #endregion

        #region Method

        public string ParseViewData(string viewData, string sampleSizeViewData)
        {
            try
            {
                SampleSizeViewData = JsonConvert.DeserializeObject<SampleSizeViewData>(sampleSizeViewData)!;
                
                switch (SampleSizeViewData.Strategy)
                {
                    case EStrategy.SRS:
                        SRSTrade = JsonConvert.DeserializeObject<SRS>(viewData)!;
                        break;
                    case EStrategy.BrunchBreak:
                        BrunchBreakTrade = JsonConvert.DeserializeObject<BrunchBreak>(viewData)!;
                        break;
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
        public string SetValues(string researchData, string tradeData)
        {
            try
            {
                SetResearchData(researchData);
                TradeData = JsonConvert.DeserializeObject<TradeDisplay>(tradeData)!;

                return string.Empty;
            }
            catch (Exception ex)
            {
                return $"Error in NewTradeVM.SetValues(): {ex.Message}";
            }
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

        #endregion
    }
}
