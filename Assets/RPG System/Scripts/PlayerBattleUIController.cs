using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBattleUIController : MonoBehaviour {

    private Queue<PlayerCombatController> playerUnitsReadyToSelectCommand = new Queue<PlayerCombatController>();

    private PlayerCombatController activePlayer = null;

    private BattleController battleController;
    private CommandBase selectedCommand = null;

    private List<ActorCombatController> currentTargets = new List<ActorCombatController>();

    private PlayerCombatController lastSinglePlayerCombatTarget;
    private EnemyCombatController lastSingleEnemyCombatTarget;

    public float menuXAdjust = -80;
    public float menuYAdjust = -30;

    public float healthXAdjust = 60;
    public float healthYAdjust = -30;

    public GameObject fingerPointerTemplate;

    private Dictionary<ActorCombatController, GameObject> fingerPointers = new Dictionary<ActorCombatController, GameObject>();

    private enum TargetChangeDirection
    {
        Left,
        Right,
        Up,
        Down
    }

    // Use this for initialization
    void Start () {
        battleController = GameObject.FindGameObjectWithTag("BattleController").GetComponent<BattleController>();

        foreach (ActorCombatController actor in battleController.playerActors)
        {
            fingerPointers.Add(actor, GameObject.Instantiate(fingerPointerTemplate));
            fingerPointers[actor].transform.position = actor.transform.Find("pointerLocation").position;
            Vector3 scaleInvert = fingerPointers[actor].transform.localScale;
            scaleInvert.x *= -1;
            fingerPointers[actor].transform.localScale = scaleInvert;
        }

        foreach (ActorCombatController actor in battleController.enemyActors)
        {
            fingerPointers.Add(actor, GameObject.Instantiate(fingerPointerTemplate));
            fingerPointers[actor].transform.position = actor.transform.Find("pointerLocation").position;
        }

        foreach (ActorCombatController actor in fingerPointers.Keys)
            fingerPointers[actor].SetActive(false);

    }
	
	// Update is called once per frame
	void Update () {

        if (battleController.isBattleFinished)
        {
            playerUnitsReadyToSelectCommand.Clear();
            selectedCommand = null;
            currentTargets.Clear();
        }

        activePlayer = null;
        if (playerUnitsReadyToSelectCommand.Count > 0)
        {
            activePlayer = playerUnitsReadyToSelectCommand.Peek();
        }

        if (activePlayer != null)
        {
            if (selectedCommand != null)
            {
                switch (selectedCommand.currentTargetSelection)
                {
                    case CommandBase.ALL_ALLIES:
                        //  Left: previous target option
                        //  Right/Up/Down: Nothing
                        if (Input.GetKeyDown(KeyCode.LeftArrow)) selectedCommand.currentTargetIndex--;
                        break;
                    case CommandBase.SELECTED_ALLY:
                        //  Left: previous target option
                        //  Right: next target option
                        //  Up/Down: Toggle targets
                        if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            selectedCommand.currentTargetIndex--;
                        }
                        if (Input.GetKeyDown(KeyCode.RightArrow)) selectedCommand.currentTargetIndex++;
                        if (Input.GetKeyDown(KeyCode.UpArrow)) lastSinglePlayerCombatTarget = NextPlayer(TargetChangeDirection.Up);
                        if (Input.GetKeyDown(KeyCode.DownArrow)) lastSinglePlayerCombatTarget = NextPlayer(TargetChangeDirection.Down);
                        break;
                    case CommandBase.SELF:
                        //  As SELECTED_ALLY but Up/Down does nothing
                        if (Input.GetKeyDown(KeyCode.LeftArrow)) selectedCommand.currentTargetIndex--;
                        if (Input.GetKeyDown(KeyCode.RightArrow)) selectedCommand.currentTargetIndex++;
                        break;
                    case CommandBase.SELECTED_ENEMY:
                        //  Left: Previous single enemy target if one available, or previous target option otherwise
                        //  Right: Next single enemy target if one availalbe, or next target option otherwise
                        //  Up/Down: Vertical toggle on targets (wraparound possible)
                        if (Input.GetKeyDown(KeyCode.LeftArrow))
                        {
                            EnemyCombatController nextEnemy = NextEnemy(TargetChangeDirection.Left);
                            if (nextEnemy == null)
                            {
                                selectedCommand.currentTargetIndex--;
                            }
                            else
                            {
                                lastSingleEnemyCombatTarget = nextEnemy;
                            }
                        }
                        if (Input.GetKeyDown(KeyCode.RightArrow))
                        {
                            EnemyCombatController nextEnemy = NextEnemy(TargetChangeDirection.Right);
                            if (nextEnemy == null)
                            {
                                selectedCommand.currentTargetIndex++;
                            }
                            else
                            {
                                lastSingleEnemyCombatTarget = nextEnemy;
                            }
                        }
                        if (Input.GetKeyDown(KeyCode.UpArrow)) lastSingleEnemyCombatTarget = NextEnemy(TargetChangeDirection.Up);
                        if (Input.GetKeyDown(KeyCode.DownArrow)) lastSingleEnemyCombatTarget = NextEnemy(TargetChangeDirection.Down);
                        break;
                    case CommandBase.ALL_ENEMIES:
                        //  Right: Next target option
                        //  Left/Up/Down: Nothing
                        if (Input.GetKeyDown(KeyCode.RightArrow)) selectedCommand.currentTargetIndex++;
                        break;
                    case CommandBase.RANDOM_ENEMY:
                        // No input to handle
                        //  This should never apply here; if RANDOM_ENEMY is in the list,
                        //  it should be alone, and a random enemy should be selected instead
                        break;
                }

                // Update targets based on current command targets identified
                currentTargets.Clear();
                switch (selectedCommand.currentTargetSelection)
                {
                    case CommandBase.ALL_ALLIES:
                        foreach (PlayerCombatController player in battleController.playerActors) currentTargets.Add(player);
                        break;
                    case CommandBase.SELECTED_ALLY:
                        currentTargets.Add(lastSinglePlayerCombatTarget);
                        break;
                    case CommandBase.SELF:
                        currentTargets.Add(lastSinglePlayerCombatTarget);
                        break;
                    case CommandBase.SELECTED_ENEMY:
                        currentTargets.Add(lastSingleEnemyCombatTarget);
                        break;
                    case CommandBase.ALL_ENEMIES:
                        foreach (EnemyCombatController enemy in battleController.enemyActors) currentTargets.Add(enemy);
                        break;
                }

                if (Input.GetKeyDown(KeyCode.Return))
                {
                    // Command is selected; player is done with prep and can be dequeued.
                    activePlayer = playerUnitsReadyToSelectCommand.Dequeue();

                    // Fill out the command with the current targets,
                    // and send it back through the actor so that actor can update state.
                    foreach (ActorCombatController target in currentTargets)
                    {
                        selectedCommand.AddTarget(target);
                    }
                    activePlayer.RegisterCommand(selectedCommand);

                    // Mark selection and active player as null so we can move to next player.
                    selectedCommand = null;
                    activePlayer = null;
                    currentTargets.Clear();

                }
            } else
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    // Dequeue the top ready player and put them at the back of the line.
                    playerUnitsReadyToSelectCommand.Enqueue(playerUnitsReadyToSelectCommand.Dequeue());
                }

            }
        }

        foreach (ActorCombatController actor in fingerPointers.Keys) fingerPointers[actor].SetActive(false);
        foreach (ActorCombatController actor in currentTargets)
        {
            fingerPointers[actor].SetActive(true);
        }
    }

    public void PlayerUnitReadyToSelectCommand(PlayerCombatController playerUnit)
    {
        playerUnitsReadyToSelectCommand.Enqueue(playerUnit);
    }

    private void OnGUI()
    {
        if (activePlayer != null)
        {
            if (selectedCommand == null)
            {
                // Show a list of their commands next to them
                Dictionary<string, CommandBase.Command> availableCommands = activePlayer.AvailableCommands();
                int commandCount = availableCommands.Count;
                Vector3 playerScreenLocation = Camera.main.WorldToScreenPoint(activePlayer.transform.position);
                float x = playerScreenLocation.x + menuXAdjust;
                float y = Screen.height - playerScreenLocation.y + menuYAdjust;
                foreach (string commandName in availableCommands.Keys)
                {
                    if (GUI.Button(new Rect(x, y, 60, 20), commandName))
                    {
                        OnPlayerActivatedCommand(availableCommands[commandName]);
                    }
                }
            }
            else
            {
                // Show current target
                foreach (ActorCombatController target in currentTargets)
                {
                    // TODO: Draw a pointer at the target
                    GUI.Label(new Rect(target.transform.position.x, target.transform.position.y, 25, 15), "Target");
                }
            }
        }

        // Show all player HP
        foreach (PlayerCombatController player in battleController.playerActors)
        {
            Vector3 playerScreenLocation = Camera.main.WorldToScreenPoint(player.transform.position);
            float x = playerScreenLocation.x + healthXAdjust;
            float y = Screen.height - playerScreenLocation.y + menuYAdjust;
            GUI.Label(new Rect(x, y, 100, 20), string.Format("{0}/{1}", player.actor.hitPoints, player.actor.maxVitality));
        }
    }

    void OnPlayerActivatedCommand(CommandBase.Command command)
    {
        selectedCommand = CommandFactory.Instance.GetCommand(command, activePlayer);
        lastSingleEnemyCombatTarget = null;
        lastSinglePlayerCombatTarget = null;

        lastSingleEnemyCombatTarget = RightEnemy();
        lastSinglePlayerCombatTarget = activePlayer;

        //switch (selectedCommand.currentTargetSelection)
        //{
        //    case CommandBase.SELECTED_ALLY:
        //        lastSinglePlayerCombatTarget = activePlayer;
        //        break;
        //    case CommandBase.SELF:
        //        lastSinglePlayerCombatTarget = activePlayer;
        //        break;
        //    case CommandBase.SELECTED_ENEMY:
        //        // Find rightmost enemy
        //        float xPos = -1000;
        //        foreach (EnemyCombatController enemy in battleController.enemyActors)
        //        {
        //            if (enemy.transform.position.x > xPos)
        //            {
        //                xPos = enemy.transform.position.x;
        //                lastSingleEnemyCombatTarget = enemy;
        //            }
        //        }
        //        break;
        //}
    }

    private PlayerCombatController NextPlayer(TargetChangeDirection change)
    {
        float currentY = lastSinglePlayerCombatTarget.transform.position.y;
        float ny = currentY;
        PlayerCombatController nextPlayer = null;
        switch (change)
        {
            case TargetChangeDirection.Up:
                foreach (PlayerCombatController player in battleController.playerActors)
                {
                    float ey = player.transform.position.y;
                    if (ey > currentY && (ey > ny || ny == currentY))
                    {
                        ny = ey;
                        nextPlayer = player;
                    }
                }
                if (nextPlayer == null) nextPlayer = BottomPlayer();
                return nextPlayer;
            case TargetChangeDirection.Down:
                foreach (PlayerCombatController player in battleController.playerActors)
                {
                    float ey = player.transform.position.y;
                    if (ey < currentY && (ey < ny || ny == currentY))
                    {
                        ny = ey;
                        nextPlayer = player;
                    }
                }
                if (nextPlayer == null) nextPlayer = TopPlayer();
                return nextPlayer;
            default:
                throw new System.ArgumentException("Unhandled switch case [" + change.ToString() + "]");

        }
    }

    private EnemyCombatController NextEnemy(TargetChangeDirection change)
    {
        float currentX = lastSingleEnemyCombatTarget.transform.position.x;
        float nx = currentX;
        float currentY = lastSingleEnemyCombatTarget.transform.position.y;
        float ny = currentY;
        EnemyCombatController nextEnemy = null;
        switch (change)
        {
            case TargetChangeDirection.Left:
                foreach (EnemyCombatController enemy in battleController.enemyActors)
                {
                    float ex = enemy.transform.position.x;
                    if (ex < currentX && (ex > nx || nx == currentX))
                    {
                        nx = ex;
                        nextEnemy = enemy;
                    }
                }
                return nextEnemy;
            case TargetChangeDirection.Right:
                foreach (EnemyCombatController enemy in battleController.enemyActors)
                {
                    float ex = enemy.transform.position.x;
                    if (ex > currentX && (ex > nx || nx == currentX))
                    {
                        nx = ex;
                        nextEnemy = enemy;
                    }
                }
                return nextEnemy;
            case TargetChangeDirection.Up:
                foreach (EnemyCombatController enemy in battleController.enemyActors)
                {
                    float ey = enemy.transform.position.y;
                    if (ey > currentY && (ey > ny || ny == currentY))
                    {
                        ny = ey;
                        nextEnemy = enemy;
                    }
                }
                if (nextEnemy == null) nextEnemy = BottomEnemy();
                return nextEnemy;
            case TargetChangeDirection.Down:
                foreach (EnemyCombatController enemy in battleController.enemyActors)
                {
                    float ey = enemy.transform.position.y;
                    if (ey < currentY && (ey < ny || ny == currentY))
                    {
                        ny = ey;
                        nextEnemy = enemy;
                    }
                }
                if (nextEnemy == null) nextEnemy = TopEnemy();
                return nextEnemy;
            default:
                throw new System.ArgumentException("Unhandled switch case [" + change.ToString() + "]");
        }
        
    }

    PlayerCombatController BottomPlayer()
    {
        PlayerCombatController bottom = null;
        float currentY = float.MaxValue;
        foreach (PlayerCombatController player in battleController.playerActors)
        {
            float ey = player.transform.position.y;
            if (ey < currentY)
            {
                currentY = ey;
                bottom = player;
            }
        }
        return bottom;
    }

    PlayerCombatController TopPlayer()
    {
        PlayerCombatController top = null;
        float currentY = float.MinValue;
        foreach (PlayerCombatController player in battleController.playerActors)
        {
            float ey = player.transform.position.y;
            if (ey > currentY)
            {
                currentY = ey;
                top = player;
            }
        }
        return top;
    }

    EnemyCombatController BottomEnemy()
    {
        EnemyCombatController bottom = null;
        float currentY = float.MaxValue;
        foreach (EnemyCombatController enemy in battleController.enemyActors)
        {
            float ey = enemy.transform.position.y;
            if (ey < currentY)
            {
                currentY = ey;
                bottom = enemy;
            }
        }
        return bottom;
    }

    EnemyCombatController TopEnemy()
    {
        EnemyCombatController top = null;
        float currentY = float.MinValue;
        foreach (EnemyCombatController enemy in battleController.enemyActors)
        {
            float ey = enemy.transform.position.y;
            if (ey > currentY)
            {
                currentY = ey;
                top = enemy;
            }
        }
        return top;
    }

    EnemyCombatController RightEnemy()
    {
        EnemyCombatController right = null;
        float currentX = float.MinValue;
        foreach (EnemyCombatController enemy in battleController.enemyActors)
        {
            float ex = enemy.transform.position.x;
            if (ex > currentX)
            {
                currentX = ex;
                right = enemy;
            }
        }
        return right;
    }

    EnemyCombatController LeftEnemy()
    {
        EnemyCombatController left = null;
        float currentX = float.MaxValue;
        foreach (EnemyCombatController enemy in battleController.enemyActors)
        {
            float ex = enemy.transform.position.x;
            if (ex < currentX)
            {
                currentX = ex;
                left = enemy;
            }
        }
        return left;
    }

}
