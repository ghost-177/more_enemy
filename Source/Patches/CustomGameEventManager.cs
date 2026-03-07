using HarmonyLib;
using LBoL.Core;
using SampleCharacterMod.BattleActions;

namespace SampleCharacterMod.Patches
{
    [HarmonyPatch]
    class CustomGameEventManager
    {
        static public GameEvent<BuffAttackEventArgs> PreCustomEvent { get; set;}
        static public GameEvent<BuffAttackEventArgs> PostCustomEvent { get; set; }

        [HarmonyPatch(typeof(GameRunController), nameof(GameRunController.EnterBattle))]
        private static bool Prefix(GameRunController __instance)
        {
            PreCustomEvent = new GameEvent<BuffAttackEventArgs>();
            PostCustomEvent = new GameEvent<BuffAttackEventArgs>();
            return true;
        }
    }
}