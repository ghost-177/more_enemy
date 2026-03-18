using System.Reflection;
using HarmonyLib;
using LBoL.Core;
using LBoL.Core.Randoms;


namespace EternalWinterMod.Enemies.Act1
{
    /// <summary>
    /// 将第一幕敌人注入 Stage 的 EnemyPool / EliteEnemyPool / BossPool。
    /// 在 Stage.Initialize Postfix 中执行，只对 Level == 1 的幕注入。
    /// </summary>
    [HarmonyPatch]
    internal static class Act1EnemyPoolPatch
    {
        [HarmonyTargetMethod]
        static MethodBase TargetMethod() =>
            AccessTools.Method(typeof(Stage), "Initialize");

        [HarmonyPostfix]
        static void Postfix(Stage __instance)
        {
            if (__instance.Level != 1)
                return;

            // ---- 普通敌人池（Act1 专用） ----
            // ID = 类名去掉末尾 "Def"，例如 YukidoujiGroupDef → "YukidoujiGroup"
            var normalPool = __instance.EnemyPoolAct1;
            normalPool?.Add("YukidoujiEWGroup",    1f);
            normalPool?.Add("MaiyoTsukiEWGroup",   1f);
            normalPool?.Add("MeikaiChoreiEWGroup", 1f);

            // ---- 精英池 ----
            var elitePool = __instance.EliteEnemyPool;
            elitePool?.Add("DaiyouseiEWGroup", 1f);
            elitePool?.Add("KaguyaEWGroup",    1f);

            // ---- Boss 池 ----
            var bossPool = __instance.BossPool;
            bossPool?.Add("LettyEWGroup", 1f);
        }
    }
}
