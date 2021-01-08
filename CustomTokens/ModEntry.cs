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
        private ModConfig config;

        public bool updatedeath = false;

        public bool updatepassout = false;

        public static PlayerData PlayerData { get; private set; } = new PlayerData();

        public static PlayerDataToWrite PlayerDataToWrite { get; private set; } = new PlayerDataToWrite();

        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += this.LocationChange;
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            helper.Events.GameLoop.UpdateTicked += this.UpdateTicked;
            helper.Events.GameLoop.Saved += this.Saved;
            helper.Events.GameLoop.DayStarted += this.DayStarted;

            // Read the mod config for values and create one if one does not currently exist
            this.config = this.Helper.ReadConfig<ModConfig>();

            if (this.config.AllowDebugging == true)
            {
                helper.ConsoleCommands.Add("tracker", "Displays the current tracked values", this.TellMe);
            }
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
                       CP won't load content after token is updated during the PlayerKilled event, 
                       Adding 1 to the value if married ensures token value is correct when content is loaded for event
                       To ensure CP will update the token, ensure an Update field of OnLocationChange or OnTimeChange or both
                       is included with the patch using the token
                       */
                       var currentdeathcountmarried = Game1.player.isMarried() 
                       ? PlayerDataToWrite.DeathCountMarried
                       : 0;

                       return new[]
                       {
                            currentdeathcountmarried.ToString()
                       };

                   }

                   return null;
               });

            // Register "DeathCountPK" token
            api.RegisterToken(
               this.ModManifest,
               "DeathCountPK",
               () =>
               {
                   /* 
                   CP won't use correct value of DeathCountMarried token  during the PlayerKilled event as the token is updated outside of the useable update rates, 
                   Adding 1 to the value of that token ensures token value is correct when content is loaded for event
                   */

                   if (Context.IsWorldReady)
                   {
                       var currentdeathcount = (int)Game1.stats.timesUnconscious + 1;

                       return new[]
                       {
                            currentdeathcount.ToString()
                       };
                   }

                   return null;
               });

            // Register "DeathCountMarriedPK" token
            api.RegisterToken(
               this.ModManifest,
               "DeathCountMarriedPK",
               () =>
               {
                   if (Context.IsWorldReady)
                   {
                       /* 
                       CP won't use correct value of DeathCountMarried token  during the PlayerKilled event as the token is updated outside of the useable update rates, 
                       Adding 1 to the value of that token ensures token value is correct when content is loaded for event
                       */
                       var currentdeathcountmarried = Game1.player.isMarried()
                       ? PlayerDataToWrite.DeathCountMarried + 1
                       : 0;

                       return new[]
                       {
                            currentdeathcountmarried.ToString()
                       };

                   }

                   return null;
               });

            // Register "PassOutCount" token
            api.RegisterToken(
               this.ModManifest,
               "PassOutCount",
               () =>
               {
                   if (Context.IsWorldReady)
                   {
                       var currentpassoutcount = PlayerDataToWrite.PassOutCount;

                       return new[]
                       {
                            currentpassoutcount.ToString()
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

            updatedeath = true;
            updatepassout = true;

            this.Monitor.Log($"Trackers set to update");

            // Read JSON file and create if needed
            PlayerDataToWrite = Helper.Data.ReadJsonFile<PlayerDataToWrite>($"data\\{Constants.SaveFolderName}.json") ?? new PlayerDataToWrite();

            // Set tokens
            PlayerData.CurrentYearsMarried = Game1.player.isMarried() == true ? YearsMarried : 0;

            PlayerData.AnniversarySeason = Game1.player.isMarried() == true ? anniversary.Season : "No season";

            PlayerData.AnniversaryDay = Game1.player.isMarried() == true ? anniversary.Day : 0;


            // Test if player is married
            if (Game1.player.isMarried() is false)
            {
                this.Monitor.Log($"{Game1.player.Name} is not married");

                if (this.config.ResetDeathCountMarriedWhenDivorced == true && PlayerDataToWrite.DeathCountMarried != 0)
                {
                    // Reset tracker if player is no longer married
                    PlayerDataToWrite.DeathCountMarried = 0;
                }
            }

            // Player is married, tokens exist
            else
            {
                this.Monitor.Log($"{Game1.player.Name} has been married for {YearsMarried} year(s)");

                this.Monitor.Log($"Anniversary is the {anniversary.Day} of {anniversary.Season}");
            }

            // Fix death tracker
            if (PlayerDataToWrite.DeathCountMarriedOld < PlayerDataToWrite.DeathCountMarried)
            {
                this.Monitor.Log("Fixing tracker to discard unsaved data");
                PlayerDataToWrite.DeathCountMarried = PlayerDataToWrite.DeathCountMarriedOld;
            }

            // Save any data to JSON file
            this.Helper.Data.WriteJsonFile<PlayerDataToWrite>($"data\\{Constants.SaveFolderName}.json", ModEntry.PlayerDataToWrite);
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
                    $"\nDeathCount: {Game1.stats.timesUnconscious}" +
                    $"\nDeathCountMarried: {PlayerDataToWrite.DeathCountMarried}" +
                    $"\nDeathCountPK: {Game1.stats.timesUnconscious + 1}" +
                    $"\nDeathCountMarriedPK: {PlayerDataToWrite.DeathCountMarried + 1}" +
                    $"\nPassOutCount: {PlayerDataToWrite.PassOutCount}", LogLevel.Debug);
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
            if(Game1.killScreen == true && Game1.player.isMarried() == true && updatedeath == true)
            {
                // Increment tracker
                PlayerDataToWrite.DeathCountMarried++;

                // Already updated, ensures tracker won't repeatedly increment
                updatedeath = false;

                if(this.config.ResetDeathCountMarriedWhenDivorced == true)
                {
                    this.Monitor.Log($"{Game1.player.Name} has died {PlayerDataToWrite.DeathCountMarried} time(s) since last marriage.");
                }
                else
                {
                    this.Monitor.Log($"{Game1.player.Name} has died {PlayerDataToWrite.DeathCountMarried} time(s) whilst married.");
                }               

                // Save any data to JSON file
                this.Helper.Data.WriteJsonFile<PlayerDataToWrite>($"data\\{Constants.SaveFolderName}.json", ModEntry.PlayerDataToWrite);
            }

            else if(Game1.killScreen == false && updatedeath == false)
            {
                // Tracker should be updated next death
                updatedeath = true;
            }

            // Has player passed out?
            else if(updatepassout == true && (Game1.timeOfDay == 2600 || Game1.player.stamina <= -15))
            {
                // Yes, update tracker

                // Increment tracker
                PlayerDataToWrite.PassOutCount++;
                // Already updated, ensures tracker won't repeatedly increment
                updatepassout = false;

                if(PlayerDataToWrite.PassOutCount > 20)
                {
                    this.Monitor.Log($"{Game1.player.Name} has passed out {PlayerDataToWrite.PassOutCount} time(s). Maybe you should go to bed earlier.");
                }
                else
                {
                    this.Monitor.Log($"{Game1.player.Name} has passed out {PlayerDataToWrite.PassOutCount} time(s).");
                }

            }

            else if(Game1.timeOfDay == 2610 && updatepassout == false)
            {
                // Decrement tracker, player can stay up later
                PlayerDataToWrite.PassOutCount--;
                // Already updated, ensures tracker won't repeatedly decrement
                updatepassout = true;
                this.Monitor.Log($"Nevermind, {Game1.player.Name} has actually passed out {PlayerDataToWrite.PassOutCount} time(s). Aren't you getting tired?");
            }
        }

        private void Saved(object sender, SavedEventArgs e)
        {
            // Update old tracker
            PlayerDataToWrite.DeathCountMarriedOld = PlayerDataToWrite.DeathCountMarried;
            this.Monitor.Log("Old death tracker updated for new day");
            // Save any data to JSON file
            this.Helper.Data.WriteJsonFile<PlayerDataToWrite>($"data\\{Constants.SaveFolderName}.json", ModEntry.PlayerDataToWrite);
        }
    }
}


