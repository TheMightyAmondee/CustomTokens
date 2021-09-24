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
    public class ChildTokens
    {
        private readonly List<NPC> children;
        private readonly List<string> childdata;

        public ChildTokens()
        {
            children = new List<NPC>(2);
            childdata = new List<string>() { "birthdayday", "birthdayseason", "age", "daysold"};
        }

        public bool AllowsInput()
        {
            return true;
        }

        public bool CanHaveMultipleValues(string input = null)
        {
            return false;
        }

        public bool IsReady()
        {
            return  Context.IsWorldReady && this.children.Count > 0;
        }

        public bool UpdateContext()
        {
            bool hasChanged = false;

            if (Context.IsWorldReady)
            {
                hasChanged = DidDataChange();
            }

            return hasChanged;
        }

       
        public bool TryValidateInput(string input, out string error)
        {
            error = "";
            string[] tokenarg = input.ToLower().Trim().Split('|');

            if (tokenarg.Count() == 2)
            {
                if (tokenarg[0].Contains("childindex=") == false)
                {
                    error = "error 0";
                }                

                else if (tokenarg[1].Contains("birthdayday") == false && tokenarg[1].Contains("birthdayseason") == false && tokenarg[1].Contains("age") == false && tokenarg[1].Contains("daysold") == false)
                {
                    error = "error 1";
                }
            }

            else
            {
                error = "Incorrect number of arguments";
            }

            return error.Equals("");
        }

        public IEnumerable<string> GetValues(string input)
        {
            var output = new List<string>();

            if (input == null)
            {
                return output;
            }

            string[] args = input.Split('|');

            int childindex = Convert.ToInt32(args[0].Substring(args[0].IndexOf('=') + 1).Trim().ToLower().Replace("childindex", "").Replace(" ", ""));

            foreach (var listdata in childdata)
            {
                if (TryGetValue(childindex, listdata, out string data) == true)
                {
                    output.Add(data);
                }
                
            }
         
            return output;
        }

        private bool TryGetValue(int index,string data, out string founddata)
        {
            bool found = false;
            founddata = "";

            try
            {
                var child = this.children.ElementAt(index);

                foreach (var childdata in childdata)
                {
                    var birthday = SDate.Now().AddDays(-(child as Child).daysOld - 1);
                    switch (data)
                    {
                        case "birthdayday":
                            founddata = birthday.Day.ToString();
                            found = true;
                            break;
                        case "birthdayseason":
                            founddata = birthday.Season.ToString();
                            found = true;
                            break;
                        case "age":
                            founddata = (child as Child).Age.ToString();
                            found = true;
                            break;
                        case "daysold":
                            founddata = (child as Child).daysOld.ToString();
                            found = true;
                            break;
                        default:
                            break;
                    }

                }
            }
            catch
            {
                return found;
            }
            
            return found;

        }

        private bool DidDataChange()
        {
            bool hasChanged = false;

            if (Game1.player == null)
            {
                return hasChanged;
            }

            if (Game1.player.getChildrenCount() != this.children.Count)
            {
                this.children.Clear();
                hasChanged = true;
            }

            foreach(var child in Game1.player.getChildren())
            {
                int index = child.GetChildIndex();

                if (this.children.Contains(child) == false)
                {
                    this.children.Insert(index, child);
                    hasChanged = true;
                }

                else if (this.children.ElementAt(index) != child)
                {
                    this.children[index] = child;
                    hasChanged = true;
                }
            }

            return hasChanged;
        }
    }
}
