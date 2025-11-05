using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    /// <summary>
    ///  One review object per sample size of 20 trades.
    /// </summary>
    public class Review
    {
        public int Id { get; set; }
        public string? First { get; set; }
        public string? Second { get; set; }
        public string? Third { get; set; }
        public string? Forth { get; set; }
        public string? Summary { get; set; }

    }
}
