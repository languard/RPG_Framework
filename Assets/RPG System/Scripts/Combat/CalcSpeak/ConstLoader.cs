using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Assets.RPG_System.Scripts.Combat.CalcSpeak
{
    /// <summary>
    /// Used by expression evaluation for combat effects.
    /// Creates a dictionary of variable values based on the given
    /// source and target. Will be passed to the Expression constructor.
    /// </summary>
    public class ConstLoader
    {
        public Dictionary<string, float> values { get; private set; }

        public ConstLoader(ActorCombatController source, ActorCombatController target)
        {
            values = new Dictionary<string, float>();

            values.Add("SOURCE_STR", source.entity.strength);
            values.Add("SOURCE_CON", source.entity.constitution);
            values.Add("SOURCE_WIL", source.entity.willpower);
            values.Add("SOURCE_INT", source.entity.intelligence);
            values.Add("SOURCE_HP", source.entity.hitPoints);
            values.Add("SOURCE_MP", source.entity.mana);
            values.Add("SOURCE_END", source.entity.endurance);
            values.Add("SOURCE_RCT", source.entity.reaction);
            values.Add("SOURCE_LVL", source.entity.entityLevel);

            values.Add("TARGET_STR", target.entity.strength);
            values.Add("TARGET_CON", target.entity.constitution);
            values.Add("TARGET_WIL", target.entity.willpower);
            values.Add("TARGET_INT", target.entity.intelligence);
            values.Add("TARGET_HP", target.entity.hitPoints);
            values.Add("TARGET_MP", target.entity.mana);
            values.Add("TARGET_END", target.entity.endurance);
            values.Add("TARGET_RCT", target.entity.reaction);
            values.Add("TARGET_LVL", target.entity.entityLevel);
        }
    }
}
