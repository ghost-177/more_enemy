using System.Collections.Generic;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.StatusEffects;
using LBoL.Core.Units;
using LBoLEntitySideloader.Attributes;
using EternalWinterMod.StatusEffects.Template;
using UnityEngine;

namespace EternalWinterMod.Enemies.Act1
{
    // ============================================================
    // 「水晶冰盾」— 大妖精永久被动SE（挂在大妖精自身上）
    // 效果：每回合结束时自动获得8格挡（冰雪妖精本能凝结的防护层）
    // ============================================================

    public sealed class DaiyouseiCrystalShieldSEDef : EternalWinterSeTemplate
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

    [EntityLogic(typeof(DaiyouseiCrystalShieldSEDef))]
    public sealed class DaiyouseiCrystalShieldSE : StatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            ReactOwnerEvent(Owner.TurnEnded, OnOwnerTurnEnded);
        }

        private IEnumerable<BattleAction> OnOwnerTurnEnded(UnitEventArgs args)
        {
            if (Owner != null)
            {
                // 每回合结束时自动获得8格挡
                yield return new CastBlockShieldAction(Owner, 8, 0, BlockShieldType.Normal, true);
            }
        }
    }
}
