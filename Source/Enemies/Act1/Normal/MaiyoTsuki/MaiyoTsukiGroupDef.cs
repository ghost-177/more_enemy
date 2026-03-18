using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using EternalWinterMod.Enemies.Template;


namespace EternalWinterMod.Enemies.Act1
{
    // 迷途月兔 ×2 出场，使用 Triangle 编队（3人编队取前两个槽位）
    public sealed class MaiyoTsukiEWGroupDef : SampleCharacterEnemyGroupTemplate
    {
        public override EnemyGroupConfig MakeConfig()
        {
            EnemyGroupConfig config = GetEnemyGroupDefaultConfig();
            config.FormationName = "Triangle";
            config.Enemies = new List<string>() { "MaiyoTsukiEW", "MaiyoTsukiEW" };
            config.EnemyType = EnemyType.Normal;
            config.RollBossExhibit = false;
            return config;
        }
    }
}
