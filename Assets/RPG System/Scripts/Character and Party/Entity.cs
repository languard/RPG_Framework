using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


//Both monsters and Characters use the Entity class
[System.Serializable]
public class Entity : ScriptableObject
{

    public string entityName;
    public string combatID;

    public int strength;
    public int strengthBase;
    public int constitution;
    public int constitutionBase;
    public int willpower;
    public int willpowerBase;
    public int intelligence;
    public int intelligenceBase;

    public int hitPoints;
    public int hitPointsBase;

    public int mana;
    public int manaBase;

    public int endurance;
    public int enduranceBase;

    public int reaction;
    public int reactionBase;

    public int entityLevel;     //can be used as a scale value.  For example, level 5 monster vs level 10 monster, or a spell that adds 3 levels to a player.

    public int physicalArmor;
    public int physicalArmorBase;
    public int magicArmor;
    public int magicArmorBase;

    public PlayerSkills playerSkills;
    public EnemySkills enemySkills;


    //property used by combat system
    public bool isDead
    {
        get
        {
            if (hitPoints > 0) return false;
            else return true;
        }
    }
    public bool isDisabled
    {
        get
        {
            if (hitPoints > 0) return false;
            else return true;
        }
    }
      


    public Entity()
    {
        strength = 1;
        strengthBase = 1;
        constitution = 1;
        constitutionBase = 1;
        willpower = 1;
        willpowerBase = 1;
        intelligence = 1;
        intelligenceBase = 1;

        CalculateAllStats();

        entityLevel = 1;

        entityName = "DEFAULT";
    }

    public void CalculateAllStats()
    {
        CalculateEndurance();
        CalculateHP();
        CalculateMana();
        CalculateReaction();
    }

    public void CalculateHP()
    {
        hitPointsBase = (int)(StaticData.HP_BASE + (StaticData.HP_CON_SCALE * constitution + StaticData.HP_STR_SCALE * strength) * StaticData.HP_STAT_SCALE);
        if (hitPoints > hitPointsBase || hitPoints <= 0) hitPoints = hitPointsBase;
    }

    public void CalculateMana()
    {
        manaBase = (int)(StaticData.M_BASE + (StaticData.M_INT_SCALE * intelligence + StaticData.M_WIL_SCALE * willpower) * StaticData.M_STAT_SCALE);
        if (mana > manaBase || mana <= 0) mana = manaBase;
    }

    public void CalculateEndurance()
    {
        enduranceBase = (int)(StaticData.END_BASE + (StaticData.END_CON_SCALE * constitution + StaticData.END_WIL_SCALE * willpower) * StaticData.END_STAT_SCALE);
        if (endurance > enduranceBase || endurance <= 0) endurance = enduranceBase;
    }

    public void CalculateReaction()
    {
        reactionBase = (int)(StaticData.R_BASE + (StaticData.R_INT_SCALE * intelligence + StaticData.R_STR_SCALE * strength) * StaticData.R_STAT_SCALE);
        reaction = reactionBase;
    }

    public void ResetStatsToBase()
    {
        strength = strengthBase;
        constitution = constitutionBase;
        willpower = willpowerBase;
        intelligence = intelligenceBase;
        physicalArmor = physicalArmorBase;
        magicArmor = magicArmorBase;
    }

    public void FullHeal()
    {
        hitPoints = hitPointsBase;
        endurance = enduranceBase;
        mana = manaBase;
    }

    public void HealHP(int amount)
    {
        hitPoints += amount;
        if (hitPoints > hitPointsBase) hitPoints = hitPointsBase;
    }

    public int GetHitPointsBase()
    {
        return hitPointsBase;
    }

}

