using System.Collections.Generic;
using StardewValley.Quests;
using StardewModdingAPI;
using StardewModdingAPI.Utilities;
using StardewValley;
using Harmony;
using System;

namespace CustomTokens
{
    public class QuestData
    {
        private static IMonitor monitor;

        public static void Hook(HarmonyInstance harmony, IMonitor monitor)
        {
            QuestData.monitor = monitor;
            monitor.Log("Initialising harmony patches...");

            harmony.Patch(
                original: AccessTools.Method(typeof(Quest), nameof(Quest.questComplete)),
                postfix: new HarmonyMethod(typeof(QuestData), nameof(QuestData.questComplete_Postfix))
            );

        }

        public static void questComplete_Postfix(Quest __instance)
        {
            try
            {
                if (ModEntry.perScreen.Value.QuestsCompleted.Contains(__instance.id.Value) == false && __instance.id.Value != 0)
                {
                    ModEntry.perScreen.Value.QuestsCompleted.Add(__instance.id.Value);
                    monitor.Log($"Quest with id {__instance.id.Value} has been completed");
                }
            }
<<<<<<< HEAD
        }

        /// <summary>Determines whether a quest is complete.</summary>
        /// <param name="data">Where to save data</param>
        /// <param name="datatowrite">Where to write data that will be written</param>
        /// <param name="monitor">Provides access to the SMAPI monitor</param>
        internal void CheckForCompletedQuests(PerScreen<PlayerData> data, PlayerDataToWrite datatowrite, IMonitor monitor)
        {
            // Clear QuestlogidsNew array
            QuestlogidsNew.Clear();

            var questlog = Game1.player.questLog;

            var questdata = data.Value.QuestsCompleted;

            // Iterate through each active quest
            foreach (Quest quest in questlog)
            {              
                // Is the quest complete?
                if (true
                    // Quest has an id
                    && quest.id != null
                    // Quest has been completed
                    && quest.completed == true
                    // Quest has not already been added to array list
                    && questdata.Contains(quest.id.Value) == false)
                {
                    // Yes, add it to quest array if it hasn't been added already
                    questdata.Add(quest.id.Value);
                    monitor.Log($"Quest with id {quest.id.Value} has been completed");

                    // If quest with id 6 is completed, add it to PlayerDataToWrite if it isn't already an element
                    if (quest.id.Value == 6 && datatowrite.AdditionalQuestsCompleted.Contains(quest.id.Value) == false)
                    {
                        datatowrite.AdditionalQuestsCompleted.Add(quest.id.Value);
                    }
                }

                // Add current quests to QuestlogidsNew
                else if (QuestlogidsNew.Contains(quest.id.Value) == false && quest.completed == false)
                {
                    QuestlogidsNew.Add(quest.id.Value);
                }

                else if(quest.destroy.Value == true)
                {
                    QuestlogidsOld.Remove(quest.id.Value);
                }
            }

            // Check for completed quests with no rewards by comparing two arrays

            // Iterate through each quest id recorded in QuestlogidsOld
            foreach (int questid in QuestlogidsOld)
            {
                // If QuestlogidsOld contains an id that QuestlogidsNew doesn't, the quest with that id is completed
                if (QuestlogidsNew.Contains(questid) == false && questdata.Contains(questid) == false)
                {
                    questdata.Add(questid);
                    monitor.Log($"Quest with id {questid} has been completed");                                       
                   
                    if (true
                       && (false
                       // If these quests are completed, add it to PlayerDataToWrite if it isn't already an element
                       || questid == 16
                       || questid == 128
                       || questid == 129
                       || questid == 130)
                       && datatowrite.AdditionalQuestsCompleted.Contains(questid) == false)
                    {
                        datatowrite.AdditionalQuestsCompleted.Add(questid);
                    }
                }               
            }

            // Necklace given to Abigail, remove alternate quest so it won't be marked as completed
            if (questdata.Contains(128) == true)
            {
                QuestlogidsOld.Remove(129);
            }

            // Necklace given to Caroline, remove alternate quest so it won't be marked as completed
            else if (questdata.Contains(129) == true)
=======
            catch (Exception ex)
>>>>>>> Version2.0
            {
                monitor.Log($"Failed in {nameof(questComplete_Postfix)}, quest tokens may not work:\n{ex}", LogLevel.Error);
            }           
        }

        internal void CheckForCompletedSpecialOrders(PerScreen<PlayerData> data, IMonitor monitor)
        {
            var order = Game1.player.team.completedSpecialOrders;

            // Check for completed special orders
            if (data.Value.SpecialOrdersCompleted.Count < order.Count())
            {
                foreach (string questkey in new List<string>(order.Keys))
                {
                    if (data.Value.SpecialOrdersCompleted.Contains(questkey) == false)
                    {
                        data.Value.SpecialOrdersCompleted.Add(questkey);
                        monitor.Log($"Special Order with key {questkey} has been completed");
                    }
                }
            }
        }
    }

    
}