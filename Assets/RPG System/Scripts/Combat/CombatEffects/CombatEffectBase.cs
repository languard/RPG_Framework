using Assets.RPG_System.Scripts.Combat.CalcSpeak;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class CombatEffectBase {

    public enum Effect
    {
        WeaponDamage,
        MagicDamage,
        ManaDrain,
        HealHitPoints
    }

    public Effect effectType { get; private set; }

    public ActorCombatController target { get; private set; }
    public ActorCombatController source { get; private set; }

    public string displayText { get; private set; }
    public string effectExpression { get; private set; }

    public CombatEffectBase(ActorCombatController source, ActorCombatController target, string effectExpression, Effect effectType)
    {
        this.source = source;
        this.target = target;
        this.effectType = effectType;
        this.effectExpression = effectExpression;
    }

    public void SetDisplayText(string text)
    {
        //need this to allow for calculations in children effects
        displayText = text;
    }

    public void Process()
    {
        ConstLoader currentConstants = new ConstLoader(source, target);
        Expression calcExpression = new Expression(effectExpression, currentConstants.values);
        calcExpression.Parse();

        int expressionValue = (int)calcExpression.Evaluate();
        SetDisplayText(expressionValue.ToString());
        
        ApplyEffect(expressionValue);
    }

    public abstract void ApplyEffect(int effectValue);

    public abstract bool IsValid { get; }
}
