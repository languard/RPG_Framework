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

    public int[] strength;
    public int[] constitution;
    public int[] willpower;
    public int[] intelligence;

    public int[] hitPoints;
    public int[] mana;
    public int[] endurance;
    public int[] reaction;

    public int entityLevel;     //can be used as a scale value.  For example, level 5 monster vs level 10 monster, or a spell that adds 3 levels to a player.
    

    public Entity()
    {
        strength = new int[] { 1, 1 };
        constitution = new int[] { 1, 1 };
        willpower = new int[] { 1, 1 };
        intelligence = new int[] { 1, 1 };
        hitPoints = new int[] { 1, 1 };
        mana = new int[] { 1, 1 };
        endurance = new int[] { 1, 1 };
        reaction = new int[] { 1, 1 };

        entityLevel = 1;

        name = "DEFAULT";
    }

}

