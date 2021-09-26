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
using System.Reflection;

namespace CustomTokens
{
    class ChildTokens
        : BaseAdvancedToken
    {
        private readonly Dictionary<string, List<Child>>  children;

        public ChildTokens()
        {
            children = new Dictionary<string, List<Child>>(StringComparer.OrdinalIgnoreCase)
            {
                [host] = new List<Child>(),
                [loc] = new List<Child>()
            };       
        }
       
        public override bool TryValidateInput(string input, out string error)
        {
            error = "";
            string[] tokenarg = input.ToLower().Trim().Split('|');

            if (tokenarg.Count() == 3)
            {
                if (tokenarg[0].Contains("player=") == false)
                {
                    error += "player argument not found";
                }
                else if (tokenarg[0].IndexOf('=') == tokenarg[0].Length - 1)
                {
                    error += "player argument not provided a value. Must be one of the following values: 'host', 'local'. ";
                }
                else
                {
                    string playerType = tokenarg[0].Substring(tokenarg[0].IndexOf('=') + 1).Trim().Replace("player", "");

                    if (playerType.Equals("host") == false && playerType.Equals("local") == false)
                    {
                        error += "player argument must be one of the following values: 'host', 'local'. ";
                    }
                }

                if (tokenarg[1].Contains("childindex=") == false)
                {
                    error += "childindex argument not found";
                }
                else if (tokenarg[0].IndexOf('=') == tokenarg[0].Length - 1)
                {
                    error += "childindex argument not provided a value.";
                }
                else
                {
                    string statArg = tokenarg[1].Substring(tokenarg[1].IndexOf('=') + 1);
                    if (statArg.Any(ch => !char.IsLetterOrDigit(ch) && ch != ' '))
                    {
                        error += "childindex argument must be numeric";
                    }
                }

                if (tokenarg[2].Contains("birthdayday") == false && tokenarg[2].Contains("birthdayseason") == false && tokenarg[2].Contains("daysold") == false && tokenarg[2].Contains("darkskinned") == false && tokenarg[2].Contains("hat") == false)
                {
                    error += "unrecognised argument value at index 2. Must be one of 'birthdayday' 'birthdayseason' 'daysold' 'darkskinned' 'hat'";
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

            string playertype = args[0].Substring(args[0].IndexOf('=') + 1).Trim().ToLower().Replace("player", "").Replace(" ", "");
            int childindex = Convert.ToInt32(args[1].Substring(args[1].IndexOf('=') + 1).Trim().ToLower().Replace("childindex", "").Replace(" ", ""));
            string tokenvalue = args[2].Trim().ToLower().Replace("=","");          


            if (playertype == "host")
            {
                bool found = TryGetValue(host, childindex, tokenvalue, out string hostdata);

                if (found == true)
                {
                    output.Add(hostdata);
                }
            }

            else if (playertype == "local")
            {
                bool found = TryGetValue(loc, childindex, tokenvalue, out string hostdata);

                if (found == true)
                {
                    output.Add(hostdata);
                }
            }

            return output;
        }

        private bool TryGetValue(string playertype, int index, string token, out string founddata)
        {
            bool found = false;
            founddata = "";

            if (Game1.IsMasterGame == true && playertype.Equals(loc) == true)
            {
                playertype = host;
            }

            foreach (var child in this.children[playertype])
            {
                if (child.GetChildIndex() == index)
                {
                    found = true;
                    var birthday = SDate.Now().AddDays(-(child.daysOld - 1)) ?? SDate.Now();

                    switch (token)
                    {
                        case "birthdayday":
                            founddata = birthday.Day.ToString();
                            break;
                        case "birthdayseason":
                            founddata = birthday.Season.ToString();
                            break;
                        case "daysold":
                            founddata = child.daysOld.ToString();
                            break;
                        case "darkskinned":
                            founddata = child.darkSkinned.Value.ToString().ToLower();
                            break;
                        case "hat":
                            founddata = child.hat.Value == null ? "null" : child.hat.Value.Name.ToString();
                            break;
                    }

                }
            }
                     
            return found;

        }

        protected override bool DidDataChange()
        {
            bool hasChanged = false;
            string playertype;

            if (Game1.IsMasterGame == false)
            {
                playertype = loc;

                if (Game1.player.getChildren().Equals(this.children[playertype]) == false)
                {
                    hasChanged = true;
                    this.children[playertype] = Game1.player.getChildren();
                }

            }

            playertype = host;

            if (Game1.MasterPlayer.getChildren().Equals(this.children[playertype]) == false)
            {
                hasChanged = true;
                this.children[playertype] = Game1.MasterPlayer.getChildren();
            }

            return hasChanged;
        }
    }
}
