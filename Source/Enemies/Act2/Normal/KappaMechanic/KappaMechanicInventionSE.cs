using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;
using EternalWinterMod.StatusEffects.Template;
using UnityEngine;

namespace EternalWinterMod.Enemies.Act2
{
    // ============================================================
    // 「奇技发明」— 激进河童技师被动SE（挂在河童技师自身上）
    // 效果：战斗开始时向玩家弃牌堆加入2张「故障机关」厄运牌
    // 实际逻辑在 KappaMechanicEW.OnBattleStarted 中执行
    // 此SE为纯视觉标识，告知玩家该被动已触发
    // ============================================================

    public sealed class KappaMechanicInventionSEDef : EternalWinterSeTemplate
    {
        public override StatusEffectConfig MakeConfig()
        {
            var cfg = GetDefaultConfig();
            cfg.Type = StatusEffectType.Special;
            cfg.HasLevel = false;
            cfg.IsStackable = false;
            cfg.HasDuration = false;
            cfg.IsVerbose = false;
            return cfg;
        }

        public override Sprite LoadSprite() => null;
    }

    [EntityLogic(typeof(KappaMechanicInventionSEDef))]
    public sealed class KappaMechanicInventionSE : StatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            // 纯标识SE，无持续效果，逻辑在敌人类的OnBattleStarted里执行
        }
    }
}
