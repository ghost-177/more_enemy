using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using EternalWinterMod.Enemies.Template;
using EternalWinterMod.GunName;


namespace EternalWinterMod.Enemies.Act1
{
    public sealed class KaguyaDef : SampleCharacterEnemyUnitTemplate
    {
        public override EnemyUnitConfig MakeConfig()
        {
            EnemyUnitConfig config = GetEnemyUnitDefaultConfig();
            config.Type = EnemyType.Elite;

            config.MaxHp = 120;
            config.MaxHpHard = 130;
            config.MaxHpLunatic = 140;

            config.Damage1 = 14;
            config.Damage1Hard = 15;
            config.Damage1Lunatic = 17;

            // Damage2 per-hit，使用 Count2 × 2 连击
            config.Damage2 = 9;
            config.Damage2Hard = 10;
            config.Damage2Lunatic = 11;

            config.Count2 = 2;
            config.Count2Hard = 2;
            config.Count2Lunatic = 2;

            config.Defend = 10;
            config.DefendHard = 11;
            config.DefendLunatic = 12;

            config.PowerLoot = new MinMax(65, 90);
            config.BluePointLoot = new MinMax(35, 55);

            config.Gun1 = new List<string> { GunNameID.GetGunFromId(800) };
            config.Gun2 = new List<string> { GunNameID.GetGunFromId(800) };

            return config;
        }
    }
}
