
Custom Tokens is a mod that provides some basic additional tokens for Content Patcher, extending what can be done

### Custom Tokens registers the following tokens:
- MineLevel the player is currently on
- Anniversary of the player, split into two tokens, AnniversaryDay and AnniversarySeason
- YearsMarried
- DeathCount
- DeathCountMarried, an extension of DeathCount that tracks how many times a player has died after being married.
- DeathCountPK, (provides a more accurate value than the DeathCount token for use in the PlayerKilled event)
- DeathCountMarriedPK, an extension of DeathCountPK that tracks how many times a player has died after being married.
- PassOutCount, how many times a player has passed out.

### Using Custom Tokens:
- SMAPI must be installed
- Ensure Custom Tokens is listed as a dependency in your content pack
- Tokens used must be prefixed with the mod's unique ID e.g TheMightyAmondee.CustomTokens/MineLevel
- Many tokens require an update rate faster than CP's default as they can change throughout the day.
- A per-save JSON file will be generated after the day is started for each save so the mod can track values not tracked by the game. These can be adjusted as needed as they will have an initial value of 0, which may not be accurate for older saves.


### Config:
- AllowDebugging adds a single debug command so the values of the tokens can be viewed in the SMAPI console. When enabled typing "tracker" in the console will display a list of token values
- ResetDeathCountMarriedWhenDivorced will cause the DeathCountMarried token to reset to 0 when divorced, default is true.

### Tokens in more detail:
Token | default value | What it tracks | Notes
----- | ------------- | -------------- | --------
Minelevel | 0 | Players current minelevel | Add 120 to Skull Cavern floors for token value
AnniversaryDay | 0 | The day the player was married on
AnniversarySeason | No season | The season the player was married in | Value is in all lower-case
DeathCount | 0 | The number of deaths
DeathCountPK | 1 | Value is DeathCount + 1 |Because there are limits on the update rate of tokens in CP, this token can be used as a more accurate snapshot of DeathCount in some cases, mainly the PlayerKilled event
DeathCountMarried | 0 | The number of deaths that occur when the player is married
DeathCountMarriedPK | 1 | Value is DeathCountMarried + 1 |Because there are limits on the update rate of tokens in CP, this token can be used as a more accurate snapshot of DeathCountMarried in some cases, mainly the PlayerKilled event
PassOutCount | 0 | The number of times the player has passed out, either from exhaustion or it reaching 2AM

