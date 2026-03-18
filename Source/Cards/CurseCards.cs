using System.Collections.Generic;
using System.Linq;
using LBoL.Base;
using LBoL.ConfigData;
using LBoL.Core;
using LBoL.Core.Battle;
using LBoL.Core.Battle.BattleActions;
using LBoL.Core.Cards;
using LBoLEntitySideloader.Attributes;
using EternalWinterMod.Cards.Template;
using EternalWinterMod.StatusEffects;

namespace EternalWinterMod.Cards
{
    // ============================================================
    // 1. 寒冰晶 (FrostCrystal)
    //    抽到时消耗1点法力
    //    不自动放逐，停留在手牌直到被弃置
    // ============================================================

    public sealed class FrostCrystalDef : CurseCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            var cfg = GetDefaultConfig();
            cfg.Index = 30001;
            cfg.Keywords = Keyword.Exile;
            return cfg;
        }
    }

    [EntityLogic(typeof(FrostCrystalDef))]
    public sealed class FrostCrystal : Card
    {
        public override IEnumerable<BattleAction> OnDraw()
        {
            yield return new LoseTurnManaAction(new ManaGroup() { Any = 1 });
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield break;
        }
    }

    // ============================================================
    // 2. 故障机关 (BrokenDevice)
    //    抽到时随机丢弃手中1张牌，然后自身放逐
    // ============================================================

    public sealed class BrokenDeviceDef : CurseCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            var cfg = GetDefaultConfig();
            cfg.Index = 30002;
            return cfg;
        }
    }

    [EntityLogic(typeof(BrokenDeviceDef))]
    public sealed class BrokenDevice : Card
    {
        public override IEnumerable<BattleAction> OnDraw()
        {
            var targets = Battle.HandZone.Where(c => c != this).ToList();
            if (targets.Count > 0)
            {
                int idx = (int)(Battle.GameRun.BattleRng.NextDouble() * targets.Count);
                yield return new DiscardAction(targets[idx]);
            }
            yield return new ExileCardAction(this);
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield break;
        }
    }

    // ============================================================
    // 3. 神罚令 (DivinePunishment)
    //    临时牌（飘忽，回合结束自动放逐）
    //    打出时消耗1点法力
    // ============================================================

    public sealed class DivinePunishmentDef : CurseCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            var cfg = GetDefaultConfig();
            cfg.Index = 30003;
            cfg.Keywords = Keyword.Ethereal;
            return cfg;
        }
    }

    [EntityLogic(typeof(DivinePunishmentDef))]
    public sealed class DivinePunishment : Card
    {
        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield return new LoseTurnManaAction(new ManaGroup() { Any = 1 });
        }
    }

    // ============================================================
    // 4. 号外报道 (ExtraNewsReport)
    //    抽到时立即失去 4 HP（穿透格挡），然后放逐
    // ============================================================

    public sealed class ExtraNewsReportDef : CurseCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            var cfg = GetDefaultConfig();
            cfg.Index = 30004;
            cfg.Value1 = 4;
            return cfg;
        }
    }

    [EntityLogic(typeof(ExtraNewsReportDef))]
    public sealed class ExtraNewsReport : Card
    {
        public override IEnumerable<BattleAction> OnDraw()
        {
            yield return new DamageAction(
                Battle.Player, Battle.Player,
                DamageInfo.HpLose(Value1, false),
                "", GunType.Single);
            yield return new ExileCardAction(this);
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield break;
        }
    }

    // ============================================================
    // 5. 冥判书 (NetherworldVerdict)
    //    持有时每回合结束失去 2 HP（穿透格挡）
    //    通过 NetherworldVerdictSe 实现持续效果
    // ============================================================

    public sealed class NetherworldVerdictDef : CurseCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            var cfg = GetDefaultConfig();
            cfg.Index = 30005;
            cfg.RelativeEffects = new System.Collections.Generic.List<string>
                { nameof(NetherworldVerdictSe) };
            return cfg;
        }
    }

    [EntityLogic(typeof(NetherworldVerdictDef))]
    public sealed class NetherworldVerdict : Card
    {
        public override IEnumerable<BattleAction> OnDraw()
        {
            yield return new ApplyStatusEffectAction(
                typeof(NetherworldVerdictSe), Battle.Player, 1, null, null, null, 0f, false);
        }

        public override void OnLeaveHand()
        {
            RemoveVerdictSe();
        }

        private void RemoveVerdictSe()
        {
            var se = Battle.Player.GetStatusEffect<NetherworldVerdictSe>();
            if (se != null && se.Level > 0)
            {
                React(new RemoveStatusEffectAction(se, false, 0f));
            }
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield break;
        }
    }

    // ============================================================
    // 6. 彼岸花 (HiganFlower)
    //    抽到时受 8 穿透伤害，然后放逐
    // ============================================================

    public sealed class HiganFlowerDef : CurseCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            var cfg = GetDefaultConfig();
            cfg.Index = 30006;
            cfg.Value1 = 8;
            return cfg;
        }
    }

    [EntityLogic(typeof(HiganFlowerDef))]
    public sealed class HiganFlower : Card
    {
        public override IEnumerable<BattleAction> OnDraw()
        {
            yield return new DamageAction(
                Battle.Player, Battle.Player,
                DamageInfo.HpLose(Value1, false),
                "", GunType.Single);
            yield return new ExileCardAction(this);
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield break;
        }
    }

    // ============================================================
    // 7. 幽冥令 (NetherworldDecree)
    //    持有时每打出1张牌额外消耗1点法力
    //    通过 NetherworldDecreeSe 实现持续效果
    // ============================================================

    public sealed class NetherworldDecreeDef : CurseCardTemplate
    {
        public override CardConfig MakeConfig()
        {
            var cfg = GetDefaultConfig();
            cfg.Index = 30007;
            cfg.RelativeEffects = new System.Collections.Generic.List<string>
                { nameof(NetherworldDecreeSe) };
            return cfg;
        }
    }

    [EntityLogic(typeof(NetherworldDecreeDef))]
    public sealed class NetherworldDecree : Card
    {
        public override IEnumerable<BattleAction> OnDraw()
        {
            yield return new ApplyStatusEffectAction(
                typeof(NetherworldDecreeSe), Battle.Player, 1, null, null, null, 0f, false);
        }

        public override void OnLeaveHand()
        {
            RemoveDecreeSe();
        }

        private void RemoveDecreeSe()
        {
            var se = Battle.Player.GetStatusEffect<NetherworldDecreeSe>();
            if (se != null && se.Level > 0)
            {
                React(new RemoveStatusEffectAction(se, false, 0f));
            }
        }

        protected override IEnumerable<BattleAction> Actions(
            UnitSelector selector, ManaGroup consumingMana, Interaction precondition)
        {
            yield break;
        }
    }
}
