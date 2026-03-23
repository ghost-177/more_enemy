using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using EternalWinterMod.Enemies.Template;
using EternalWinterMod.GunName;


namespace EternalWinterMod.Enemies.Act3
{
    public sealed class YuyukoEliteEWDef : SampleCharacterEnemyUnitTemplate
    {
        public override EnemyUnitConfig MakeConfig()
        {
            EnemyUnitConfig config = GetEnemyUnitDefaultConfig();
            config.Type = EnemyType.Elite;

            config.MaxHp = 160;
            config.MaxHpHard = 175;
            config.MaxHpLunatic = 190;

            // 幽灵蝶
            config.Damage1 = 16;
            config.Damage1Hard = 18;
            config.Damage1Lunatic = 20;

            // 死亡领域
            config.Damage2 = 12;
            config.Damage2Hard = 13;
            config.Damage2Lunatic = 15;

            config.Defend = 14;
            config.DefendHard = 16;
            config.DefendLunatic = 18;

            config.PowerLoot = new MinMax(75, 100);
            config.BluePointLoot = new MinMax(45, 65);

            config.Gun1 = new List<string> { "Instant" };
            config.Gun2 = new List<string> { "Instant" };

            return config;
        }
    }
}
