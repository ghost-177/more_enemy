using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using EternalWinterMod.Enemies.Template;
using EternalWinterMod.GunName;


namespace EternalWinterMod.Enemies.Act1
{
    public sealed class MaiyoTsukiEWDef : SampleCharacterEnemyUnitTemplate
    {
        public override EnemyUnitConfig MakeConfig()
        {
            EnemyUnitConfig config = GetEnemyUnitDefaultConfig();
            config.Type = EnemyType.Normal;

            config.MaxHp = 28;
            config.MaxHpHard = 31;
            config.MaxHpLunatic = 34;

            config.Damage1 = 5;
            config.Damage1Hard = 6;
            config.Damage1Lunatic = 7;

            // No defend — pure aggression
            config.Defend = 0;
            config.DefendHard = 0;
            config.DefendLunatic = 0;

            config.PowerLoot = new MinMax(20, 35);
            config.BluePointLoot = new MinMax(0, 0);

            config.Gun1 = new List<string> { "Instant" };

            return config;
        }
    }
}
