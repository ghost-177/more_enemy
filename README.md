## Sample character mod

A sample character mod for Touhou: Lost Branch of Legend.

This includes a character, a custom model, various cards examples and several classes to simplify the moding process.

For more examples and beginner tutorials, check:

* [General tips/guidelines on how to make a mod](https://gitlab.com/rmrfmaxxc1/lbol_sample_character_mod/-/blob/main/TIPS.md)
* [Sideloader on github](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/tree/master)
* [Sideloader: Making your first card](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/MyFirstCard.md)
* [Sideloader: Class parameters](https://github.com/Neoshrimp/LBoL-Entity-Sideloader/blob/master/src/LBoL-Entity-Sideloader/EntityReference.md)

### Installation 

The installation and building process is detailed [here](https://gitlab.com/rmrfmaxxc1/lbol_sample_character_mod/-/wikis/installation).

### Options

By default, the model used for the player is "Youmu". This can be changed in BepinexPlugin.cs

| Parameter | Type | Default | Description |
| --- | --- | --- | --- |
| modUniqueID | string | SampleCharacterMod | A unique ID used to define the character, the card's owner and load the model. **WARNING**: If you are planning to use this mod as a base, for another character, rename this parameter. If the same ID is shared, the characters will share the same card pool and the model will most likely be bugged.|
| playerName | string | SampleCharacter | Prefix of the .png files in DirResource (i.e. SampleCharacterAvatar.png) |
| useInGameModel| bool | true | Whether to use an in-game model (default) or a custom one, the custom one is located in DirResource/SampleCharacterModel.png |
| modelName | string | Youmu | Name of the in-game model to load. |
| modelIsFlipped | bool | true | Whether the model needs to be flipped.|
| offColors | List<ManaColor>() | { ManaColor.Colorless } | The character's non-main colors. This is solely used to automatically put non-main colors cards at the bottom of the cards collection.|

### Quickstart

If you intend to use this project as a base for a character, use Crtl+Shift+h in visual studio code to rename every instance of "SampleCharacter" to the name of your character. The names of the files in DirResources / Resources will have to be updated accordingly.

Update the PInfo.cs and manifest.json files with the appropriate informations (link to the mod, name of the mod, description...)

When the mod is complete, delete the demo cards/status effects to not bloat the character's card pool.

#### Duplicate IDs
Beware of duplicate IDs. If you intend to make a custom card or character, make sure that the name of the cards/status effects don't conflict with any in-game ID. For instance, do not name any class "KokoroAttack" or "NitoriUnderwater" as they are already taken by the cards "Melancholy Eruption" and "Water Blanket" respectively. 

If this happens, the modded card will not be loaded into the game. To avoid this situation add a unique prefix, like "PlayableNitori" or "NitoriCharacter" to avoid the issue. Please note that this may still conflict with mods that implement the same characters and use a similar prefix. However, the odds of it happening are very slim. 