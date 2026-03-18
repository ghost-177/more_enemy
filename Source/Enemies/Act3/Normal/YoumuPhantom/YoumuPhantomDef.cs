using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using EternalWinterMod.Enemies.Template;
using EternalWinterMod.GunName;


namespace EternalWinterMod.Enemies.Act3
{
    // 魂魄妖梦——精英前置小怪版，较精英版数值偏低
    public sealed class YoumuPhantomEWDef : SampleCharacterEnemyUnitTemplate
    {
        public override EnemyUnitConfig MakeConfig()
        {
            EnemyUnitConfig config = GetEnemyUnitDefaultConfig();
            config.Type = EnemyType.Normal;

            config.MaxHp = 70;
            config.MaxHpHard = 77;
            config.MaxHpLunatic = 85;

            // 人刀击
            config.Damage1 = 14;
            config.Damage1Hard = 15;
            config.Damage1Lunatic = 17;

            // 幽灵半身斩（×2 连击）
            config.Damage2 = 10;
            config.Damage2Hard = 11;
            config.Damage2Lunatic = 13;

            config.Count2 = 2;
            config.Count2Hard = 2;
            config.Count2Lunatic = 2;

            config.Defend = 11;
            config.DefendHard = 12;
            config.DefendLunatic = 14;

            config.PowerLoot = new MinMax(50, 65);
            config.BluePointLoot = new MinMax(0, 0);

            config.Gun1 = new List<string> { GunNameID.GetGunFromId(800) };
            config.Gun2 = new List<string> { GunNameID.GetGunFromId(800) };

            return config;
        }
    }
}
