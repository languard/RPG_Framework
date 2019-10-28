using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class StaticData
{
    //move direction constants  DO NOT CHANGE
    public const int DIR_UP = 0;
    public const int DIR_LEFT = 1;
    public const int DIR_RIGHT = 2;
    public const int DIR_DOWN = 3;

    //HP Calculation constants
    public const float HP_STR_SCALE = 1;
    public const float HP_CON_SCALE = 1;
    public const float HP_BASE = 1;
    public const float HP_STAT_SCALE = 1;
    public const float HP_CL_SCALE = 1;

    //Mana Calculation constants
    public const float M_INT_SCALE = 1;
    public const float M_WIL_SCALE = 1;
    public const float M_BASE = 1;
    public const float M_STAT_SCALE = 1;
    public const float M_CL_SCALE = 1;

    //Reaction Calculation constants
    public const float R_INT_SCALE = 1;
    public const float R_STR_SCALE = 1;
    public const float R_BASE = 1;
    public const float R_STAT_SCALE = 1;
    public const float R_CL_SCALE = 1;

    //Endurance Calculation constants
    public const float END_CON_SCALE = 1;
    public const float END_WIL_SCALE = 1;
    public const float END_BASE = 1;
    public const float END_STAT_SCALE = 1;
    public const float END_CL_SCALE = 1;

    //combat variables
    public const float DEFAULT_BATTLE_TIME = 5;     //seconds
    public const float REACTION_WEIGHT = 0.1f;      //how much does reaction contribute to speed in battle.     

}

