using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;


namespace CustomTokens
{
    public interface IContentPatcherAPI
    {
        void RegisterToken(IManifest mod, string name, Func<IEnumerable<string>> getValue);
    }

    public class ModEntry 
        : Mod
    {
        public bool update = false;
        public static PlayerData PlayerData { get; private set; } = new PlayerData();

        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += this.LocationChange;
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            helper.Events.GameLoop.UpdateTicked += this.UpdateTicked;
            helper.Events.GameLoop.Saved += this.Saved;
            helper.Events.GameLoop.DayStarted += this.DayStarted;
            helper.ConsoleCommands.Add("tracker", "Displays the current tracked values", this.TellMe);
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Access Content Patcher API
            var api = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");

            // Register "MineLevel" token
            api.RegisterToken(
                this.ModManifest, 
                "MineLevel", 
                () =>
                {
                    if (Context.IsWorldReady)
                    {
                        var currentMineLevel = PlayerData.CurrentMineLevel;

                        return new[] 
                        { 
                            currentMineLevel.ToString() 
                        };
                    }
                    
                    return null;
                });

            // Register "AnniversaryDay" token
            api.RegisterToken(
                this.ModManifest,
                "AnniversaryDay",
                () =>
                {
                    if (Context.IsWorldReady)
                    {
                        var AnniversaryDay = PlayerData.AnniversaryDay;

                        return new[]
                        {
                            AnniversaryDay.ToString()
                        };
                    }

                    return null;
                });
            // Register "AnniversarySeason" token
            api.RegisterToken(
                this.ModManifest,
                "AnniversarySeason",
                () =>
                {
                    if (Context.IsWorldReady)
                    {
                        var AnniversarySeason = PlayerData.AnniversarySeason;

                        return new[]
                        {
                            AnniversarySeason
                        };
                    }

                    return null;
                });
            // Register "YearsMarried" token
            api.RegisterToken(
               this.ModManifest,
               "YearsMarried",
               () =>
               {
                   if (Context.IsWorldReady)
                   {
                       var currentYearsMarried = PlayerData.CurrentYearsMarried;

                       return new[]
                       {
                            currentYearsMarried.ToString()
                       };
                   }

                   return null;
               });

            // Register "DeathCount" token
            api.RegisterToken(
               this.ModManifest,
               "DeathCount",
               () =>
               {
                   if (Context.IsWorldReady)
                   {
                       var currentdeathcount = (int)Game1.stats.timesUnconscious;

                       return new[]
                       {
                            currentdeathcount.ToString()
                       };
                   }

                   return null;
               });

            // Register "DeathCountMarried" token
            api.RegisterToken(
               this.ModManifest,
               "DeathCountMarried",
               () =>
               {
                   if (Context.IsWorldReady)
                   {
                       /* 
                       CP won't load content after token is updated, 
                       Adding 1 to the value if married ensures token value is correct when content is loaded
                       To ensure CP will update the token, ensure an Update field of OnLocationChange or OnTimeChange or both
                       is included with the patch using the token
                       */
                       var currentdeathcountmarried = Game1.player.isMarried() is true 
                       ? PlayerData.DeathCountAfterMarriage + 1 
                       : 0;

                       return new[]
                       {
                            currentdeathcountmarried.ToString()
                       };
                   }

                   return null;
               });
        }

        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            // Get days married
            int DaysMarried = Game1.player.GetDaysMarried();
            float Years = DaysMarried / 112;
            // Get years married
            double YearsMarried = Math.Floor(Years);
            // Get Anniversary date
            var anniversary = SDate.Now().AddDays(-(DaysMarried - 1));

           
            // Test if player is married
            if (Game1.player.isMarried() is false)
            {
                this.Monitor.Log($"{Game1.player.Name} is not married");

                // Reset tracker if player is no longer married
                PlayerData.DeathCountAfterMarriage = 0;
            }

            // Player is married, tokens can exist
            else
            {
                PlayerData.CurrentYearsMarried = YearsMarried;

                PlayerData.AnniversarySeason = anniversary.Season;

                PlayerData.AnniversaryDay = anniversary.Day;

                this.Monitor.Log($"{Game1.player.Name} has been married for {YearsMarried} year(s)");

                this.Monitor.Log($"Anniversary is the {anniversary.Day} of {anniversary.Season}");

            }

            // Fix death tracker
            if (PlayerData.DeathCountAfterMarriageOld < PlayerData.DeathCountAfterMarriage)
            {
                this.Monitor.Log("Fixing tracker to discard unsaved data");
                PlayerData.DeathCountAfterMarriage = PlayerData.DeathCountAfterMarriageOld;
            }
        }
        private void LocationChange(object sender, WarpedEventArgs e)
        {
            // Get current location as a MineShaft
            var mineShaft = Game1.currentLocation as MineShaft;
           
            // Test to see if current location is a MineShaft
            if (!(mineShaft is null))
            {
                // Yes, update tracker

                this.Monitor.Log($"{Game1.player.Name} is on level {mineShaft.mineLevel}.");

                PlayerData.CurrentMineLevel = mineShaft.mineLevel;
            }

            else
            {
                // No, does the tracker reflect this?
                if(PlayerData.CurrentMineLevel > 0)
                {
                    // No, reset mine level tracker
                    PlayerData.CurrentMineLevel = 0;
                    this.Monitor.Log($"Minelevel tracker reset");
                }
            }
        }

        private void TellMe(string command, string[] args)
        {
            try 
            {
                // Display information in SMAPI console
                this.Monitor.Log($"\n\nCurrentMineLevel: {PlayerData.CurrentMineLevel}" +
                    $"\nCurrentYearsMarried: {PlayerData.CurrentYearsMarried}" +
                    $"\nAnniversaryDay: {PlayerData.AnniversaryDay}" +
                    $"\nAnniversarySeason: {PlayerData.AnniversarySeason}" +
                    $"\nDeathCount:{Game1.stats.timesUnconscious}" +
                    $"\nDeathCountAfterMarriage: {PlayerData.DeathCountAfterMarriage}", LogLevel.Debug);
            }
            catch
            {
                // Display an error if command has failed to execute
                this.Monitor.Log("Command failed", LogLevel.Error);
            }
            
        }

        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // Update tracker if player died, is married and tracker should update
            if(Game1.killScreen == true && Game1.player.isMarried() == true && update == true)
            {

                PlayerData = Helper.Data.ReadJsonFile<PlayerData>($"data\\{Constants.SaveFolderName}.json") ?? new PlayerData();

                // Increment tracker
                PlayerData.DeathCountAfterMarriage++;

                // Already updated, ensures tracker won't repeatedly increment
                update = false;

                this.Monitor.Log($"{Game1.player.Name} has died {PlayerData.DeathCountAfterMarriage} time(s) since last marriage.");
            }

            else if(Game1.killScreen == false && update == false)
            {
                // Tracker should be updated next death
                update = true;
            }

            // Save any data to JSON recorded that day, ensures data is discard if day was not saved
            this.Helper.Data.WriteJsonFile<PlayerData>($"data\\{Constants.SaveFolderName}.json", ModEntry.PlayerData);
        }

        private void Saved(object sender, SavedEventArgs e)
        {
            // Update old tracker
            PlayerData.DeathCountAfterMarriageOld = PlayerData.DeathCountAfterMarriage;
            this.Monitor.Log("Old death tracker updated for new day");
            // Save any data to JSON recorded that day, ensures data is discard if day was not saved
            this.Helper.Data.WriteJsonFile<PlayerData>($"data\\{Constants.SaveFolderName}.json", ModEntry.PlayerData);
        }
    }
}


