using System.Reflection;
using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Randoms;


namespace EternalWinterMod.Enemies.Act2
{
    /// <summary>
    /// 将第二幕敌人注入 Stage 的 EnemyPoolAct2 / EliteEnemyPool / BossPool。
    /// 在 Stage.Initialize Postfix 中执行，只对 Level == 2 的幕注入。
    /// </summary>
    [HarmonyPatch]
    internal static class Act2EnemyPoolPatch
    {
        [HarmonyTargetMethod]
        static MethodBase TargetMethod() =>
            AccessTools.Method(typeof(Stage), "Initialize");

        [HarmonyPostfix]
        static void Postfix(Stage __instance)
        {
            if (__instance.Level != 2)
                return;

            // ---- 普通敌人池（Act2 专用） ----
            var normalPool = __instance.EnemyPoolAct2;
            normalPool?.Add("TenguScoutEWGroup",    1f);
            normalPool?.Add("KappaMechanicEWGroup", 1f);
            normalPool?.Add("MountainFairyEWGroup", 1f);

            // ---- 精英池 ----
            var elitePool = __instance.EliteEnemyPool;
            elitePool?.Add("MomijiEWGroup",  1f);
            elitePool?.Add("KanakoEWGroup",  1f);

            // ---- Boss 池 ----
            var bossPool = __instance.BossPool;
            bossPool?.Add("SuwakoEWGroup", 1f);
        }
    }
}
