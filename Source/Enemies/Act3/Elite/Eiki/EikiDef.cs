using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using EternalWinterMod.Enemies.Template;
using EternalWinterMod.GunName;


namespace EternalWinterMod.Enemies.Act3
{
    public sealed class EikiEWDef : SampleCharacterEnemyUnitTemplate
    {
        public override EnemyUnitConfig MakeConfig()
        {
            EnemyUnitConfig config = GetEnemyUnitDefaultConfig();
            config.Type = EnemyType.Elite;

            config.MaxHp = 170;
            config.MaxHpHard = 185;
            config.MaxHpLunatic = 200;

            // 审判
            config.Damage1 = 18;
            config.Damage1Hard = 20;
            config.Damage1Lunatic = 22;

            // 裁决
            config.Damage2 = 14;
            config.Damage2Hard = 15;
            config.Damage2Lunatic = 17;

            config.Defend = 16;
            config.DefendHard = 18;
            config.DefendLunatic = 20;

            config.PowerLoot = new MinMax(80, 105);
            config.BluePointLoot = new MinMax(50, 70);

            config.Gun1 = new List<string> { GunNameID.GetGunFromId(800) };
            config.Gun2 = new List<string> { GunNameID.GetGunFromId(800) };

            return config;
        }
    }
}
