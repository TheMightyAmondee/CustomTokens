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
    public class ModEntry : Mod
    {


        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += this.LocationChange;
            helper.Events.GameLoop.GameLaunched += this.GameLaunched;
        }

        private void GameLaunched(object sender, GameLaunchedEventArgs e)
        {
            this.Monitor.Log($"Game Launched", LogLevel.Debug);
            var api = this.Helper.ModRegistry.GetApi<IContentPatcherAPI>("Pathoschild.ContentPatcher");
            api.RegisterToken(this.ModManifest, "PlayerName", () =>
            {
                if (Context.IsWorldReady)
                    return new[] { Game1.player.Name };
                if (SaveGame.loaded?.player != null)
                    return new[] { SaveGame.loaded.player.Name }; // lets token be used before save is fully loaded
                return null;
            });
            api.RegisterToken(this.ModManifest, "MineLevel", () =>
            {
                if (Context.IsPlayerFree)
                    return new[] { Game1.CurrentMineLevel.ToString()};
                if (SaveGame.loaded?.player != null)
                    return new[] { SaveGame.loaded.player.Name}; // lets token be used before save is fully loaded
                return null;
            });
        }
        
        private void LocationChange(object sender, WarpedEventArgs e)
        {
            this.Monitor.Log($"{Game1.player.Name} warped from {e.OldLocation} to {e.NewLocation}", LogLevel.Debug);
            var mineShaft = Game1.currentLocation as MineShaft;
           
            if (!(mineShaft is null))
            {
                this.Monitor.Log($"{Game1.player.Name} is on level {mineShaft.mineLevel}.", LogLevel.Debug);
            }            
        }
    }
}


