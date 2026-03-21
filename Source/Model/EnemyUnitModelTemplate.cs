using Cysharp.Threading.Tasks;
using LBoL.ConfigData;
using LBoLEntitySideloader;
using LBoLEntitySideloader.Entities;
using LBoLEntitySideloader.Resource;
using LBoLEntitySideloader.Utils;
using UnityEngine;
using EternalWinterMod.Localization;
using LBoL.Presentation;

namespace EternalWinterMod.Model
{
    /// <summary>
    /// 敌人模型模板基类。
    /// 每个敌人继承此类，重写 VanillaModelName 属性返回对应的原版 Spine 模型名（见 LBoL.EntityLib.EnemyUnits 各类名）。
    /// GetId() 返回的 ID 必须与 EnemyUnitConfig.ModleName 一致。
    /// </summary>
    public abstract class EWEnemyUnitModelTemplate : UnitModelTemplate
    {
        /// <summary>原版 Spine 模型名，用于复用游戏内已有动画。</summary>
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
            UnitModelConfig config = UnitModelConfig.FromName(VanillaModelName).Copy();
            config.Flip = true; // 敌人面朝左（游戏惯例），Flip=true 让 Spine 从右向左
            return config;
        }
    }
}
