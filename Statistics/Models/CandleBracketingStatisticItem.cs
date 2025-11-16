using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Statistics.Models
{
    public record CandleBracketingStatisticItem(string Key, string Description, object? Value, int Category);
}
