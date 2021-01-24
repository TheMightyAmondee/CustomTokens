using System;
using System.Collections;
using StardewValley.Quests;
using StardewModdingAPI.Utilities;
using StardewValley;

namespace CustomTokens
{
    public class QuestData
    {
        public ArrayList Questlogids = new ArrayList();
        public void AddCompletedQuests(PerScreen<PlayerData> data, PlayerDataToWrite datatowrite)
        {
            
            var questlog = Game1.player.questLog;

            var questdata = data.Value.QuestsCompleted;

            // Get quests in questlog and add to an array
            foreach (Quest quest in questlog)
            {
                Questlogids.Add(quest.id);
            }

            // Get additional quests completed, manually added quests
            foreach(long questid in datatowrite.AdditionalQuestsCompleted)
            {
                questdata.Add((int)questid);
            }

            // Method for determining whether a quest received in the mail is completed
            void MailQuests(int questid, string mailname)
            {
                if (true
                    // Player has or will receive the mail to begin the quest
                    && Game1.player.hasOrWillReceiveMail(mailname) == true
                    // Quest is not present in the questlog
                    && Questlogids.Contains(questid) == false
                    // Player's mailbox does not contain the mail, they have actually begun and finished the quest
                    && Game1.player.mailbox.Contains(mailname) == false
                    // questdata does not currently contain the quest id
                    && questdata.Contains(questid) == false)
                {
                    questdata.Add(questid);
                }
            }

            // Method for determining whether a quest that involves an event is completed
            void EventQuest(int questid, int eventid)
            {
                if (true 
                    // Player has seen the event that finishes or begins the quest
                    && Game1.player.eventsSeen.Contains(eventid) == true
                    // Quest is not present in the questlog
                    && Questlogids.Contains(questid) == false
                    // questdata does not currently contain the quest id
                    && questdata.Contains(questid) == false)
                {
                    questdata.Add(questid);
                }
            }

            // Method for determining whether a quest that requires another quest to have been completed is completed
            void QuestPreReq(int questid, int questprereq)
            {
                if (true
                    // questdata contains the id of the quest that must be completed before this quest can be done
                    && questdata.Contains(questprereq) == true
                    // Quest is not present in the questlog
                    && Questlogids.Contains(questid) == false
                    // questdata does not currently contain the quest id
                    && questdata.Contains(questid) == false)
                {
                    questdata.Add(questid);
                }
            }

            // Introductions quest, if it's not in the log, it would have been completed
            if (Questlogids.Contains(9) == false)
            {
                questdata.Add(9);
            }

            // How to win friends quest
            QuestPreReq(25, 9);

            // Raising animals quest, 6 must be added manually
            QuestPreReq(7, 6);

            // To the beach quest
            EventQuest(13, 739330);

            // Advancement quest, 6 must be added manually
            QuestPreReq(8, 6);

            // Explore the mines quest
            EventQuest(14, 100162);

            // Deeper in the mine quest
            QuestPreReq(17, 14);

            // To the bottom quest
            QuestPreReq(18, 17);

            // The skull key quest
            if (Game1.player.hasSkullKey == true && Questlogids.Contains(19) == false)
            {
                questdata.Add(19);
            }

            // Archaeology quest pt 1
            EventQuest(23, 0);

            // Archaeology quest pt 2
            QuestPreReq(24, 23);

            // Rat problem quest
            EventQuest(26, 611439);

            // Forging ahead quest
            EventQuest(11, 992553);

            // Smelting quest
            QuestPreReq(12, 11);

            // Initiation quest
            QuestPreReq(15, 14);

            // Errand for your wife quest
            EventQuest(126, 3917601);

            // Hayley's cake-walk quest
            EventQuest(127, 6184644);

            // The mysterious Mr. Qi quests, 2 needs to be added manually
            QuestPreReq(3, 2);
            QuestPreReq(4, 3);
            QuestPreReq(5, 4);

            // Cryptic note quest
            if (Game1.player.secretNotesSeen.Contains(10) == true && Questlogids.Contains(30) == false && questdata.Contains(30) == false)
            {
                questdata.Add(30);
            }

            // Strange note quest
            if (Game1.player.secretNotesSeen.Contains(23) == true && Questlogids.Contains(29) == false && questdata.Contains(30) == false)
            {
                questdata.Add(29);
            }

            // A winter mystery quest (Is the figure Krobus? You decide.)
            EventQuest(31, 2120303);

            // Marnie's request quest
            EventQuest(21, 92);

            // Fish casserole quest, both possible events
            EventQuest(22, 94);
            EventQuest(22, 95);

            // Dark talisman quest
            if (Game1.player.hasDarkTalisman == true)
            {
                questdata.Add(28);
            }

            // Goblin problem quest
            if (Game1.player.hasMagicInk == true)
            {
                questdata.Add(27);
            }

            // Meet the wizard quest
            EventQuest(1, 112);

            // Mail quests
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

            // Qi's challenge quest
            if (Game1.player.hasOrWillReceiveMail("QiChallengeComplete") == true)
            {
                questdata.Add(20);
            }

            // Sort array and clear unnecessary data
            questdata.Sort();
            Questlogids.Clear();

            // Quests 130, 129, 128 and 16 must also be added manually
        }
    }
}