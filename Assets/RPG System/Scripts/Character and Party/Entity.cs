using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


//Both monsters and Characters use the Entity class
[System.Serializable]
public class Entity
{

    public static int CUR = 0;
    public static int BASE = 1;


    public string name;

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

        name = "DEFAULT";
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
        if (hitPoints > hitPointsBase) hitPoints = hitPointsBase;
    }

    public void CalculateMana()
    {
        manaBase = (int)(StaticData.M_BASE + (StaticData.M_INT_SCALE * intelligence + StaticData.M_WIL_SCALE * willpower) * StaticData.M_STAT_SCALE);
        if (mana > manaBase) mana = manaBase;
    }

    public void CalculateEndurance()
    {
        enduranceBase = (int)(StaticData.END_BASE + (StaticData.END_CON_SCALE * constitution + StaticData.END_WIL_SCALE * willpower) * StaticData.END_STAT_SCALE);
        if (endurance > enduranceBase) endurance = enduranceBase;
    }

    public void CalculateReaction()
    {
        reactionBase = (int)(StaticData.R_BASE + (StaticData.R_INT_SCALE * intelligence + StaticData.R_STR_SCALE * strength) * StaticData.R_STAT_SCALE);
        reaction = reactionBase;
    }
}

