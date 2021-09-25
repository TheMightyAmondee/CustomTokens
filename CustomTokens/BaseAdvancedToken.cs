using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley.Characters;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace CustomTokens
{
    internal abstract class BaseAdvancedToken
    {
        internal static readonly string host = "hostPlayer", loc = "localPlayer";
        public virtual bool AllowsInput()
        {
            return true;
        }

        public virtual bool CanHaveMultipleValues(string input = null)
        {
            return false;
        }

        public virtual bool IsReady()
        {
            return Context.IsWorldReady;
        }

        public virtual bool UpdateContext()
        {
            bool hasChanged = false;

            if (Context.IsWorldReady)
            {
                hasChanged = DidDataChange();
            }

            return hasChanged;
        }

        public virtual bool HasBoundedRangeValues(string input, out int min, out int max)
        {
            min = 0;
            max = int.MaxValue;
            return true;
        }

        public abstract IEnumerable<string> GetValues(string input);

        public abstract bool TryValidateInput(string input, out string error);

        protected abstract bool DidDataChange();
    }
}
