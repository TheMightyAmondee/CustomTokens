
Custom Tokens is a mod that provides some basic additional tokens for Content Patcher, extending what can be done

### Custom Tokens registers the following tokens:
- MineLevel the player is currently on
- VolcanoFloor the player is currently on
- Anniversary of the player, split into two tokens, AnniversaryDay and AnniversarySeason
- YearsMarried
- QuestIDsCompleted, a list of quest ids that the player has completed
- QuestsCompleted, the total number of quests completed
- SOKeysCompleted, a list of the special orders a player has completed. See Special order data for how to interpret return values
- SOCompleted, total number of special orders completed
- DeathCount
- DeathCountMarried, an extension of DeathCount that tracks how many times a player has died after being married.
- DeathCountPK, (provides a more accurate value than the DeathCount token for use in the PlayerKilled event)
- DeathCountMarriedPK, an extension of DeathCountPK that tracks how many times a player has died after being married.
- PassOutCount, how many times a player has passed out.

### Using Custom Tokens:
- SMAPI must be installed
- Ensure Custom Tokens is listed as a dependency in your content pack
- Tokens used must be prefixed with the mod's unique ID e.g TheMightyAmondee.CustomTokens/MineLevel
- Many tokens require an update rate faster than CP's default as they can change throughout the day
- A per-save JSON file will be generated after the day is started for each save so the mod can track values not tracked by the game. These can be adjusted as needed as they will have an initial value of 0, which may not be accurate for older saves. Ensure the old tracker value is also updated to the same value as the current tracker value when changing values
- While the mod can determine whether most quests have been completed, some quest ids from previously completed quests in old save files need to be added manually to the save's JSON file in AdditionalQuestsCompleted. Custom quest ids can also be added here
  - If you have completed "Getting Started", add 6
  - If you have completed "Initiation" and entered the Adventurer's Guild, add 16
  - If you have completed the first part of "The Mysterious Mr. Qi" (battery pack in tunnel), add 2
  - If you have given the ornate necklace to Abigail, add 128
  - If you have given the ornate necklace to Caroline, add 129
  - If you have finished "The Pirate's Wife", add 130

### Config:
- AllowDebugging adds a single debug command so the values of the tokens can be viewed in the SMAPI console. When enabled typing "tracker" in the console will display a list of token values
- ResetDeathCountMarriedWhenDivorced will cause the DeathCountMarried token to reset to 0 when divorced, default is true.

### Tokens in more detail:
Token | default value | What it tracks | Notes 
----- | ------------- | -------------- | ------
Minelevel | 0 | Players current minelevel | Add 120 to Skull Cavern floors for token value. The quarry mine has a minelevel of 77377
VolcanoFloor | 0 | Players current floor in the Volcano Dungeon
AnniversaryDay | 0 | The day the player was married on
AnniversarySeason | No season | The season the player was married in | Value is in all lower-case
Years Married | 0 | The number of years the player has been married for
QuestIDsCompleted | None | A list of quest ids that the player has completed | Only records quests with ids as specified in the Quests.xnb
QuestsCompleted | 0 | The total number of quests completed | Includes quests with no ids e.g Bulletin board quests
SOKeysCompleted | None | A list of the special orders a player has completed 
SOCompleted | 0 | Total number of special orders completed
DeathCount | 0 | The number of deaths
DeathCountPK | 0 | Value is DeathCount + 1 when save is loaded |Because there are limits on the update rate of tokens in CP, this token can be used as a more accurate snapshot of DeathCount in some cases, mainly the PlayerKilled event
DeathCountMarried | 0 | The number of deaths that occur when the player is married
DeathCountMarriedPK | 0 | Value is DeathCountMarried + 1 when married | Because there are limits on the update rate of tokens in CP, this token can be used as a more accurate snapshot of DeathCountMarried in some cases, mainly the PlayerKilled event
PassOutCount | 0 | The number of times the player has passed out, either from exhaustion or it reaching 2AM

### Special Order data
Since Special Orders don't have an id like quests they are recorded in the save file using a unique key as shown in the table below. The mod will return this unique key for each Special Order completed in the SOKeysCompleted token

Key | Special Order Name
----|------------------
Willy | Juicy Bugs Wanted!
Willy2 | Tropical Fish
Pam | The Strong Stuff
Pierre | Pierre's Prime Produce
Robin | Robin's Project
Robin2 | Robin's Resource Rush
Emily | Rock Rejuvenation
Demetrius | Aquatic Overpopulation
Demetrius2 | Biome Balance
Gus | Gus' Famous Omelet
Lewis | Crop Order
Wizard | A Curious Substance
Wizard2 | Prismatic Jelly
Clint | Cave Patrol
Linus | Community Cleanup
Evelyn | Gifts for George
Gunther | Fragments of the past
Caroline | Island Ingredients
QiChallenge | FivePlagues
QiChallenge2 | Qi's Crop
QiChallenge3 | Let's Play A Game
QiChallenge4 | Four Precious Stones
QiChallenge5 | Qi's Hungry Challenge
QiChallenge6 | Qi's Cuisine
QiChallenge7 | Qi's Kindness
QiChallenge8 | Extended Family
QiChallenge9 | Danger In The Deep
QiChallenge10 | Skull Cavern Invasion
QiChallenge11 | Find Qi's Double
QiChallenge12 | Qi's Prismatic Grange
