using Microsoft.AspNetCore.Mvc.Rendering;
using Models.ViewModels.DisplayClasses;

namespace Models.ViewModels
{
    public class PartialViewsVM
    {
        public ResearchFirstBarPullbackDisplay ResearchFirstBarPullbackDisplay { get; set; }
        public ResearchCradle ResearchCradle { get; set; }

        public ResearchCandleBracketing CandleBracketing { get; set; }
        public TradesVM TradesVM { get; set; }
        public List<SelectListItem> YesNoOptions { get; set; }
        public List<SelectListItem> OrderType { get; set; }
        public List<SelectListItem> TradeRating { get; set; }
        public List<SelectListItem> Outcome { get; set; }

        public PartialViewsVM()
        {
            ResearchFirstBarPullbackDisplay = new();
            TradesVM = new();
            ResearchCradle = new();
            CandleBracketing = new();
            SetListItems();
        }

        private void SetListItems()
        {
            Outcome = new List<SelectListItem>
            {
                new() {Value = "0", Text = "Loss"},
                new() {Value = "1", Text = "Win"}
            };

            TradeRating = new List<SelectListItem>
            {
                new() { Value = "0", Text = "A+"},
                new() { Value = "1", Text = "A"},
                new() { Value = "2", Text = "A-" },
                new() { Value = "3", Text = "Book of Horror" }
            };

            YesNoOptions = new List<SelectListItem>
            {
                new() {Value = "0", Text = "No"},
                new() {Value = "1", Text = "Yes"}
            };



            OrderType = new List<SelectListItem>
            {
                new() { Value = "0", Text = "Market"},
                new() { Value = "1", Text = "Limit"},
            };
        }
    }
}
