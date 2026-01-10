using SharedEnums.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.RequestModels
{
    public class DeleteTradeRequestModel
    {
        public int Id { get; set; }

        public EStrategy Strategy { get; set; }
    }
}
