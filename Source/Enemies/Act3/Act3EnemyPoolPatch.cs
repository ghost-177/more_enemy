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

            __instance.EnemyPoolAct3?.Add("HiganbanaWraithEWGroup", 1f);
            __instance.EnemyPoolAct3?.Add("YoumuPhantomEWGroup", 1f);
            __instance.EnemyPoolAct3?.Add("NetherMessengerEWGroup", 1f);

            __instance.EliteEnemyPool?.Add("EikiEWGroup", 1f);
            __instance.EliteEnemyPool?.Add("YuyukoEliteEWGroup", 1f);

            __instance.BossPool?.Add("YuyukoEWGroup", 1f);
        }
    }
}
