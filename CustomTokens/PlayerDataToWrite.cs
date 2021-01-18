using System.Collections;

namespace CustomTokens
{
    public class PlayerDataToWrite
    {
        public int DeathCountMarried { get; set; }
        public int DeathCountMarriedOld { get; set; }
        public int PassOutCount { get; set; }

        public ArrayList QuestsCompleted = new ArrayList();
    }
}
