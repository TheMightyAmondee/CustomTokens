using System.Collections;

namespace CustomTokens
{
    public class PlayerData
    {
        public int CurrentMineLevel { get; set; }
        public int CurrentVolcanoFloor { get; set; }
        public double CurrentYearsMarried { get; set; }
        public int AnniversaryDay { get; set; }
        public string AnniversarySeason { get; set; } = "No season";

        public ArrayList SpecialOrdersCompleted = new ArrayList();

        public ArrayList QuestsCompleted = new ArrayList();
    }
}
