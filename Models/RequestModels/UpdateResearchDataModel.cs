using SharedEnums.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RequestModels
{
    public class UpdateResearchDataModel
    {
        public string Data { get; set; } 
        public Strategy Strategy { get; set; }
    }
}
