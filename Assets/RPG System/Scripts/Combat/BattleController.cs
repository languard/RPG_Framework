using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleController : MonoBehaviour {

    public List<EnemyCombatController> enemyActors = new List<EnemyCombatController>();
    public List<PlayerCombatController> playerActors = new List<PlayerCombatController>();

    public List<CommandBase> commands = new List<CommandBase>();

    private Queue<CommandBase> activeCommands = new Queue<CommandBase>();

    public float activationPauseTime = 0.25f;
    public float marqueeTime = 1.0f;
    public float hesitationTime = 0.1f;
    public float effectsTime = 1.0f;
    public float postCommandCooldownTime = 0.5f;

    private Dictionary<ActorCombatController, MeshRenderer> silhouettes = new Dictionary<ActorCombatController, MeshRenderer>();

    public GUIStyle marqueeStyle;
    public GUIStyle effectsStyle;

    public float sceneOutroTime = 2.0f;
    private float sceneOutroCountdown = 0.0f;

    //add any addition variables needed for battle rewards here
    public int victoryGold = 1;

    private enum CommandState
    {
        Idle,
        HighlightActor,
        ShowMarquee,
        Hesitation,
        Effects,
        Cooldown
    }

    private enum BattleState
    {
        Normal,
        Victory,
        Defeat
    }

    private BattleState battleState = BattleState.Normal;

    private CommandState commandState = CommandState.Idle;
    private float stateTimer = 0.0f;

    public bool isBattleFinished
    {
        get { return battleState != BattleState.Normal; }
    }

	// Use this for initialization
	void Start () {

        GameObject[] players = GameObject.FindGameObjectsWithTag("PlayerCharacter");
        foreach (GameObject player in players)
        {
            playerActors.Add(player.GetComponent<PlayerCombatController>());
            foreach (Transform child in player.transform)
            {
                if (child.tag == "BattleSilhouette")
                {
                    silhouettes.Add(player.GetComponent<PlayerCombatController>(), child.GetComponent<MeshRenderer>());
                }
            }
        }

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("EnemyCharacter");
        foreach (GameObject enemy in enemies)
        {
            enemyActors.Add(enemy.GetComponent<EnemyCombatController>());
            foreach (Transform child in enemy.transform)
            {
                if (child.tag == "BattleSilhouette")
                {
                    silhouettes.Add(enemy.GetComponent<EnemyCombatController>(), child.GetComponent<MeshRenderer>());
                }
            }
        }

        foreach (ActorCombatController actor in silhouettes.Keys)
        {
            silhouettes[actor].enabled = false;
        }

    }

    // Update is called once per frame
    void Update ()
    {
        switch (battleState)
        {
            case BattleState.Normal:
                UpdateCommandQueue();
                UpdateCommandState();

                bool anyPlayerAlive = false, anyEnemyAlive = false;
                foreach (PlayerCombatController player in playerActors) anyPlayerAlive |= !player.entity.isDead;
                foreach (EnemyCombatController enemy in enemyActors) anyEnemyAlive |= !enemy.entity.isDead;

                if (!anyEnemyAlive) battleState = BattleState.Victory;
                if (!anyPlayerAlive) battleState = BattleState.Defeat;

                if (battleState != BattleState.Normal)
                {
                    sceneOutroCountdown = sceneOutroTime;
                }

                break;
            case BattleState.Defeat:
                sceneOutroCountdown -= Time.deltaTime;
                if (sceneOutroCountdown <= 0.0f)
                {
                    UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
                }
                break;
            case BattleState.Victory:
                sceneOutroCountdown -= Time.deltaTime;
                if (sceneOutroCountdown <= 0.0f)
                {
                    //either create new methods to call, or modify BattleDone to pass on rewards
                    GameObject.Find("GameMaster").GetComponent<GameMaster>().BattleDone(victoryGold);
                }
                break;
        }

    }

    private void UpdateCommandState()
    {
        switch (commandState)
        {
            case CommandState.Idle:
                if (activeCommands.Count > 0)
                {
                    // Is the acting character alive?
                    if (activeCommands.Peek().owner.entity.isDisabled)
                    {
                        activeCommands.Dequeue();
                    }
                    else
                    {
                        stateTimer = activationPauseTime;
                        commandState = CommandState.HighlightActor;
                    }
                }
                break;
            case CommandState.HighlightActor:
                silhouettes[activeCommands.Peek().owner].enabled = true;
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                {
                    stateTimer = marqueeTime;
                    commandState = CommandState.ShowMarquee;
                }
                break;
            case CommandState.ShowMarquee:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                {
                    stateTimer = hesitationTime;
                    commandState = CommandState.Hesitation;
                }
                break;
            case CommandState.Hesitation:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                {
                    stateTimer = effectsTime;
                    commandState = CommandState.Effects;

                    CommandBase activeCommand = activeCommands.Peek();

                    // Single-target retargeting - if the target actor 
                    // is already dead, retarget a different actor on 
                    // the target actor's team
                    if (activeCommand.targetActors.Count == 1 && activeCommand.targetActors[0].entity.isDead && activeCommand.isRetargetable)
                    {
                        bool targetIsEnemy = (activeCommand.targetActors[0] as EnemyCombatController != null);
                        if (targetIsEnemy)
                            activeCommand.targetActors[0] = enemyActors[0];
                        else
                            activeCommand.targetActors[0] = playerActors[0];    // May need to refine this since player actors don't really go away.

                    }
                    
                    List<CombatEffectBase> effects = activeCommands.Peek().Execute();
                    foreach (CombatEffectBase effect in effects)
                    {
                        effect.Process();
                        if (effect as WeaponDamage != null)
                        {
                            ShowDamage show = effect.target.gameObject.AddComponent<ShowDamage>();
                            show.damage = effect.displayText;
                            show.lifetime = effectsTime;
                            show.displayStyle = effectsStyle;

                            if (enemyActors.Contains(effect.target as EnemyCombatController))
                            {
                                if (effect.target.entity.isDead)
                                {
                                    enemyActors.Remove(effect.target as EnemyCombatController);
                                }
                            }
                        }

                    }
                }
                break;
            case CommandState.Effects:
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                {
                    stateTimer = postCommandCooldownTime;
                    commandState = CommandState.Cooldown;
                    activeCommands.Peek().owner.ProcessCommand(activeCommands.Peek());
                }
                break;
            case CommandState.Cooldown:
                silhouettes[activeCommands.Peek().owner].enabled = false;
                stateTimer -= Time.deltaTime;
                if (stateTimer <= 0)
                {
                    activeCommands.Dequeue();
                    commandState = CommandState.Idle;
                }
                break;
        }
    }

    private void UpdateCommandQueue()
    {
        // We're going to look through all currently registered commands
        // to see if any have been completed.
        // But, if more than one is queued up, we can't let
        // the addition be arbitrary. If one event finished
        // at the start of the frame and another finished at the end,
        // we always want the first one to precede the second.

        Dictionary<float, CommandBase> completedCommands = new Dictionary<float, CommandBase>();
        for (int i = 0; i < commands.Count; i++)
        {
            // The AddTimeWaited returns negative results while waiting for completion.
            // The first frame where it's ready, it will return how much time it had
            // left to wait when the frame started.
            float waitReturn = commands[i].AddTimeWaited(Time.deltaTime);
            if (waitReturn >= 0)
            {
                while (completedCommands.ContainsKey(waitReturn)) waitReturn += 0.001f;
                completedCommands.Add(waitReturn, commands[i]);
            }
        }

        // Using that, we can find all complete commands, and sort
        // them by how much time they had left to execute.
        // They go into the queue in that order.
        // Ties are broken based on order in the array; the while loop
        // ensures that we use unique times.
        List<float> waitTimes = new List<float>();
        foreach (float waitTime in completedCommands.Keys) waitTimes.Add(waitTime);
        waitTimes.Sort();

        foreach (float waitTime in waitTimes)
        {
            commands.Remove(completedCommands[waitTime]);
            activeCommands.Enqueue(completedCommands[waitTime]);
        }
    }

    public void AddCommand(CommandBase command)
    {
        commands.Add(command);
    }

    private void OnGUI()
    {
        switch (battleState)
        {
            case BattleState.Normal:
                if (commandState == CommandState.ShowMarquee)
                {
                    GUI.Label(new Rect(Screen.width / 2 - 100, 40, 200, 30), activeCommands.Peek().displayName, marqueeStyle);
                }
                break;
            case BattleState.Victory:
                GUI.Label(new Rect(Screen.width / 2 - 100, 40, 200, 30), "Victory!", marqueeStyle);
                break;
            case BattleState.Defeat:
                GUI.Label(new Rect(Screen.width / 2 - 100, 40, 200, 30), "Defeat", marqueeStyle);
                break;
        }



    }
}
