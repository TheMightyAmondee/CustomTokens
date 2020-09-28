using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;

namespace CustomTokens
{
   public class ModEntry : Mod
    {
        public override void Entry(IModHelper helper)
        {
            helper.Events.Player.Warped += this.LocationChange;
            helper.Events.Input.ButtonPressed += this.OnButtonPressed;
        }


        private void OnButtonPressed(object sender, ButtonPressedEventArgs b)
        {
            this.Monitor.Log($"{Game1.player.Name} pressed {b.Button}.", LogLevel.Debug);
        }


        private void LocationChange(object sender, WarpedEventArgs e)
        {

            var mineShaft = Game1.currentLocation as MineShaft;

            if (!(mineShaft is null))
            {
                this.Monitor.Log($"{Game1.player.Name} is on level {mineShaft.mineLevel}.", LogLevel.Debug);
            }            
        }
    }
}
