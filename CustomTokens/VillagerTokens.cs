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
    class VillagerTokens
        : BaseAdvancedToken
    {
        private readonly Dictionary<string, NPC> villagers;
        private readonly List<string> villagerdata;

        public VillagerTokens()
        {
            villagers = new Dictionary<string, NPC>();
            villagerdata = new List<string>() { "birthdayday", "birthdayseason", "age", "manners", "optimism", "gender"};
        }

        public override bool IsReady()
        {
            return Context.IsWorldReady && Game1.currentLocation != null;
        }

        public override bool UpdateContext()
        {
            bool hasChanged = false;

            if (Context.IsWorldReady && Game1.currentLocation != null)
            {
                hasChanged = DidDataChange();
            }

            return hasChanged;
        }

        public override bool TryValidateInput(string input, out string error)
        {
            error = "";
            string[] tokenarg = input.ToLower().Trim().Split('|');

            if (tokenarg.Count() == 2)
            {
                if (tokenarg[0].Contains("villager=") == false)
                {
                    error = "error 0";
                }

                foreach (var token in villagerdata)
                {
                    if (tokenarg[1].Contains(token))
                    {
                        error = "";
                        break;
                    }

                    error = "error 1";
                }
            }

            else
            {
                error = "Incorrect number of arguments";
            }

            return error.Equals("");
        }

        public override IEnumerable<string> GetValues(string input)
        {
            var output = new List<string>();

            if (input == null)
            {
                return output;
            }

            string[] args = input.Split('|');

            string villagername = args[0].Substring(args[0].IndexOf('=') + 1).Trim().ToLower().Replace("villager", "").Replace(" ", "");
            string villagerdata = args[1].Trim().ToLower().Replace("=", "");

            if (TryGetValue(villagername, villagerdata, out string data) == true)
            {
                output.Add(data);
            }

            return output;
        }

        private bool TryGetValue(string villagername, string data, out string founddata)
        {
            bool found = false;
            founddata = "";

            try
            {
                var villager = this.villagers[villagername];

                switch (data)
                {
                    case "birthdayday":
                        founddata = villager.Birthday_Day.ToString();
                        found = true;
                        break;
                    case "birthdayseason":
                        founddata = villager.Birthday_Season;
                        found = true;
                        break;
                    case "age":
                        founddata = villager.Age.ToString();
                        found = true;
                        break;
                    case "manners":
                        founddata = villager.Manners.ToString();
                        found = true;
                        break;
                    case "optimism":
                        founddata = villager.Optimism.ToString();
                        found = true;
                        break;
                    case "gender":
                        founddata = villager.Gender.ToString();
                        found = true;
                        break;
                    default:
                        break;
                }
            }
            catch
            {
                return found;
            }

            return found;

        }

        protected override bool DidDataChange()
        {
            bool hasChanged = false;

            if (Game1.currentLocation == null)
            {
                return hasChanged;
            }

            foreach (var villager in Utility.getAllCharacters())
            {
                if (villager.isVillager() && this.villagers.ContainsKey(villager.Name.ToLower()) == false)
                {
                    villagers.Add(villager.Name.ToLower(), villager);
                    hasChanged = true;
                }

                else if (villager.isVillager() && this.villagers[villager.Name.ToLower()] != villager)
                {
                    this.villagers[villager.Name.ToLower()] = villager;
                    hasChanged = true;
                }
            }
            return hasChanged;
        }
    }
}
