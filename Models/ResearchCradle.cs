using Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models
{
    public class ResearchCradle : BaseTrade
    {
        // Those are the properties that will be saved into the DB. This is the model that will be turned into an entity
        public int TestCradleProp { get; set; }
    }
}
