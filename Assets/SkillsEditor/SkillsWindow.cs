using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class SkillsWindow : EditorWindow
{
    public static SkillsWindow instance;

    private static GUIStyle listItemStyle = new GUIStyle();
    private static GUIStyle selectedItemStyle = new GUIStyle();

    [MenuItem("Tools/Skills/Skills Editor")]
    static void ShowWindow()
    {
        instance = (SkillsWindow)EditorWindow.GetWindow<SkillsWindow>();
        instance.titleContent = new GUIContent("Skills Editor");

        // TODO: Load skills from JSON data file

    }

    private string searchText = string.Empty;
    private Vector2 skillListScrollPosition = Vector2.zero;
    private int skillSelectionIndex = 0;

    private string displayText = "Fireball";
    private float actionDelay = 2.0f;

    private CommandBase.Target targetFlags = CommandBase.Target.SELECTED_ENEMY;
    private CommandBase.Target defaultTarget = CommandBase.Target.SELECTED_ENEMY;
    private int selectedDefaultIndex = 0;

    private bool isRetargetable = true;

    private string[] effectTypes = new string[] { "Weapon Damage", "Magic Damage", "Mana Drain" };

    private List<CombatEffectBase> combatEffects = new List<CombatEffectBase>();

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
         * 
         * Try for the layout tonight?
         */

        listItemStyle.normal.background = Texture2D.blackTexture;   // Not that it shows up, awesome
        listItemStyle.normal.textColor = Color.white;

        selectedItemStyle.normal.background = Texture2D.whiteTexture;
        selectedItemStyle.normal.textColor = Color.blue;

        GUI.Box(new Rect(10, 50, 100, 300), string.Empty);

        GUI.Label(new Rect(10, 10, 40, 15), "Skills");
        GUI.SetNextControlName("txtSearch");
        searchText = GUI.TextField(new Rect(10, 30, 90, 15), searchText);
        if (GUI.GetNameOfFocusedControl() != "txtSearch" && searchText.Length == 0) GUI.Label(new Rect(10, 30, 90, 15), "(Search)");

        skillListScrollPosition = GUI.BeginScrollView(new Rect(10, 50, 100, 300), skillListScrollPosition, new Rect(0, 0, 100, 100));

        int startSelectionIndex = skillSelectionIndex;
        if (GUI.Button(new Rect(5, 10, 90, 15), "one", skillSelectionIndex == 0 ? selectedItemStyle : listItemStyle)) skillSelectionIndex = 0;
        if (GUI.Button(new Rect(5, 30, 90, 15), "two", skillSelectionIndex == 1 ? selectedItemStyle : listItemStyle)) skillSelectionIndex = 1;
        if (startSelectionIndex != skillSelectionIndex)
        {
            Repaint();
        }
        GUI.EndScrollView();


        GUI.Box(new Rect(120, 10, 400, 400), string.Empty);

        mainControlRectY = 20;
        // TODO: Set up the skill display itself, and wire up components to variables.
        // Then, create the save/load function. Going to be cool!
        string skillName = skillSelectionIndex == 0 ? "one" : "two";
        EditorGUI.TextField(MainControlRect(), "Name", skillName);
        EditorGUI.TextField(MainControlRect(), "Display Text", displayText);
        EditorGUI.FloatField(MainControlRect(), "Action Delay", actionDelay);
        //EditorGUI.LabelField(new Rect(130, y, 100, 15), "Targets");



        // The dance of the target options.

        // First, figure out what our default selection is - because it might forcibly change right after this.
        // Get the list of currently available flags, and use the selectedDefaultIndex value to figure out
        // which one is selected in the editor.
        List<CommandBase.Target> flags = new List<CommandBase.Target>();
        foreach (CommandBase.Target target in (CommandBase.Target[])System.Enum.GetValues(typeof(CommandBase.Target)))
        {
            if ((targetFlags & target) > 0) flags.Add(target);
        }
        CommandBase.Target selectedDefaultTarget = CommandBase.Target.NONE;
        if (selectedDefaultIndex < flags.Count) selectedDefaultTarget = flags[selectedDefaultIndex];

        // Now show the targets enum flags field to pick up changes.
        targetFlags = (CommandBase.Target)(EditorGUI.EnumFlagsField(MainControlRect(), "Targets", targetFlags));

        // Now, re-fetch the list of available targets.
        flags.Clear();
        foreach (CommandBase.Target target in (CommandBase.Target[])System.Enum.GetValues(typeof(CommandBase.Target)))
        {
            if ((targetFlags & target) > 0) flags.Add(target);
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

        isRetargetable = EditorGUI.Toggle(MainControlRect(), "Retargetable", isRetargetable);


        EditorGUI.LabelField(MainControlRect(), "Effects");

        foreach (CombatEffectBase effect in combatEffects)
        {
            if (effect as WeaponDamage != null)
            {
                // Show weapon damage display
            }
            else if (effect as MagicDamage != null)
            {
                // Show magic damage display
            }
            else if (effect as ManaDrain != null)
            {
                // Show mana drain display
            }
        }

        // Rats - we need more technical draft time on this. Ah well.

    }

    private int mainControlRectY = 20;
    private Rect MainControlRect()
    {
        Rect ret = new Rect(130, mainControlRectY, 300, 15);
        mainControlRectY += 20;
        return ret;
    }
}