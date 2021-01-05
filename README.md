
Custom Tokens is a mod that provides some basic additional tokens for Content Patcher, extending what can be done

### Custom Tokens registers the following tokens:
- MineLevel the player is currently on
- Anniversary of the player, split into the day and season
- Years Married
- DeathCount
- An extension of DeathCount that tracks how many times a player has died after being married. 

### Using Custom Tokens:
- SMAPI must be installed
- Ensure Custom Tokens is listed as a dependency in your content pack
- Tokens used must be prefixed with the mod's unique ID e.g TheMightyAmondee.CustomTokens/MineLevel


### Config:
- AllowDebugging adds a single debug command so the values of the tokens can be viewed in the SMAPI console. When enabled typing "tracker" in the console will display a list of token values
- ResetDeathCountMarriedWhenDivorced will cause the DeathCountMarried token to reset to 0 when divorced, default is true
