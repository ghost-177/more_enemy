using LBoL.ConfigData;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using EternalWinterMod.Config;
using EternalWinterMod.Localization;
using UnityEngine;

namespace EternalWinterMod.StatusEffects.Template
{
    /// <summary>
    /// 永冬异变状态效果模板基类。
    /// </summary>
    public abstract class EternalWinterSeTemplate : StatusEffectTemplate
    {
        public override IdContainer GetId() => SampleCharacterDefaultConfig.DefaultID(this);

        public override LocalizationOption LoadLocalization()
            => SampleCharacterLocalization.StatusEffectsBatchLoc.AddEntity(this);

        // 暂无图标，返回 null 使用默认占位
        public override Sprite LoadSprite() => null;

        protected StatusEffectConfig GetDefaultConfig() => SampleCharacterDefaultConfig.DefaultStatusEffectConfig();
    }
}
