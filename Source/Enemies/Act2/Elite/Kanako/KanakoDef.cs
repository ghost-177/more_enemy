using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using EternalWinterMod.Enemies.Template;
using EternalWinterMod.GunName;


namespace EternalWinterMod.Enemies.Act2
{
    public sealed class KanakoDef : SampleCharacterEnemyUnitTemplate
    {
        public override EnemyUnitConfig MakeConfig()
        {
            EnemyUnitConfig config = GetEnemyUnitDefaultConfig();
            config.Type = EnemyType.Elite;

            config.MaxHp = 150;
            config.MaxHpHard = 165;
            config.MaxHpLunatic = 180;

            // 神风攻击
            config.Damage1 = 17;
            config.Damage1Hard = 19;
            config.Damage1Lunatic = 21;

            // 御柱
            config.Damage2 = 12;
            config.Damage2Hard = 13;
            config.Damage2Lunatic = 15;

            config.Defend = 12;
            config.DefendHard = 13;
            config.DefendLunatic = 15;

            config.PowerLoot = new MinMax(75, 100);
            config.BluePointLoot = new MinMax(40, 60);

            config.Gun1 = new List<string> { GunNameID.GetGunFromId(800) };
            config.Gun2 = new List<string> { GunNameID.GetGunFromId(800) };

            return config;
        }
    }
}
