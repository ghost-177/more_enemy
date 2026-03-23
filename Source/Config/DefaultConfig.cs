using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader.Entities;
using EternalWinterMod.GunName;
using UnityEngine;
using static LBoLEntitySideloader.Entities.EnemyGroupTemplate;

namespace EternalWinterMod.Config
{
    public sealed class SampleCharacterDefaultConfig
    {
        private static readonly string OwnerName = BepinexPlugin.modUniqueID;

        public static string DefaultID(EntityDefinition entity)
        {
            string IDdef = entity.GetType().Name;
            // 去掉末尾 "Def" 得到 ID
            string ID = IDdef.Remove(IDdef.Length - 3);
            return ID;
        }

        public static CardConfig CardDefaultConfig()
        {
            return new CardConfig(
                Index: 0,
                Id: "",
                Order: 10,
                AutoPerform: true,
                Perform: new string[0][],
                GunName: "",
                GunNameBurst: "",
                DebugLevel: 0,
                Revealable: false,
                IsPooled: false,
                FindInBattle: false,
                HideMesuem: true,
                IsUpgradable: false,
                Rarity: Rarity.Common,
                Type: CardType.Status,
                TargetType: null,
                Colors: new List<ManaColor>() { ManaColor.Colorless },
                IsXCost: false,
                Cost: default(ManaGroup),
                UpgradedCost: null,
                Kicker: null,
                UpgradedKicker: null,
                MoneyCost: null,
                Damage: null,
                UpgradedDamage: null,
                Block: null,
                UpgradedBlock: null,
                Shield: null,
                UpgradedShield: null,
                Value1: null,
                UpgradedValue1: null,
                Value2: null,
                UpgradedValue2: null,
                Mana: null,
                UpgradedMana: null,
                Scry: null,
                UpgradedScry: null,
                ToolPlayableTimes: null,
                Loyalty: null,
                UpgradedLoyalty: null,
                PassiveCost: null,
                UpgradedPassiveCost: null,
                ActiveCost: null,
                UpgradedActiveCost: null,
                ActiveCost2: null,
                UpgradedActiveCost2: null,
                UltimateCost: null,
                UpgradedUltimateCost: null,
                Keywords: Keyword.None,
                UpgradedKeywords: Keyword.None,
                EmptyDescription: false,
                RelativeKeyword: Keyword.None,
                UpgradedRelativeKeyword: Keyword.None,
                RelativeEffects: new List<string>(),
                UpgradedRelativeEffects: new List<string>(),
                RelativeCards: new List<string>(),
                UpgradedRelativeCards: new List<string>(),
                Owner: OwnerName,
                ImageId: "",
                UpgradeImageId: "",
                Unfinished: false,
                Illustrator: null,
                Pack: "",
                SubIllustrator: new List<string>());
        }

        public static StatusEffectConfig DefaultStatusEffectConfig()
        {
            return new StatusEffectConfig(
                Id: "",
                ImageId: null,
                Index: 0,
                Order: 10,
                Type: StatusEffectType.Negative,
                IsVerbose: false,
                IsStackable: true,
                StackActionTriggerLevel: null,
                HasLevel: true,
                LevelStackType: StackType.Add,
                HasDuration: false,
                DurationStackType: StackType.Add,
                DurationDecreaseTiming: DurationDecreaseTiming.Custom,
                HasCount: false,
                CountStackType: StackType.Keep,
                LimitStackType: StackType.Keep,
                ShowPlusByLimit: false,
                Keywords: Keyword.None,
                RelativeEffects: new List<string>() {},
                VFX: "Default",
                VFXloop: "Default",
                SFX: "Default"
            );
        }

        public static EnemyUnitConfig EnemyUnitDefaultConfig()
        {
            return new EnemyUnitConfig(
                Id: "",
                RealName: false,
                OnlyLore: false,
                BaseManaColor: new List<LBoL.Base.ManaColor>() { ManaColor.Colorless },
                Order: 10,
                ModleName: "",
                NarrativeColor: "#ffff",
                Type: EnemyType.Boss,
                IsPreludeOpponent: false,
                HpLength: null,
                MaxHpAdd: null,
                MaxHp: 100,
                Damage1: 10,
                Damage2: 10,
                Damage3: 10,
                Damage4: 10,
                Power: 1,
                Defend: 10,
                Count1: 1,
                Count2: 1,
                MaxHpHard: 100,
                Damage1Hard: 10,
                Damage2Hard: 10,
                Damage3Hard: 10,
                Damage4Hard: 10,
                PowerHard: 1,
                DefendHard: 10,
                Count1Hard: 1,
                Count2Hard: 1,
                MaxHpLunatic: 100,
                Damage1Lunatic: 10,
                Damage2Lunatic: 10,
                Damage3Lunatic: 10,
                Damage4Lunatic: 10,
                PowerLunatic: 1,
                DefendLunatic: 10,
                Count1Lunatic: 1,
                Count2Lunatic: 1,
                PowerLoot: new MinMax(0, 0),
                BluePointLoot: new MinMax(0, 0),
                Gun1: new List<string> { "Instant" },
                Gun2: new List<string> { "Instant" },
                Gun3: new List<string> { "Instant" },
                Gun4: new List<string> { "Instant" }
            );
        }

        public static EnemyGroupConfig EnemyGroupDefaultConfig()
        {
            return new EnemyGroupConfig(
                Id: "",
                Hidden: false,
                Environment: null,
                IsSub: false,
                Subs: new List<string>() { },
                Name: "",
                FormationName: VanillaFormations.Single,
                Enemies: new List<string>() { },
                EnemyType: EnemyType.Normal,
                DebutTime: 1f,
                RollBossExhibit: false,
                PlayerRoot: new Vector2(-4f, 0.5f),
                PreBattleDialogName: "",
                PostBattleDialogName: ""
            );
        }
    }
}
