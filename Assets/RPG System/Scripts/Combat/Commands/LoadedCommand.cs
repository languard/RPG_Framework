using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.RPG_System.Scripts.Commands
{
    public class LoadedCommand : CommandBase
    {
        private EffectDescriptor[] effects;

        public LoadedCommand(string commandName, ActorCombatController actor, float delayTime, float staminaCost, float manaCost, Target[] targets, int defaultTargetIndex, bool isRetargetable, EffectDescriptor[] effects)
            : base(commandName, actor, delayTime, staminaCost, manaCost, targets, defaultTargetIndex, isRetargetable)
        {
            this.effects = effects;
        }

        protected override List<CombatEffectBase> OnExecute()
        {
            List<CombatEffectBase> combatEffects = new List<CombatEffectBase>();

            foreach (EffectDescriptor effect in effects)
            {
                switch (effect.effectType)
                {
                    case CombatEffectBase.Effect.HealHitPoints:
                        foreach (ActorCombatController target in targetActors) combatEffects.Add(new HealHitPoints(owner, target, effect.effectExpression));
                        break;
                    case CombatEffectBase.Effect.MagicDamage:
                        foreach (ActorCombatController target in targetActors) combatEffects.Add(new MagicDamage(owner, target, effect.effectExpression));
                        break;
                    case CombatEffectBase.Effect.WeaponDamage:
                        foreach (ActorCombatController target in targetActors) combatEffects.Add(new WeaponDamage(owner, target, effect.effectExpression));
                        break;
                }
            }

            return combatEffects;

        }
    }
}
