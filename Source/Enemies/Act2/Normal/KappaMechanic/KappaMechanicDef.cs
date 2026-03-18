using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using EternalWinterMod.Enemies.Template;
using EternalWinterMod.GunName;


namespace EternalWinterMod.Enemies.Act2
{
    public sealed class KappaMechanicDef : SampleCharacterEnemyUnitTemplate
    {
        public override EnemyUnitConfig MakeConfig()
        {
            EnemyUnitConfig config = GetEnemyUnitDefaultConfig();
            config.Type = EnemyType.Normal;

            config.MaxHp = 60;
            config.MaxHpHard = 65;
            config.MaxHpLunatic = 70;

            // 普通弹射
            config.Damage1 = 8;
            config.Damage1Hard = 9;
            config.Damage1Lunatic = 10;

            // 蓄力炮击
            config.Damage2 = 13;
            config.Damage2Hard = 14;
            config.Damage2Lunatic = 16;

            config.Defend = 0;
            config.DefendHard = 0;
            config.DefendLunatic = 0;

            config.PowerLoot = new MinMax(40, 55);
            config.BluePointLoot = new MinMax(0, 0);

            config.Gun1 = new List<string> { GunNameID.GetGunFromId(800) };
            config.Gun2 = new List<string> { GunNameID.GetGunFromId(800) };

            return config;
        }
    }
}
