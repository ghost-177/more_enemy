using LBoLEntitySideloader.Attributes;
using LBoL.Core.StatusEffects;
using UnityEngine;

namespace SampleCharacterMod.StatusEffects
{
    //Empty status effect that is purely used to define a new pseudo-keyword. 
    //See /DirResources/StatusffectsEn.yaml for the keyword.
    public sealed class SampleCharacterEnhanceSeDef : SampleCharacterStatusEffectTemplate
    {
        //Keywords don't have sprites.
        public override Sprite LoadSprite() => null;

    }

    [EntityLogic(typeof(SampleCharacterEnhanceSeDef))]
    public sealed class SampleCharacterEnhanceSe : StatusEffect
    {
    }
}

