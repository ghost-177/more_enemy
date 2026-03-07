using HarmonyLib;

namespace SampleCharacterMod
{
    public static class PInfo
    {
        //Rename the variable below to prevent conflicts between mod.
        public const string GUID = "author.game.typeofmod.SampleCharacter";
        public const string Name = "SampleCharacterMod";
        public const string version = "0.0.1";
        public static readonly Harmony harmony = new Harmony(GUID);

    }
}
