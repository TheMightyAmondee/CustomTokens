using System;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace CustomTokens
{
    public class MarriageTokens
    {

        internal void UpdateMarriageTokens(IMonitor monitor, PerScreen<PlayerData> data, ModConfig config)
        {
            // Get days married
            int DaysMarried = Game1.player.GetDaysMarried();
            float Years = DaysMarried / 112;
            // Get years married
            double YearsMarried = Math.Floor(Years);
            // Get Anniversary date
            var anniversary = SDate.Now().AddDays(-(DaysMarried - 1));          

            // Set tokens for the start of the day
            data.Value.CurrentYearsMarried = Game1.player.isMarried() == true ? YearsMarried : 0;

            data.Value.AnniversarySeason = Game1.player.isMarried() == true ? anniversary.Season : "No season";

            data.Value.AnniversaryDay = Game1.player.isMarried() == true ? anniversary.Day : 0;

            // Test if player is married
            if (Game1.player.isMarried() is false)
            {
                // No, relevant trackers will use their default values

                monitor.Log($"{Game1.player.Name} is not married");

                if (config.ResetDeathCountMarriedWhenDivorced == true && ModEntry.perScreen.Value.DeathCountMarried != 0)
                {
                    // Reset tracker if player is no longer married
                    ModEntry.perScreen.Value.DeathCountMarried = 0;
                }
            }

            // Yes, tokens exist
            else
            {
                monitor.Log($"{Game1.player.Name} has been married for {YearsMarried} year(s)");

                monitor.Log($"Anniversary is the {anniversary.Day} of {anniversary.Season}");
            }
        }
    }
}
