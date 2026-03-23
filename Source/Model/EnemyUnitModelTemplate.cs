using Cysharp.Threading.Tasks;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using UnityEngine;
using EternalWinterMod.Localization;
using LBoL.Presentation;

namespace EternalWinterMod.Model
{
    /// <summary>
    /// 敌人模型模板基类。
    /// 每个敌人继承此类，重写 VanillaModelName 属性返回对应的原版 Spine 动画名。
    /// GetId() 返回的 ID 必须与 EnemyUnitConfig.ModleName 一致（ModleName="" 时 sideloader 自动用实体 ID）。
    /// </summary>
    public abstract class EWEnemyUnitModelTemplate : UnitModelTemplate
    {
        /// <summary>原版 Spine 动画名（LBoL.EntityLib.EnemyUnits 各类名），用于加载游戏内已有骨骼动画。</summary>
        public abstract string VanillaModelName { get; }

        public override LocalizationOption LoadLocalization()
        {
            return SampleCharacterLocalization.UnitModelBatchLoc.AddEntity(this);
        }

        public override ModelOption LoadModelOptions()
        {
            return new ModelOption(ResourcesHelper.LoadSpineUnitAsync(VanillaModelName));
        }

        public override UniTask<Sprite> LoadSpellSprite()
        {
            return ResourcesHelper.LoadSpellPortraitAsync(VanillaModelName);
        }

        public override UnitModelConfig MakeConfig()
        {
            // 尝试从原版模型复制布局参数；若该名称不在 UnitModelConfig 表中则使用默认值
            UnitModelConfig vanillaConfig = UnitModelConfig.FromName(VanillaModelName);
            UnitModelConfig config = (vanillaConfig != null) ? vanillaConfig : DefaultConfig();
            config.Flip = true;
            return config;
        }
    }
}
