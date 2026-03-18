using LBoL.Base;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using EternalWinterMod.Config;
using EternalWinterMod.Localization;


namespace EternalWinterMod.Exhibits
{
    public abstract class EternalWinterExhibitTemplate : ExhibitTemplate
    {
        public override IdContainer GetId()
        {
            return SampleCharacterDefaultConfig.DefaultID(this);
        }

        public override LocalizationOption LoadLocalization()
        {
            return SampleCharacterLocalization.ExhibitsBatchLoc.AddEntity(this);
        }

        public override ExhibitSprites LoadSprite()
        {
            // 暂无专属图片，返回空精灵（游戏会显示空白图标）
            return new ExhibitSprites();
        }

        public override ExhibitConfig MakeConfig()
        {
            return GetDefaultExhibitConfig();
        }

        protected ExhibitConfig GetDefaultExhibitConfig()
        {
            return new ExhibitConfig(
                Index: 0,
                Id: "",
                Order: 10,
                IsDebug: false,
                IsPooled: true,
                IsSentinel: false,
                Revealable: false,
                Appearance: AppearanceType.NonShop,
                Owner: BepinexPlugin.modUniqueID,
                LosableType: ExhibitLosableType.DebutLosable,
                Rarity: Rarity.Rare,
                Value1: null,
                Value2: null,
                Value3: null,
                Mana: null,
                BaseManaRequirement: null,
                BaseManaColor: null,
                BaseManaAmount: 1,
                HasCounter: false,
                InitialCounter: null,
                Keywords: Keyword.None,
                RelativeEffects: new System.Collections.Generic.List<string>(),
                RelativeCards: new System.Collections.Generic.List<string>()
            );
        }
    }
}
