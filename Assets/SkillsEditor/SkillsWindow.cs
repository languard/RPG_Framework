using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using Assets.RPG_System.Scripts.Combat.CalcSpeak;

[Serializable]
public class EffectDescriptor
{
    public CombatEffectBase.Effect effectType;
    public string effectExpression;
}

[Serializable]
public class SkillDescriptor
{
    public string name = "New Skill";
    public string displayText = "Enter Display Text";
    public float delay = 2.0f;
    public int manaCost = 0;
    public CommandBase.Target targets = CommandBase.Target.SELECTED_ENEMY | CommandBase.Target.SELECTED_ALLY;
    public CommandBase.Target startTarget = CommandBase.Target.SELECTED_ENEMY;
    public bool isRetargetable = true;
    public EffectDescriptor[] effects;

    public void SetEffects(List<EffectDescriptor> effectsList)
    {
        effects = effectsList.ToArray();
    }

    public void RemoveEffect(EffectDescriptor effect)
    {
        List<EffectDescriptor> effectList = new List<EffectDescriptor>();
        if (effects != null) effectList.AddRange(effects);

        effectList.Remove(effect);
        SetEffects(effectList);
    }

    public void AddEffect(EffectDescriptor effect)
    {
        List<EffectDescriptor> effectList = new List<EffectDescriptor>();
        if (effects != null) effectList.AddRange(effects);

        effectList.Add(effect);
        SetEffects(effectList);
    }
}

public class SkillDatabase
{
    public static string jsonFilePath = "skills.json";
    private static Dictionary<string, SkillDescriptor> skillsByName = new Dictionary<string, SkillDescriptor>();

    public static SkillDescriptor GetSkill(string name)
    {
        if (!skillsByName.ContainsKey(name)) return null;
        return skillsByName[name];
    }

    public static List<string> FindSkills(string likeSearch)
    {
        List<string> skillNames = new List<string>();
        foreach (string name in skillsByName.Keys)
        {
            if (likeSearch.Length == 0 || name.Contains(likeSearch)) skillNames.Add(name);
        }
        skillNames.Sort();
        return skillNames;
    }

    public static void UpdateSkill(SkillDescriptor descriptor)
    {
        if (descriptor == null) throw new ArgumentNullException("descriptor");
        string name = descriptor.name;
        if (!skillsByName.ContainsKey(name))
            skillsByName.Add(name, descriptor);
        else
            skillsByName[name] = descriptor;
    }

    public static void RenameSkill(string oldName, SkillDescriptor currentSkill)
    {
        if (oldName == currentSkill.name)
        {
            skillsByName[oldName] = currentSkill;
            return;
        }

        if (skillsByName.ContainsKey(currentSkill.name))
            skillsByName[currentSkill.name] = currentSkill;
        else
            skillsByName.Add(currentSkill.name, currentSkill);

        skillsByName.Remove(oldName);
    }

    public static bool isLoaded
    {
        get
        {
            return skillsByName.Count > 0;
        }
    }

    public static void LoadSkills()
    {
        string[] jsonLines = System.IO.File.ReadAllLines(jsonFilePath);
        skillsByName.Clear();
        foreach (string line in jsonLines)
        {
            SkillDescriptor skillDescriptor = JsonUtility.FromJson<SkillDescriptor>(line);
            skillsByName.Add(skillDescriptor.name, skillDescriptor);
        }
    }

    public static void SaveSkills()
    {
        List<string> jsonLines = new List<string>();
        foreach (string name in skillsByName.Keys)
        {
            jsonLines.Add(JsonUtility.ToJson(skillsByName[name]));
        }
        System.IO.File.WriteAllLines(jsonFilePath, jsonLines.ToArray());
    }
}

#if UNITY_EDITOR
public class SkillsWindow : EditorWindow
{
    public static SkillsWindow instance;

    private static GUIStyle listItemStyle = new GUIStyle();
    private static GUIStyle selectedItemStyle = new GUIStyle();

    private static Dictionary<string, float> validVariables = new Dictionary<string, float>();

    private void OnEnable()
    {
        validVariables.Add("SOURCE_STR", 0);
        validVariables.Add("SOURCE_CON", 0);
        validVariables.Add("SOURCE_WIL", 0);
        validVariables.Add("SOURCE_INT", 0);
        validVariables.Add("SOURCE_HP", 0);
        validVariables.Add("SOURCE_MP", 0);
        validVariables.Add("SOURCE_END", 0);
        validVariables.Add("SOURCE_RCT", 0);
        validVariables.Add("SOURCE_LVL", 0);

        validVariables.Add("TARGET_STR", 0);
        validVariables.Add("TARGET_CON", 0);
        validVariables.Add("TARGET_WIL", 0);
        validVariables.Add("TARGET_INT", 0);
        validVariables.Add("TARGET_HP", 0);
        validVariables.Add("TARGET_MP", 0);
        validVariables.Add("TARGET_END", 0);
        validVariables.Add("TARGET_RCT", 0);
        validVariables.Add("TARGET_LVL", 0);


        effectDisplayNames.Clear();
        effectsByName.Clear();

        effectDisplayNames.Add(CombatEffectBase.Effect.HealHitPoints, "Heal HP");
        effectDisplayNames.Add(CombatEffectBase.Effect.MagicDamage, "Magic Damage");
        effectDisplayNames.Add(CombatEffectBase.Effect.ManaDrain, "Mana Drain");
        effectDisplayNames.Add(CombatEffectBase.Effect.WeaponDamage, "Weapon Damage");

        foreach (CombatEffectBase.Effect effect in effectDisplayNames.Keys)
        {
            effectsByName.Add(effectDisplayNames[effect], effect);
        }



        SkillDatabase.LoadSkills();
    }

    [MenuItem("Tools/Skills/Skills Editor")]
    static void ShowWindow()
    {
        instance = (SkillsWindow)EditorWindow.GetWindow<SkillsWindow>();
        instance.titleContent = new GUIContent("Skills Editor");
    }

    private string searchText = string.Empty;
    private Vector2 skillListScrollPosition = Vector2.zero;
    private string selectedSkillName = string.Empty;

    private int selectedDefaultIndex = 0;

    private static Dictionary<CombatEffectBase.Effect, string> effectDisplayNames = new Dictionary<CombatEffectBase.Effect, string>();
    private static Dictionary<string, CombatEffectBase.Effect> effectsByName = new Dictionary<string, CombatEffectBase.Effect>();

    private SkillDescriptor selectedSkill;

    private void OnGUI()
    {
        // Display items
        /*
         * "Skills" label at top left
         * Search text box
         * Results box with selectable items, vertical scrolling
         * Right panel
         *  "Name" label and text box
         *  "Display" label and text box
         *  "Delay" label and text box
         *  "Targets" label and enumeration multi-select
         *  "Start Target" label and enumeration single-select
         *  "Retargetable" label and checkbox
         *  "Effects" label
         *      Vertical scrolling area with effect controls
         *      New Effect button
         * Create button, creates a new skill
         * When a change has been made, a Save and Revert button are shown as enabled
         */

        listItemStyle.normal.background = Texture2D.blackTexture;   // Not that it shows up, awesome
        listItemStyle.normal.textColor = Color.white;

        selectedItemStyle.normal.background = Texture2D.whiteTexture;
        selectedItemStyle.normal.textColor = Color.blue;


        bool newSkillClicked = false;
        string newSkillName = string.Empty;
        if (GUI.Button(new Rect(30, 370, 100, 20), "New Skill"))
        {
            SkillDescriptor newSkill = new SkillDescriptor();
            newSkillName = newSkill.name;
            SkillDatabase.UpdateSkill(newSkill);
            newSkillClicked = true;
        }


        GUI.Box(new Rect(10, 50, 140, 300), string.Empty);
        GUI.Label(new Rect(10, 10, 40, 15), "Skills");
        GUI.SetNextControlName("txtSearch");
        if (newSkillClicked) searchText = string.Empty;
        searchText = GUI.TextField(new Rect(10, 30, 90, 15), searchText);
        if (GUI.GetNameOfFocusedControl() != "txtSearch" && searchText.Length == 0) GUI.Label(new Rect(10, 30, 90, 15), "(Search)");

        List<string> skillNames = SkillDatabase.FindSkills(searchText);
        skillListScrollPosition = GUI.BeginScrollView(new Rect(10, 50, 140, 300), skillListScrollPosition, new Rect(0, 0, 120, skillNames.Count * 20));

        if (newSkillClicked)
        {
            selectedSkillName = newSkillName;
        }

        string startSelectionName = selectedSkillName;

        int yButton = 10, skillIndex = 0;
        foreach (string skillName in skillNames)
        {
            if (GUI.Button(new Rect(5, yButton, 130, 15), skillName, selectedSkillName == skillName ? selectedItemStyle : listItemStyle)) selectedSkillName = skillName;
            yButton += 20;
            skillIndex++;
        }

        if (startSelectionName != selectedSkillName)
        {
            EditorGUI.FocusTextInControl(null);
            Repaint();
        }

        selectedSkill = SkillDatabase.GetSkill(selectedSkillName);

        GUI.EndScrollView();


        GUI.Box(new Rect(160, 10, 700, 400), string.Empty);

        if (selectedSkill != null)
        {

            mainControlRectY = 20;
            // TODO: Set up the skill display itself, and wire up components to variables.
            // Then, create the save/load function. Going to be cool!
            string originalName = selectedSkill.name;
            selectedSkill.name = EditorGUI.TextField(MainControlRect(), "Name", selectedSkill.name);
            if (originalName != selectedSkill.name)
            {
                SkillDatabase.RenameSkill(originalName, selectedSkill);
                selectedSkillName = selectedSkill.name;
            }
            selectedSkill.displayText = EditorGUI.TextField(MainControlRect(), "Display Text", selectedSkill.displayText);
            selectedSkill.delay = EditorGUI.FloatField(MainControlRect(), "Action Delay", selectedSkill.delay);
            //EditorGUI.LabelField(new Rect(130, y, 100, 15), "Targets");



            // The dance of the target options.

            // First, figure out what our default selection is - because it might forcibly change right after this.
            // Get the list of currently available flags, and use the selectedDefaultIndex value to figure out
            // which one is selected in the editor.
            List<CommandBase.Target> flags = new List<CommandBase.Target>();
            foreach (CommandBase.Target target in (CommandBase.Target[])System.Enum.GetValues(typeof(CommandBase.Target)))
            {
                if ((selectedSkill.targets & target) > 0) flags.Add(target);
            }
            CommandBase.Target selectedDefaultTarget = CommandBase.Target.NONE;
            if (selectedDefaultIndex < flags.Count) selectedDefaultTarget = flags[selectedDefaultIndex];

            // Now show the targets enum flags field to pick up changes.
            selectedSkill.targets = (CommandBase.Target)(EditorGUI.EnumFlagsField(MainControlRect(), "Targets", selectedSkill.targets));

            // Now, re-fetch the list of available targets.
            flags.Clear();
            foreach (CommandBase.Target target in (CommandBase.Target[])System.Enum.GetValues(typeof(CommandBase.Target)))
            {
                if ((selectedSkill.targets & target) > 0) flags.Add(target);
            }
            if (flags.Count > 0)
            {
                // Is our selected default value still in here? If so, make sure we're still pointing at it; otherwise, point at the first item in the list.
                if (flags.Contains(selectedDefaultTarget))
                    selectedDefaultIndex = flags.IndexOf(selectedDefaultTarget);
                else
                    selectedDefaultIndex = 0;

                // Now convert the flags to strings so we can display them in a popup.
                List<string> flagStrings = new List<string>();
                foreach (CommandBase.Target flag in flags) flagStrings.Add(flag.ToString());
                selectedDefaultIndex = EditorGUI.Popup(MainControlRect(), "Default target", selectedDefaultIndex, flagStrings.ToArray());
                if (selectedDefaultIndex < flags.Count) selectedDefaultTarget = flags[selectedDefaultIndex];
            }

            selectedSkill.isRetargetable = EditorGUI.Toggle(MainControlRect(), "Retargetable", selectedSkill.isRetargetable);

            mainControlRectY += 40;
            EditorGUI.LabelField(MainControlRect(), "Effects");

            EffectDescriptor killed = null;
            if (selectedSkill.effects != null)
            {
                foreach (EffectDescriptor effect in selectedSkill.effects)
                {
                    if (RenderEffectFieldWithDelete(effect))
                    {
                        // Deleted
                        killed = effect;
                    }
                }
                if (killed != null) selectedSkill.RemoveEffect(killed);

            }
            EditorGUI.LabelField(new Rect(170, mainControlRectY, 100, 15), "Add effect type");

            List<string> effectNames = new List<string>();
            foreach (string key in effectsByName.Keys) effectNames.Add(key);

            selectedNewEffectType = EditorGUI.Popup(new Rect(280, mainControlRectY, 100, 15), selectedNewEffectType, effectNames.ToArray());
            if (GUI.Button(new Rect(390, mainControlRectY, 70, 15), "Add"))
            {
                EffectDescriptor newDescriptor = new EffectDescriptor();
                newDescriptor.effectType = effectsByName[effectNames[selectedNewEffectType]];
                newDescriptor.effectExpression = string.Empty;
                selectedSkill.AddEffect(newDescriptor);
            }

            SkillDatabase.SaveSkills();
        }


    }

    private int selectedNewEffectType = 0;

    private int mainControlRectY = 20;
    private Rect MainControlRect()
    {
        Rect ret = new Rect(170, mainControlRectY, 300, 15);
        mainControlRectY += 20;
        return ret;
    }

    private bool RenderEffectFieldWithDelete(EffectDescriptor effect)
    {
        Expression expression = new Expression(effect.effectExpression, validVariables);
        expression.Parse();
        string expressionError = string.Empty;
        if (!expression.isValid)
        {
            expressionError = expression.LastError;
        }
        effect.effectExpression = EditorGUI.TextField(new Rect(170, mainControlRectY, 400, 15), effectDisplayNames[effect.effectType], effect.effectExpression);
        bool result = GUI.Button(new Rect(600, mainControlRectY, 20, 15), "X");
        if (!expression.isValid)
        {
            mainControlRectY += 20;
            EditorGUI.LabelField(new Rect(170, mainControlRectY, 700, 15), expressionError);
        }
        mainControlRectY += 20;
        return result;
    }
}
#endif