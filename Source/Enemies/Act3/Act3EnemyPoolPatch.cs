using System.Reflection;
using LBoL.Core;
using HarmonyLib;


namespace EternalWinterMod.Enemies.Act3
{
    [HarmonyPatch]
    internal static class Act3EnemyPoolPatch
    {
        [HarmonyTargetMethod]
        static MethodBase TargetMethod() => AccessTools.Method(typeof(Stage), "Initialize");

        [HarmonyPostfix]
        static void Postfix(Stage __instance)
        {
            if (__instance.Level != 3) return;

            __instance.EnemyPoolAct3?.Add("HiganbanaWraithGroup", 1f);
            __instance.EnemyPoolAct3?.Add("YoumuPhantomGroup", 1f);
            __instance.EnemyPoolAct3?.Add("NetherMessengerGroup", 1f);

            __instance.EliteEnemyPool?.Add("EikiGroup", 1f);
            __instance.EliteEnemyPool?.Add("YuyukoEliteGroup", 1f);

            __instance.BossPool?.Add("YuyukoGroup", 1f);
        }
    }
}
