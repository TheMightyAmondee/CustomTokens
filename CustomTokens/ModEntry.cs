﻿using System;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using System.Collections.Generic;
using Harmony;


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
        internal ModConfig config;

        public static PlayerData PlayerData { get; private set; } = new PlayerData();
        internal static TrackerCommand TrackerCommand { get; private set; } = new TrackerCommand();
        internal static LocationTokens LocationTokens { get; private set; } = new LocationTokens();
        internal static DeathAndExhaustionTokens DeathAndExhaustionTokens { get; private set; } = new DeathAndExhaustionTokens();
        internal static MarriageTokens MarriageTokens { get; private set; } = new MarriageTokens();
        internal static QuestData QuestData { get; private set; } = new QuestData();

        public static readonly PerScreen<PlayerData> perScreen = new PerScreen<PlayerData>(createNewState: () => PlayerData);

        private static readonly string[] tokens = { "DeathCountMarried", "PassOutCount", "QuestsCompleted" };

        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            // Add required event helpers
            helper.Events.Player.Warped += this.LocationChange;
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            helper.Events.GameLoop.UpdateTicked += this.UpdateTicked;
            helper.Events.GameLoop.Saving += this.Saving;
            helper.Events.GameLoop.ReturnedToTitle += this.Title;
            helper.Events.GameLoop.DayStarted += this.DayStarted;

            // Read the mod config for values and create one if one does not currently exist
            this.config = this.Helper.ReadConfig<ModConfig>();

            // Add debug command if AllowDebugging is true
            if (this.config.AllowDebugging == true)
            {
                helper.ConsoleCommands.Add("tracker", "Displays the current tracked values", this.Tracker);
            }

            var harmony = HarmonyInstance.Create(this.ModManifest.UniqueID);
            QuestData.Hook(harmony, this.Monitor);
        }

        /// <summary>Raised after the game is launched, right before the first update tick.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            // Access Content Patcher API
            var api = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");

            if(api != null)
            {
                // Register "MineLevel" token
                api.RegisterToken(
                    this.ModManifest,
                    "MineLevel",
                    () =>
                    {
                        if (Context.IsWorldReady)
                        {
                            var currentMineLevel = ModEntry.perScreen.Value.CurrentMineLevel;

                            return new[]
                            {
                            currentMineLevel.ToString()
                            };
                        }

                        return null;
                    });

                // Register "DeepestMineLevel" token
                api.RegisterToken(
                    this.ModManifest,
                    "DeepestMineLevel",
                    () =>
                    {
                        if (Context.IsWorldReady)
                        {
                            var deepestMineLevel = ModEntry.perScreen.Value.DeepestMineLevel;

                            return new[]
                            {
                            deepestMineLevel.ToString()
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
                            var currentVolcanoFloor = ModEntry.perScreen.Value.CurrentVolcanoFloor;

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
                            var AnniversaryDay = ModEntry.perScreen.Value.AnniversaryDay;

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
                            var AnniversarySeason = ModEntry.perScreen.Value.AnniversarySeason;

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
                           var currentYearsMarried = ModEntry.perScreen.Value.CurrentYearsMarried;

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
                           ? ModEntry.perScreen.Value.DeathCountMarried
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
                           ? ModEntry.perScreen.Value.DeathCountMarried + 1
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
                           var currentpassoutcount = ModEntry.perScreen.Value.PassOutCount;

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
                            // Create array with the length of the QuestsCompleted array list
                            string[] questsdone = new string[ModEntry.perScreen.Value.QuestsCompleted.Count];

                            // Set each value in new array to be the same as in QuestCompleted
                            foreach (var quest in ModEntry.perScreen.Value.QuestsCompleted)
                            {
                                questsdone.SetValue(quest.ToString(), ModEntry.perScreen.Value.QuestsCompleted.IndexOf(quest));
                            }

                           return questsdone;
                       }

                       return null;
                   });

                // Register "SOIDsCompleted" token
                api.RegisterToken(
                   this.ModManifest,
                   "SOIDsCompleted",
                   () =>
                   {
                       if (Context.IsWorldReady)
                       {

                       // Create array with the length of the SpecialOrdersCompleted array list
                       string[] ordersdone = new string[ModEntry.perScreen.Value.SpecialOrdersCompleted.Count];

                       // Set each value in new array to be the same as in SpecialOrdersCompleted
                       foreach (var order in ModEntry.perScreen.Value.SpecialOrdersCompleted)
                           {
                               ordersdone.SetValue(order, ModEntry.perScreen.Value.SpecialOrdersCompleted.IndexOf(order));
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
                           var totalspecialorderscompleted = ModEntry.perScreen.Value.SpecialOrdersCompleted.Count;

                           return new[]
                           {
                           totalspecialorderscompleted.ToString()
                           };

                       }

                       return null;
                   });
            }
            else
            {
                this.Monitor.Log("Content Patcher API not found, I'm not going to do anything useful", LogLevel.Warn);
            }
          
        }

        /// <summary>The method called after a new day starts.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            foreach(var token in tokens)
            {
                if (!Game1.player.modData.ContainsKey($"{this.ModManifest.UniqueID}.{token}"))
                {
                    Game1.player.modData.Add($"{this.ModManifest.UniqueID}.{token}", "");
                }
            }
            // Read JSON file and create if needed
            //PlayerDataToWrite = this.Helper.Data.ReadJsonFile<PlayerDataToWrite>($"data\\{Constants.SaveFolderName}.json") ?? new PlayerDataToWrite();

            string[] QuestsComplete = Game1.player.modData[$"{this.ModManifest.UniqueID}.QuestsCompleted"].Split('/');

            foreach(string questid in QuestsComplete)
            {
                if (questid != "" && ModEntry.perScreen.Value.QuestsCompleted.Contains(int.Parse(questid)) == false)
                {
                    ModEntry.perScreen.Value.QuestsCompleted.Add(int.Parse(questid));
                }
            }

            ModEntry.perScreen.Value.DeepestMineLevel = Game1.player.deepestMineLevel;

            DeathAndExhaustionTokens.updatepassout = true;
            DeathAndExhaustionTokens.updatedeath = true;
            this.Monitor.Log($"Trackers set to update");

            MarriageTokens.UpdateMarriageTokens(this.Monitor, ModEntry.perScreen, this.config);
        }

        /// <summary>Raised after the current player moves to a new location.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void LocationChange(object sender, WarpedEventArgs e)
        {
            LocationTokens.UpdateLocationTokens(this.Monitor, ModEntry.perScreen);
        }

        /// <summary>Raised when the tracker command is entered into the SMAPI console.</summary>
        /// <param name="command">The command name (tracker)</param>
        /// <param name="args">The command arguments</param>
        private void Tracker(string command, string[] args)
        {
            TrackerCommand.DisplayInfo(this.Monitor, ModEntry.perScreen);
        }

        /// <summary>Raised after the game state is updated (≈60 times per second).</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void UpdateTicked(object sender, UpdateTickedEventArgs e)
        {          
            DeathAndExhaustionTokens.UpdateDeathAndExhaustionTokens(this.Helper, this.Monitor, ModEntry.PlayerData, this.config);
            // Check if any special orders have been completed
            QuestData.CheckForCompletedSpecialOrders(ModEntry.perScreen, this.Monitor);

        }

        /// <summary>Raised before/after the game writes data to save file (except the initial save creation). 
        /// This is also raised for farmhands in multiplayer.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void Saving(object sender, SavingEventArgs e)
        {
            string[] QuestsComplete = Game1.player.modData[$"{this.ModManifest.UniqueID}.QuestsCompleted"].Split('/');

            foreach(string questid in QuestsComplete)
            {
                if(questid != "")
                {
                    ModEntry.perScreen.Value.QuestsCompleted.Remove(int.Parse(questid));
                }
            }

            foreach (int questid in ModEntry.perScreen.Value.QuestsCompleted)
            {
                Game1.player.modData[$"{this.ModManifest.UniqueID}.QuestsCompleted"] = Game1.player.modData[$"{this.ModManifest.UniqueID}.QuestsCompleted"] + $"{questid}/";
            }

            ModEntry.perScreen.Value.QuestsCompleted.Clear();

            // Update old tracker
            Game1.player.modData[$"{this.ModManifest.UniqueID}.DeathCountMarried"] = ModEntry.perScreen.Value.DeathCountMarried.ToString();
            Game1.player.modData[$"{this.ModManifest.UniqueID}.PassOutCount"] = ModEntry.perScreen.Value.PassOutCount.ToString();
            this.Monitor.Log("Trackers updated for new day");
        }

        private void Title(object sender, ReturnedToTitleEventArgs e)
        {
            if (ModEntry.perScreen.Value.SpecialOrdersCompleted.Count != 0)
            {
                ModEntry.perScreen.Value.SpecialOrdersCompleted.Clear();
                this.Monitor.Log("Clearing Special Order data, ready for new save");
            }

            if (ModEntry.perScreen.Value.QuestsCompleted.Count != 0)
            {
                ModEntry.perScreen.Value.QuestsCompleted.Clear();               
                this.Monitor.Log("Clearing Quest data, ready for new save");
            }
        }
    }
}