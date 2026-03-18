using HarmonyLib;

namespace EternalWinterMod
{
    public static class PInfo
    {
        public const string GUID = "touhou.lbol.eternal_winter";
        public const string Name = "EternalWinterEnemyMod";
        public const string version = "0.1.0";
        public static readonly Harmony harmony = new Harmony(GUID);
    }
}
