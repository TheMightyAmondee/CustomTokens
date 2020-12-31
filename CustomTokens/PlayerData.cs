using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CustomTokens
{
    public class PlayerData
    {
        public int CurrentMineLevel { get; set; }
        public double CurrentYearsMarried { get; set; }
        public int AnniversaryDay { get; set; }
        public string AnniversarySeason { get; set; } = "No season";
        public int DeathCountAfterMarriage { get; set; }
    }
}
