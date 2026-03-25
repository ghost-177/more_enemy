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
    // 「凛冬之气」— 雪童子永久被动SE（挂在雪童子自身上）
    // 效果：每当玩家回合开始时，对玩家施加1层虚弱
    // ============================================================

    public sealed class YukidoujiWinterBreathSEDef : EternalWinterSeTemplate
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

    [EntityLogic(typeof(YukidoujiWinterBreathSEDef))]
    public sealed class YukidoujiWinterBreathSE : StatusEffect
    {
        protected override void OnAdded(Unit unit)
        {
            ReactOwnerEvent(Battle.Player.TurnStarted, OnPlayerTurnStarted);
        }

        private IEnumerable<BattleAction> OnPlayerTurnStarted(UnitEventArgs args)
        {
            if (Owner != null)
            {
                // 对玩家施加1层虚弱（HasDuration=true，duration=1，startAutoDecreasing=false）
                yield return new ApplyStatusEffectAction(
                    typeof(Weak), Battle.Player, null, 1, null, null, 0f, false);
            }
        }
    }
}
