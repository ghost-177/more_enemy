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

namespace EternalWinterMod.Enemies.Act3
{
    // ============================================================
    // 「幽冥领域」— 西行寺幽幽子被动SE（挂在幽幽子自身上）
    // 效果：每回合开始时自动获得10护盾（幽冥之气的保护）
    // ============================================================

    public sealed class YuyukoNetherworldRealmSEDef : EternalWinterSeTemplate
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

    [EntityLogic(typeof(YuyukoNetherworldRealmSEDef))]
    public sealed class YuyukoNetherworldRealmSE : StatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            ReactOwnerEvent(Owner.TurnStarted, OnOwnerTurnStarted);
        }

        private IEnumerable<BattleAction> OnOwnerTurnStarted(UnitEventArgs args)
        {
            if (Owner != null)
            {
                // 每回合开始时获得10护盾（护盾=Shield，格挡=Block，这里用Shield param）
                yield return new CastBlockShieldAction(Owner, 0, 10, BlockShieldType.Normal, true);
            }
        }
    }
}
