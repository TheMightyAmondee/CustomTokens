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
        public string AnniversaryDate { get; set; }

    }

    public class ModEntry 
        : Mod
    {
        // mine level tracking
        private PlayerDataTracking _mineLevelTracking;
        // years married tracking
        private PlayerDataTracking _yearsMarried;
        // anniversary tracker
        private PlayerDataTracking _Anniversarydate;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += this.LocationChange;
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
            helper.Events.GameLoop.DayStarted += this.DayStarted;
            helper.Events.GameLoop.ReturnedToTitle += this.ExitSave;
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
                "Anniversary",
                () =>
                {
                    if (Context.IsWorldReady)
                    {
                        var Anniversary = _Anniversarydate is null
                                                    ? "No anniversary"
                                                    : _Anniversarydate.AnniversaryDate;

                        return new[]
                        {
                            Anniversary
                        };
                    }

                    return null;
                });
            //Register "Anniversary" token
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
            //Anniversary
            var anniversary = SDate.Now().AddDays(-(DaysMarried - 1));
            string anniversarydate = $"{anniversary.Season}_{anniversary.Day}";

            //Test if player is married
            if (Game1.player.isMarried() is false)
            {
                this.Monitor.Log($"{Game1.player.Name} is not married");
            }

            //Player is married, tokens can exist
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

                //Does the anniversary tracker exist?
                //No, create tracker
                if( _Anniversarydate is null)
                {
                    _Anniversarydate =
                        new PlayerDataTracking()
                        {
                            AnniversaryDate = anniversarydate
                        };
                }
                //Yes, update tracker
                else
                {
                    _Anniversarydate.AnniversaryDate = anniversarydate;
                }

                this.Monitor.Log($"{Game1.player.Name} has been married for {YearsMarried} year(s)", LogLevel.Debug);

                this.Monitor.Log($"For use in tokens, anniversary is {anniversarydate}", LogLevel.Debug);
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
                this.Monitor.Log($"{Game1.player.Name} is on level {mineShaft.mineLevel}.");

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
                this.Monitor.Log($"Mine Level updated to {mineShaft.mineLevel}");
            }

            else
            {
                // does the mine tracker exist?
                if (!(_mineLevelTracking is null))
                {
                    // reset mine level tracker
                    _mineLevelTracking = null;
                    this.Monitor.Log($"Mine tracker reset to null");
                }
            }
        }

        //Discard tokens when save is exited
        private void ExitSave(object sender, ReturnedToTitleEventArgs e)
        {
            if(!(_Anniversarydate is null))
            {
                _Anniversarydate = null;
                this.Monitor.Log($"Anniversary reset");
            }

            if(!(_yearsMarried is null))
            {
                _yearsMarried = null;
                this.Monitor.Log($"YearsMarried reset");
            }
        }
    }
}


