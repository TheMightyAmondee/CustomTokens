using System;
using System.Collections;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;
using System.Text;
using StardewValley.Quests;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewModdingAPI.Utilities;
using StardewValley;
using StardewValley.Locations;
using System.Collections.Generic;

namespace CustomTokens
{
    public class QuestsCompleted
    {
        public void AddCompletedQuests(PlayerDataToWrite datatowrite)
        {
            // Add previous quest data
            var questlog = Game1.player.questLog;

            var questdata = datatowrite.QuestsCompleted;

            void MailQuests(int questid, string mailname)
            {
                if (Game1.player.hasOrWillReceiveMail(mailname) && !questlog.Contains(Quest.getQuestFromId(questid)) && !Game1.player.mailbox.Contains(mailname))
                {
                    questdata.Add(questid);
                }
            }

            void EventQuest(int questid, int eventid)
            {
                if (!System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debugger.Launch();
                }
                if (Game1.player.eventsSeen.Contains(eventid) && !questlog.Contains(Quest.getQuestFromId(questid)))
                {
                    questdata.Add(questid);
                }
            }

            void QuestPreReq(int questid, int questprereq)
            {
                if (!System.Diagnostics.Debugger.IsAttached)
                {
                    System.Diagnostics.Debugger.Launch();
                }
                if (questdata.Contains(questprereq) && !questlog.Contains(Quest.getQuestFromId(questid)))
                {
                    questdata.Add(questid);
                }
            }

            if (!questlog.Contains(Quest.getQuestFromId(9)))
            {
                questdata.Add(9);
            }

            QuestPreReq(25, 9);

            QuestPreReq(7, 6);

            EventQuest(13, 739330);

            QuestPreReq(8, 6);

            EventQuest(14, 100162);

            EventQuest(17, 100162);

            QuestPreReq(18, 17);

            if (Game1.player.hasSkullKey == true && !questlog.Contains(Quest.getQuestFromId(19)))
            {
                questdata.Add(19);
            }

            EventQuest(23, 0);

            QuestPreReq(24, 23);

            EventQuest(26, 611439);
            EventQuest(11, 992553);

            QuestPreReq(12, 11);

            QuestPreReq(15, 14);

            if (Game1.player.stats.slimesKilled >= 10 && !questlog.Contains(Quest.getQuestFromId(16)) && questdata.Contains(15))
            {
                questdata.Add(16);
            }

            EventQuest(126, 3917601);

            EventQuest(127, 6184644);

            QuestPreReq(3, 2);
            QuestPreReq(4, 3);
            QuestPreReq(5, 4);

            if (Game1.player.secretNotesSeen.Contains(10) && !questlog.Contains(Quest.getQuestFromId(30)))
            {
                questdata.Add(30);
            }

            EventQuest(31, 2120303);

            EventQuest(21, 92);

            EventQuest(22, 94);

            if (Game1.player.hasDarkTalisman == true)
            {
                questdata.Add(28);
            }

            if (Game1.player.hasMagicInk == true)
            {
                questdata.Add(29);
            }

            if (Game1.player.hasOrWillReceiveMail("wizardJunimoNote") && questdata.Contains(26) && !Game1.player.mailbox.Contains("wizardJunimoNote"))
            {
                questdata.Add(1);
            }

            MailQuests(100, "spring_11_1");
            MailQuests(101, "spring_19_1");
            MailQuests(102, "summer_3_1");
            MailQuests(103, "summer_14_1");
            MailQuests(104, "summer_20_1");
            MailQuests(105, "summer_25_1");
            MailQuests(106, "fall_3_1");
            MailQuests(107, "fall_8_1");
            MailQuests(108, "fall_19_1");
            MailQuests(109, "winter_2_1");
            MailQuests(110, "winter_6_1");
            MailQuests(111, "winter_12_1");
            MailQuests(112, "winter_17_1");
            MailQuests(113, "winter_21_1");
            MailQuests(114, "winter_26_1");
            MailQuests(115, "spring_6_2");
            MailQuests(116, "spring_15_2");
            MailQuests(117, "spring_21_2");
            MailQuests(118, "summer_6_2");
            MailQuests(119, "summer_15_2");
            MailQuests(120, "summer_21_2");
            MailQuests(121, "fall_6_2");
            MailQuests(122, "fall_19_2");
            MailQuests(123, "winter_5_2");
            MailQuests(124, "winter_13_2");
            MailQuests(125, "winter_19_2");

            if (Game1.player.hasOrWillReceiveMail("QiChallengeComplete"))
            {
                questdata.Add(20);
            }
        }
    }
}