using LBoL.ConfigData;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using EternalWinterMod.Config;
using EternalWinterMod.ImageLoader;
using EternalWinterMod.Localization;

namespace EternalWinterMod.Cards.Template
{
    /// <summary>
    /// 永冬异变厄运牌模板基类。
    /// 继承此类后，GetId() 自动截去末尾 "Def" 作为 ID。
    /// </summary>
    public abstract class CurseCardTemplate : CardTemplate
    {
        public override IdContainer GetId() => SampleCharacterDefaultConfig.DefaultID(this);

        public override LocalizationOption LoadLocalization()
            => SampleCharacterLocalization.CardsBatchLoc.AddEntity(this);

        public override CardImages LoadCardImages() => SampleCharacterImageLoader.LoadCardImages(this);

        protected CardConfig GetDefaultConfig() => SampleCharacterDefaultConfig.CardDefaultConfig();
    }
}
