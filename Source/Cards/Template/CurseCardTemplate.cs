using LBoL.ConfigData;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using EternalWinterMod.Config;
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

        // 暂无美术资源，返回 null 使用占位图
        public override CardImages LoadCardImages() => null;

        protected CardConfig GetDefaultConfig() => SampleCharacterDefaultConfig.CardDefaultConfig();
    }
}
