using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// A descriptor for all battle-acting entities in the game.
public class Actor : MonoBehaviour {

    public int strength;
    public int constitution;
    public int willpower;
    public int intelligence;

    public int vBase = 100;
    public float vStrScale = 1.0f;
    public float vConScale = 1.0f;
    public float vStatScale = 1.0f;
    public float vLevelScale = 1.0f;

    public int mBase = 100;
    public float mIntScale = 1.0f;
    public float mWillScale = 1.0f;
    public float mStatScale = 1.0f;
    public float mLevelScale = 1.0f;

    public int rBase = 100;
    public float rStrScale = 1.0f;
    public float rIntScale = 1.0f;
    public float rStatScale = 1.0f;
    public float rLevelScale = 1.0f;

    public int endBase = 100;
    public float endConScale = 1.0f;
    public float endWillScale = 1.0f;
    public float endStatScale = 1.0f;
    public float endLevelScale = 1.0f;

    public float classStrScale = 1.0f;
    public float classConScale = 1.0f;
    public float classIntScale = 1.0f;
    public float classWillScale = 1.0f;

    public int maxVitality
    {
        get
        {
            return (int)(vBase + (strength * vStrScale + constitution * vConScale) * vStatScale + level * vLevelScale);
        }
    }

    public int maxMana
    {
        get
        {
            return (int)(mBase + (intelligence * mIntScale + willpower * mWillScale) * mStatScale + level * mLevelScale);
        }
    }

    public int reaction
    {
        get
        {
            return (int)(rBase + (strength * rStrScale + intelligence * rIntScale) * rStatScale + level * rLevelScale);
        }
    }

    public int maxEndurance
    {
        get
        {
            return (int)(endBase + (constitution * endConScale + willpower * endWillScale) * endStatScale + level * endLevelScale);
        }
    }

    public int level
    {
        get
        {
            return (int)(strength * classStrScale + constitution * classConScale + intelligence * classIntScale + willpower * classWillScale);
        }
    }

    private int _hitPoints;
    public int hitPoints
    {
        get
        {
            return _hitPoints;
        }
        set
        {
            _hitPoints = value;
            if (_hitPoints <= 0) _hitPoints = 0;
            if (_hitPoints >= maxVitality) _hitPoints = maxVitality;
        }
    }

    private int _mana;
    public int mana
    {
        get
        {
            return _mana;
        }
        set
        {
            _mana = value;
            if (_mana <= 0) _mana = 0;
            if (_mana >= maxMana) _mana = maxMana;
        }
    }

    public bool isDead
    {
        get
        {
            return hitPoints <= 0;
        }
    }

    public bool isDisabled
    {
        get
        {
            if (isDead) return true;
            return false;
        }
    }

    void Start()
    {
        hitPoints = maxVitality;
    }
}
