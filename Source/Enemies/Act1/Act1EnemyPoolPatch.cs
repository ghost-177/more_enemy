using System.Reflection;
using HarmonyLib;
using LBoL.Core;


namespace EternalWinterMod.Enemies.Act1
{
    /// <summary>
    /// 将所有幕的敌人注入 Stage 的各 EnemyPool / EliteEnemyPool / BossPool。
    /// Stage.GetEnemies 根据 station.Act（1/2/3）决定采样哪个 ActPool，
    /// 因此同一个 Stage 对象持有三幕的池，无需按 Level 区分。
    /// </summary>
    [HarmonyPatch]
    internal static class AllEnemyPoolPatch
    {
        [HarmonyTargetMethod]
        static MethodBase TargetMethod() =>
            AccessTools.Method(typeof(Stage), "Initialize");

        [HarmonyPostfix]
        static void Postfix(Stage __instance)
        {
            // ---- 第一幕 普通敌人 ----
            __instance.EnemyPoolAct1?.Add("YukidoujiEWGroup",    1f);
            __instance.EnemyPoolAct1?.Add("MaiyoTsukiEWGroup",   1f);
            __instance.EnemyPoolAct1?.Add("MeikaiChoreiEWGroup", 1f);

            // ---- 第二幕 普通敌人 ----
            __instance.EnemyPoolAct2?.Add("TenguScoutEWGroup",    1f);
            __instance.EnemyPoolAct2?.Add("KappaMechanicEWGroup", 1f);
            __instance.EnemyPoolAct2?.Add("MountainFairyEWGroup", 1f);

            // ---- 第三幕 普通敌人 ----
            __instance.EnemyPoolAct3?.Add("HiganbanaWraithEWGroup", 1f);
            __instance.EnemyPoolAct3?.Add("YoumuPhantomEWGroup",    1f);
            __instance.EnemyPoolAct3?.Add("NetherMessengerEWGroup", 1f);

            // ---- 精英（跨幕共享池） ----
            __instance.EliteEnemyPool?.Add("DaiyouseiEWGroup",  1f);
            __instance.EliteEnemyPool?.Add("KaguyaEWGroup",     1f);
            __instance.EliteEnemyPool?.Add("MomijiEWGroup",     1f);
            __instance.EliteEnemyPool?.Add("KanakoEWGroup",     1f);
            __instance.EliteEnemyPool?.Add("EikiEWGroup",       1f);
            __instance.EliteEnemyPool?.Add("YuyukoEliteEWGroup",1f);

            // ---- Boss（跨幕共享池） ----
            __instance.BossPool?.Add("LettyEWGroup",  1f);
            __instance.BossPool?.Add("SuwakoEWGroup", 1f);
            __instance.BossPool?.Add("YuyukoEWGroup", 1f);
        }
    }
}
