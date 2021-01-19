using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using System.Text;
using StardewValley.Quests;
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
        // Basic api
        void RegisterToken(IManifest mod, string name, Func<IEnumerable<string>> getValue);

        // Advanced api, not currently used
        void RegisterToken(IManifest mod, string name, object token);
    }

    public class ModEntry 
        : Mod
    {
        private ModConfig config;

        public bool updatedeath = false;

        public bool updatepassout = false;

        public static PlayerData PlayerData { get; private set; } = new PlayerData();

        public static PlayerDataToWrite PlayerDataToWrite { get; private set; } = new PlayerDataToWrite();

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Add required event helpers
            helper.Events.Player.Warped += this.LocationChange;
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            helper.Events.GameLoop.UpdateTicked += this.UpdateTicked;
            helper.Events.GameLoop.Saved += this.Saved;
            helper.Events.GameLoop.ReturnedToTitle += this.Title;
            helper.Events.GameLoop.DayStarted += this.DayStarted;

            // Read the mod config for values and create one if one does not currently exist
            this.config = this.Helper.ReadConfig<ModConfig>();

            // Add debug command if AllowDebugging is true
            if (this.config.AllowDebugging == true)
            {
                helper.ConsoleCommands.Add("tracker", "Displays the current tracked values", this.Tracker);
            }
        }

        /// <summary>Raised after the game is launched, right before the first update tick.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
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

            // Register "VolcanoFloor" token
            api.RegisterToken(
                this.ModManifest,
                "VolcanoFloor",
                () =>
                {
                    if (Context.IsWorldReady)
                    {
                        var currentVolcanoFloor = PlayerData.CurrentVolcanoFloor;

                        return new[]
                        {
                            currentVolcanoFloor.ToString()
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
                   CP won't use correct value of DeathCountMarried token during the PlayerKilled event as the token is updated outside of the useable update rates, 
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
                       CP won't use correct value of DeathCountMarried token during the PlayerKilled event as the token is updated outside of the useable update rates, 
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

            // Register "TotalQuestsCompleted" token
            api.RegisterToken(
               this.ModManifest,
               "QuestsCompleted",
               () =>
               {
                   if (Context.IsWorldReady)
                   {
                       var currentquestsdone = Game1.stats.questsCompleted;

                       return new[]
                       {
                            currentquestsdone.ToString()
                       };

                   }

                   return null;
               });

            // Register "QuestIDsCompleted" token
            api.RegisterToken(
               this.ModManifest,
               "QuestIDsCompleted",
               () =>
               {
                   if (Context.IsWorldReady)
                   {
                       /*
                       Will only add completed quests for quests completed after mod is added or updated
                       as the mod can't determine specifically which quests have been completed in the past
                       No flags or trackable changes occur to determine past quest completion so past 
                       completed quests must be entered in the JSON manually
                       */

                       // Create array with the length of the QuestsCompleted array list
                       string[] questsdone = new string[PlayerDataToWrite.QuestsCompleted.Count];

                       // Set each value in new array to be the same as in QuestCompleted
                       foreach(var quest in PlayerDataToWrite.QuestsCompleted)
                       {
                           questsdone.SetValue(quest, PlayerDataToWrite.QuestsCompleted.IndexOf(quest));
                       }

                       return questsdone;

                   }

                   return null;
               });

            // Register "SONamesCompleted" token
            api.RegisterToken(
               this.ModManifest,
               "SONamesCompleted",
               () =>
               {
                   if (Context.IsWorldReady)
                   {
                       
                       // Create array with the length of the SpecialOrdersCompleted array list
                       string[] ordersdone = new string[PlayerData.SpecialOrdersCompleted.Count];

                       // Set each value in new array to be the same as in SpecialOrdersCompleted
                       foreach (var quest in PlayerData.SpecialOrdersCompleted)
                       {
                           ordersdone.SetValue(quest, PlayerData.SpecialOrdersCompleted.IndexOf(quest));
                       }

                       return ordersdone;

                   }

                   return null;
               });

            // Register "SOCompleted" token
            api.RegisterToken(
               this.ModManifest,
               "SOCompleted",
               () =>
               {
                   if (Context.IsWorldReady)
                   {
                       var totalspecialorderscompleted = PlayerData.SpecialOrdersCompleted.Count;

                       return new[]
                       {
                           totalspecialorderscompleted.ToString()
                       };

                   }

                   return null;
               });

        }
        /// <summary>The method called after a new day starts.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
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

            // Set tokens for the start of the day
            PlayerData.CurrentYearsMarried = Game1.player.isMarried() == true ? YearsMarried : 0;

            PlayerData.AnniversarySeason = Game1.player.isMarried() == true ? anniversary.Season : "No season";

            PlayerData.AnniversaryDay = Game1.player.isMarried() == true ? anniversary.Day : 0;

            // Test if player is married
            if (Game1.player.isMarried() is false)
            {
                // No, relevant trackers will use their default values

                this.Monitor.Log($"{Game1.player.Name} is not married");

                if (this.config.ResetDeathCountMarriedWhenDivorced == true && PlayerDataToWrite.DeathCountMarried != 0)
                {
                    // Reset tracker if player is no longer married
                    PlayerDataToWrite.DeathCountMarried = 0;
                }
            }

            // Yes, tokens exist
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
            this.Monitor.Log("Writing data to JSON file");
            this.Helper.Data.WriteJsonFile<PlayerDataToWrite>($"data\\{Constants.SaveFolderName}.json", ModEntry.PlayerDataToWrite);
        }
        /// <summary>Raised after the current player moves to a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void LocationChange(object sender, WarpedEventArgs e)
        {
            // Get current location as a MineShaft
            var mineShaft = Game1.currentLocation as MineShaft;
            // Get current location as a VolcanoDungeon
            var VolcanoShaft = Game1.currentLocation as VolcanoDungeon;
           
            // Test to see if current location is a MineShaft
            if (!(mineShaft is null))
            {
                // Yes, update tracker with new data

                // Display trace information in SMAPI log
                if (mineShaft.mineLevel < 121)
                {
                    this.Monitor.Log($"{Game1.player.Name} is on level {mineShaft.mineLevel} of the mine.");
                }
                else if (mineShaft.mineLevel == 77377)
                {
                    this.Monitor.Log($"{Game1.player.Name} is in the Quarry Mine.");
                }
                else
                {
                    this.Monitor.Log($"{Game1.player.Name} on level {mineShaft.mineLevel} (level {mineShaft.mineLevel - 120} of the Skull Cavern).");
                }

                // Update tracker
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

            // Test to see if current location is a Volcano Floor
            if (!(VolcanoShaft is null))
            {
                // Yes, update tracker with new data

                // Display trace information in SMAPI log
                if (VolcanoShaft.level != 5)
                {
                    this.Monitor.Log($"{Game1.player.Name} is on volcano floor {VolcanoShaft.level}.");
                }
                else
                {
                    this.Monitor.Log($"{Game1.player.Name} is at the volcano dwarf shop. (Buy something?)");
                }

                // Update tracker
                PlayerData.CurrentVolcanoFloor = VolcanoShaft.level;
            }

            else
            {
                // No, does the tracker reflect this?
                if (PlayerData.CurrentVolcanoFloor > 0)
                {
                    // No, reset mine level tracker
                    PlayerData.CurrentVolcanoFloor = 0;
                    this.Monitor.Log($"VolcanoFloor tracker reset");
                }
            }
        }

        /// <summary>Raised when the tracker command is entered into the SMAPI console.</summary>
        /// <param name="command">The command name (tracker)</param>
        /// <param name="args">The command arguments</param>
        private void Tracker(string command, string[] args)
        {
            string Quests(ArrayList collection)
            {
                StringBuilder questsasstring = new StringBuilder("None");

                // Remove default string if array isn't empty
                if (collection.Count > 0)
                {
                    questsasstring.Remove(0, 4);
                }

                // Add each quest id to string
                foreach (var quest in collection)
                {
                    questsasstring.Append($", {quest}");

                    // Remove whitespace and comma if id is the first in the array
                    if (collection.IndexOf(quest) == 0)
                    {
                        questsasstring.Remove(0, 2);
                    }
                }

                return questsasstring.ToString();
            }

            try
            {
                // Display information in SMAPI console
                this.Monitor.Log($"\n\nMineLevel: {PlayerData.CurrentMineLevel}" +
                    $"\nVolcanoFloor: {PlayerData.CurrentVolcanoFloor}" +
                    $"\nYearsMarried: {PlayerData.CurrentYearsMarried}" +
                    $"\nQuestIDsCompleted: {Quests(PlayerDataToWrite.QuestsCompleted)}" +
                    $"\nSpecialOrdersCompleted: {Quests(PlayerData.SpecialOrdersCompleted)}" +
                    $"\nQuestsCompleted: {Game1.stats.questsCompleted}" +
                    $"\nAnniversaryDay: {PlayerData.AnniversaryDay}" +
                    $"\nAnniversarySeason: {PlayerData.AnniversarySeason}" +
                    $"\nDeathCount: {Game1.stats.timesUnconscious}" +
                    $"\nDeathCountMarried: {PlayerDataToWrite.DeathCountMarried}" +
                    $"\nDeathCountPK: {(Game1.player.isMarried() ? Game1.stats.timesUnconscious + 1 : 0)}" +
                    $"\nDeathCountMarriedPK: {(Game1.player.isMarried() ? PlayerDataToWrite.DeathCountMarried + 1 : 0)}" +
                    $"\nPassOutCount: {PlayerDataToWrite.PassOutCount}", LogLevel.Info);
            }
            catch(Exception ex)
            {
                // Throw an exception if command failed to execute
                throw new Exception("Command failed", ex);
            }
            
        }
        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {
            // Update tracker if player died, is married and tracker should update
            if(Game1.killScreen == true && Game1.player.isMarried() == true && updatedeath == true)
            {
                // Increment tracker
                PlayerDataToWrite.DeathCountMarried++;

                // Already updated, ensures tracker won't repeatedly increment
                updatedeath = false;

                // Display trace information in SMAPI log
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

                // Display trace information in SMAPI log
                if (PlayerDataToWrite.PassOutCount > 20)
                {
                    this.Monitor.Log($"{Game1.player.Name} has passed out {PlayerDataToWrite.PassOutCount} time(s). Maybe you should go to bed earlier.");
                }
                else
                {
                    this.Monitor.Log($"{Game1.player.Name} has passed out {PlayerDataToWrite.PassOutCount} time(s).");
                }

            }

            else if (Game1.timeOfDay == 2610 && updatepassout == false)
            {
                // Decrement tracker, player can stay up later
                PlayerDataToWrite.PassOutCount--;
                // Already updated, ensures tracker won't repeatedly decrement
                updatepassout = true;
                // Display trace information in SMAPI log
                this.Monitor.Log($"Nevermind, {Game1.player.Name} has actually passed out {PlayerDataToWrite.PassOutCount} time(s). Aren't you getting tired?");
            }

            // If there are quests, check if any are completed
            if (Game1.player.questLog.Count > 0)
            {
                // Iterate through each active quest
                foreach (Quest quest in Game1.player.questLog)
                {
                    // Is the quest complete?
                    if (true 
                        // Quest has an id
                        && quest.id != null
                        // Quest has been completed
                        && quest.completed == true
                        // Quest has not already been added to array list
                        && PlayerDataToWrite.QuestsCompleted.Contains(quest.id.ToString()) == false)
                    {
                        // Yes, add it to quest array if it hasn't been added already
                        PlayerDataToWrite.QuestsCompleted.Add(quest.id.ToString());
                        // Display trace information in SMAPI log
                        this.Monitor.Log($"Quest with id {quest.id} has been completed");
                    }
                }
                
            }

            var order = Game1.player.team.completedSpecialOrders;

            if (PlayerData.SpecialOrdersCompleted.Count == 0 || PlayerData.SpecialOrdersCompleted.Count < order.Count())
            {
                foreach (string name in new List<string>(order.Keys))
                {
                    if (!PlayerData.SpecialOrdersCompleted.Contains(name))
                    {
                        PlayerData.SpecialOrdersCompleted.Add(name);
                        this.Monitor.Log($"Special Order with name {name} has been completed");
                    }
                }
            }
        }
        /// <summary>Raised before/after the game writes data to save file (except the initial save creation). 
        /// This is also raised for farmhands in multiplayer.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Saved(object sender, SavedEventArgs e)
        {
            // Update old tracker
            PlayerDataToWrite.DeathCountMarriedOld = PlayerDataToWrite.DeathCountMarried;
            this.Monitor.Log("Old death tracker updated for new day");

            // Save any data to JSON file
            this.Monitor.Log("Writing data to JSON file");
            this.Helper.Data.WriteJsonFile<PlayerDataToWrite>($"data\\{Constants.SaveFolderName}.json", ModEntry.PlayerDataToWrite);
        }

        private void Title(object sender, ReturnedToTitleEventArgs e)
        {
            if(PlayerData.SpecialOrdersCompleted.Count != 0)
            {
                PlayerData.SpecialOrdersCompleted.Clear();
                this.Monitor.Log("Clearing Special Order data, ready for new save");
            }
        }
    }
}