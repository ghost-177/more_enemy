using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using EternalWinterMod.Enemies.Template;
using EternalWinterMod.GunName;


namespace EternalWinterMod.Enemies.Act2
{
    public sealed class MomijiEWDef : SampleCharacterEnemyUnitTemplate
    {
        public override EnemyUnitConfig MakeConfig()
        {
            EnemyUnitConfig config = GetEnemyUnitDefaultConfig();
            config.Type = EnemyType.Elite;

            config.MaxHp = 130;
            config.MaxHpHard = 145;
            config.MaxHpLunatic = 160;

            // 刀击
            config.Damage1 = 15;
            config.Damage1Hard = 17;
            config.Damage1Lunatic = 19;

            // 速斩（×2 连击）
            config.Damage2 = 10;
            config.Damage2Hard = 11;
            config.Damage2Lunatic = 13;

            config.Count2 = 2;
            config.Count2Hard = 2;
            config.Count2Lunatic = 2;

            config.Defend = 16;
            config.DefendHard = 18;
            config.DefendLunatic = 20;

            config.PowerLoot = new MinMax(70, 95);
            config.BluePointLoot = new MinMax(40, 60);

            config.Gun1 = new List<string> { GunNameID.GetGunFromId(800) };
            config.Gun2 = new List<string> { GunNameID.GetGunFromId(800) };

            return config;
        }
    }
}
