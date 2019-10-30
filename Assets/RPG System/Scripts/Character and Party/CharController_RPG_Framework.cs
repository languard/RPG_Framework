using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(SpriteRenderer))]
public class CharController_RPG_Framework : MonoBehaviour {

    
    [SerializeField] float moveSpeed = 3;

    [SerializeField] int grid = 1;

    [SerializeField] float delta = 0.001f;

    [SerializeField]
    Camera mainCamera;


    public bool isMoving = false;
    public bool isOnGrid = false;
    public bool canAct = true;

    Vector3 moveVector = Vector3.zero;

    public delegate void DoAnimate(CharController_RPG_Framework controller);

    public DoAnimate AnimationEvent; 
    
    public int moveDirection = 0;

    public Vector2Int gridPosition { get { return new Vector2Int(Mathf.RoundToInt(transform.position.x / grid), Mathf.RoundToInt(transform.position.y / grid)); } }

    float currentHorizontal = 0;
    float currentVertical = 0;

    bool skipNextFrame = true;

    GameMaster GM;

    Rigidbody2D rb;

    MapLogic ml;

    // Use this for initialization
    void Start() {
        
        rb = GetComponent<Rigidbody2D>();
        GM = GameObject.Find("GameMaster").GetComponent<GameMaster>();
        GM.RegisterPlayerController(this);
        
    }

    void Update()
    {
        currentHorizontal = Input.GetAxis("Horizontal");
        currentVertical = Input.GetAxis("Vertical");

        if(!isMoving && canAct)
        {
            if(Input.GetButtonDown("UseAction"))
            {
                CheckForUsableObject(moveDirection);
            }
        }

        CheckForGridAlignment();

    }

    public void DisableForBattle()
    {
        canAct = false;
        mainCamera.enabled = false;
    }

    public void ActivateController()
    {
        canAct = true;
        mainCamera.enabled = true;
        //attempting to fix rare map bug
        skipNextFrame = true;
    }

    //Keeping the player on the grid with smooth movement requires a fixed framerate.
    void FixedUpdate() {

        //seeing if skipping a frame fixes a rare map bug
        if (skipNextFrame)
        {
            skipNextFrame = false;
            return;
        }

        AnimationEvent(this);
        //only animation allowed if canAct is off
        if (!canAct) return;

        if (ml == null) ml = GameObject.Find("LevelData").GetComponent<MapLogic>();

        //Not ellegant, but check every frame to see if the map BGM should play
        ml.StartMusic();

            //isOnGrid = false;

        if (isMoving)
        {
            transform.position = transform.position + moveVector * moveSpeed;
            CheckForGridAlignment();
            if(isOnGrid && canAct)
            {
                isMoving = false;
                ml.FinishStep();                
            }
        }

        //float deltaX = Mathf.Abs(Mathf.RoundToInt(transform.position.x / grid) - (transform.position.x / grid));
        //float deltaY = Mathf.Abs(Mathf.RoundToInt(transform.position.y / grid) - (transform.position.y / grid));

        //if (deltaX <= delta && deltaY <= delta) isOnGrid = true;

        CheckForGridAlignment();

        //movement
        if (isOnGrid && canAct)
        {
            isMoving = false;            
            if (currentVertical < -0.2f && currentVertical < currentHorizontal)
            {
                //Move Down
                moveDirection = StaticData.DIR_DOWN;
                //check for collision
                if (!CheckForCollision(StaticData.DIR_DOWN))
                {
                    isMoving = true;
                    moveVector = Vector3.zero;
                    moveVector.y = -1;                    
                }
            }
            else if (currentVertical > 0.2f && currentVertical > currentHorizontal)
            {
                //Move down
                moveDirection = StaticData.DIR_UP;
                if (!CheckForCollision(StaticData.DIR_UP))
                {
                    isMoving = true;
                    moveVector = Vector3.zero;
                    moveVector.y = 1;
                    
                }
            }
            else if (currentHorizontal > 0.2f)
            {
                //Move right
                moveDirection = StaticData.DIR_RIGHT;
                if (!CheckForCollision(StaticData.DIR_RIGHT))
                {
                    isMoving = true;
                    moveVector = Vector3.zero;
                    moveVector.x = 1;
                    
                }
            }
            else if (currentHorizontal < -0.2f)
            {
                //Move left
                moveDirection = StaticData.DIR_LEFT;
                if (!CheckForCollision(StaticData.DIR_LEFT))
                {
                    isMoving = true;
                    moveVector = Vector3.zero;
                    moveVector.x = -1;
                    
                }
            }
        } // end movement

        

    }

    public void CheckForGridAlignment()
    {
        isOnGrid = false;
        float deltaX = Mathf.Abs(Mathf.RoundToInt(transform.position.x / grid) - (transform.position.x / grid));
        float deltaY = Mathf.Abs(Mathf.RoundToInt(transform.position.y / grid) - (transform.position.y / grid));
        if (deltaX <= delta && deltaY <= delta) isOnGrid = true;
    }

    void OnTriggerStay2D(Collider2D other)
    {
        
        if(isOnGrid)
        {
            //other.GetComponent<TriggerInteract>().DoAction();
        }
    }

    bool CheckForCollision(int direction)
    {
        bool result = false;
        RaycastHit2D[] hitList = new RaycastHit2D[5];


        Vector2 offset = Vector2.zero;

        switch(direction)
        {
            case StaticData.DIR_UP:
                offset.y = 1;
                break;

            case StaticData.DIR_DOWN:
                offset.y = -1;
                break;

            case StaticData.DIR_RIGHT:
                offset.x = 1;
                break;

            case StaticData.DIR_LEFT:
                offset.x = -1;
                break;
        }

        //offset = offset * grid ;

        int hitCount = rb.Cast(offset, hitList, grid / 2);
        if (hitCount > 5) Debug.LogError("Do not have more than 5 items stacked on one tile!  Bad designer, no twinkie!");
          
        for(int i=0; i<hitCount; i++)
        {
            if (hitList[i].collider.isTrigger) result = false;
            else result = true;
        }
            
        

        return result;
    }

    void CheckForUsableObject(int direction)
    {
        Vector2 checkDir = Vector2.zero;
        RaycastHit2D[] hitList = new RaycastHit2D[5];

        switch (direction)
        {
            case StaticData.DIR_UP:
                checkDir.y = 1;
                break;

            case StaticData.DIR_DOWN:
                checkDir.y = -1;
                break;

            case StaticData.DIR_RIGHT:
                checkDir.x = 1;
                break;

            case StaticData.DIR_LEFT:
                checkDir.x = -1;
                break;
        }

        int hitCount = rb.Cast(checkDir, hitList, grid / 2);
        if (hitCount > 5) Debug.LogError("Do not have more than 5 items stacked on one tile!  Bad designer, no twinkie!");
        for (int i = 0; i < hitCount; i++)
        {
            Fungus.Flowchart fc = hitList[i].collider.gameObject.GetComponent<Fungus.Flowchart>();
            if (fc != null)
            {                
                fc.ExecuteBlock("InteractStart");
                break;
            }//end if
        }//end for
    }//end method

    public void SetLocation(int x, int y)
    {
        Vector3 newPos = new Vector3(x, y, 0);
        transform.position = newPos;
    }
            
}
