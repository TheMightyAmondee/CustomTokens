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

    /// <summary>
    /// Special class to track required playerdata
    /// </summary>
    public class PlayerDataTracking
    {
        public int CurrentMineLevel { get; set; }
        public double CurrentYearsMarried { get; set; }
    }

    public class ModEntry 
        : Mod
    {
        // mine level tracking
        private PlayerDataTracking _mineLevelTracking;

        private PlayerDataTracking _yearsMarried;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += this.LocationChange;
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            helper.Events.GameLoop.DayStarted += this.DayStarted;
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {            
            var api = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");

            // register "MineLevel" token
            api.RegisterToken(
                this.ModManifest, 
                "MineLevel", 
                () =>
                {
                    if (Context.IsWorldReady)
                    {
                        var currentMineLevel = _mineLevelTracking is null
                                                    ? 0
                                                    : _mineLevelTracking.CurrentMineLevel;

                        return new[] 
                        { 
                            currentMineLevel.ToString() 
                        };
                    }
                    
                    return null;
                });

            //Register "YearsMarried" token
            api.RegisterToken(
                this.ModManifest,
                "YearsMarried",
                () =>
                {
                    if (Context.IsWorldReady)
                    {
                        var currentYearsMarried = _yearsMarried is null
                                                    ? 0
                                                    : _yearsMarried.CurrentYearsMarried;

                        return new[]
                        {
                            currentYearsMarried.ToString()
                        };
                    }

                    return null;
                });
        }

        private void DayStarted(object sender, DayStartedEventArgs e)
        {
            //Get days married
            int DaysMarried = Game1.player.GetDaysMarried();
            float Years = DaysMarried / 112;
            //Years married
            double YearsMarried = Math.Floor(Years);

            //Test if player is married
            if(Game1.player.isMarried() is false)
            {
                this.Monitor.Log($"{Game1.player.Name} is not married");
            }
            else
            {
                // does the tracker for years married exist?
                //No, create tracker object
                if (_yearsMarried is null)
                {
                    _yearsMarried =
                        new PlayerDataTracking()
                        {
                            CurrentYearsMarried = YearsMarried
                        };

                }
                //Yes, update tracker
                else
                {
                    _yearsMarried.CurrentYearsMarried = YearsMarried;
                }
            }
        }
        private void LocationChange(object sender, WarpedEventArgs e)
        {
#if DEBUG
            if(!System.Diagnostics.Debugger.IsAttached)
            {
               // System.Diagnostics.Debugger.Launch();
            }
#endif
            // get current location as a MineShaft
            var mineShaft = Game1.currentLocation as MineShaft;
           
            // test to see if current location is a MineShaft
            if (!(mineShaft is null))
            {
                this.Monitor.Log($"{Game1.player.Name} is on level {mineShaft.mineLevel}.", LogLevel.Debug);

                // test for mine level tracking
                if (_mineLevelTracking is null)
                {
                    // create mine level tracking object with current mine level
                    _mineLevelTracking =
                        new PlayerDataTracking()
                        {
                            CurrentMineLevel = mineShaft.mineLevel
                        };
                }
                else
                {
                    // mine level tracking object exists!  Just update the current mine level
                    _mineLevelTracking.CurrentMineLevel = mineShaft.mineLevel;
                }
                this.Monitor.Log($"Mine Level updated to {mineShaft.mineLevel}", LogLevel.Trace);
            }

            else
            {
                // does the mine tracker exist?
                if (!(_mineLevelTracking is null))
                {
                    // reset mine level tracker
                    _mineLevelTracking = null;
                    this.Monitor.Log($"Mine tracker reset",LogLevel.Trace);
                }
            }
        }
    }
}


