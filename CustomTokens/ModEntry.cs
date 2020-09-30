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
    /// Special class to track mine level
    /// </summary>
    public class MineLevelTracking
    {
        public int CurrentMineLevel { get; set; }
    }

    public class ModEntry 
        : Mod
    {
        // mine level tracking
        private MineLevelTracking _mineLevelTracking;

        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += this.LocationChange;
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this.Monitor.Log($"Game Launched", LogLevel.Debug);
            var api = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");

            // register "PlayerName" token
            api.RegisterToken(
                this.ModManifest, 
                "PlayerName", 
                () =>
                {
                    if (Context.IsWorldReady)
                    {
                        return new[] 
                        { 
                            Game1.player.Name 
                        };
                    }

                    if (SaveGame.loaded?.player != null)
                    {
                        // lets token be used before save is fully loaded
                        return new[] 
                        { 
                            SaveGame.loaded.player.Name 
                        };
                    }

                    return null;
                });

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
        }
        
        private void LocationChange(object sender, WarpedEventArgs e)
        {
            this.Monitor.Log($"{Game1.player.Name} warped from {e.OldLocation} to {e.NewLocation}", LogLevel.Debug);

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
                        new MineLevelTracking()
                        {
                            CurrentMineLevel = mineShaft.mineLevel
                        };
                }
                else
                {
                    // mine level tracking object exists!  Just update the current mine level
                    _mineLevelTracking.CurrentMineLevel = mineShaft.mineLevel;
                }
            }            
        }
    }
}


