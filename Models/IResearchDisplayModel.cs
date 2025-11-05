using Microsoft.AspNetCore.Mvc.Rendering;
using Models.ViewModels;
using Models.ViewModels.DisplayClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public interface IResearchDisplayModel
    {
        ResearchFirstBarPullbackDisplay ResearchFirstBarPullbackDisplay { get; set; }

        TradeDisplay TradeData { get; set; }

        ResearchCradle ResearchCradle { get; set; }

    }
}
