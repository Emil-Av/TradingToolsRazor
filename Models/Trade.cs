using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class Trade : BaseTrade
    {
        public int? ResearchId { get; set; }

        [ForeignKey("ResearchId")]
        public ResearchFirstBarPullback? Research { get; set; }
    }
}
