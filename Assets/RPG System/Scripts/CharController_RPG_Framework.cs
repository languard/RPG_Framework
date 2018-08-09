using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[RequireComponent(typeof(SpriteRenderer))]
public class CharController_RPG_Framework : MonoBehaviour {

    [SerializeField] Sprite[] downSprites;
    [SerializeField] Sprite[] leftSprites;
    [SerializeField] Sprite[] rightSprites;
    [SerializeField] Sprite[] upSprites;

    [SerializeField] float animationSpeed = 1;
    [SerializeField] float moveSpeed = 3;

    [SerializeField] int grid = 1;

    [SerializeField] float delta = 0.001f;

    [SerializeField] Tilemap primary;
    [SerializeField] Tilemap interactive;

    bool isMoving = false;
    bool isOnGrid = false;
    Vector3 moveVector = Vector3.zero;

    float animationFrame = 0;
    int maxAnimationFrame;
    int animationDirection = 0;

    const int DIR_UP = 0;
    const int DIR_LEFT = 1;
    const int DIR_RIGHT = 2;
    const int DIR_DOWN = 3;

    float currentHorizontal = 0;
    float currentVertical = 0;

    SpriteRenderer sr;

    Rigidbody2D rb;

    // Use this for initialization
    void Start() {

        sr = GetComponent<SpriteRenderer>();
        maxAnimationFrame = downSprites.Length;
        rb = GetComponent<Rigidbody2D>();

        UnityEngine.Tilemaps.Tilemap tmap;

        
    }

    void Update()
    {
        currentHorizontal = Input.GetAxis("Horizontal");
        currentVertical = Input.GetAxis("Vertical");

        if(!isMoving)
        {
            if(Input.GetButtonDown("UseAction"))
            {
                CheckForUsableObject(animationDirection);
            }
        }

    }

    //Keeping the player on the grid with smooth movement requires a fixed framerate.
    void FixedUpdate() {


        isOnGrid = false;

        float deltaX = Mathf.Abs(Mathf.RoundToInt(transform.position.x / grid) - (transform.position.x / grid));
        float deltaY = Mathf.Abs(Mathf.RoundToInt(transform.position.y / grid) - (transform.position.y / grid));

        if (deltaX <= delta && deltaY <= delta) isOnGrid = true;


        //movement
        if (isOnGrid)
        {
            isMoving = false;            
            if (currentVertical < -0.2f)
            {
                //Move Down
                animationDirection = DIR_DOWN;
                //check for collision
                if (!CheckForCollision(DIR_DOWN))
                {
                    isMoving = true;
                    moveVector = Vector3.zero;
                    moveVector.y = -1;                    
                }
            }
            else if (currentVertical > 0.2f)
            {
                //Move down
                animationDirection = DIR_UP;
                if (!CheckForCollision(DIR_UP))
                {
                    isMoving = true;
                    moveVector = Vector3.zero;
                    moveVector.y = 1;
                    
                }
            }
            else if (currentHorizontal > 0.2f)
            {
                //Move right
                animationDirection = DIR_RIGHT;
                if (!CheckForCollision(DIR_RIGHT))
                {
                    isMoving = true;
                    moveVector = Vector3.zero;
                    moveVector.x = 1;
                    
                }
            }
            else if (currentHorizontal < -0.2f)
            {
                //Move left
                animationDirection = DIR_LEFT;
                if (!CheckForCollision(DIR_LEFT))
                {
                    isMoving = true;
                    moveVector = Vector3.zero;
                    moveVector.x = -1;
                    
                }
            }
        } // end movement

        //animate and move if moving
        if (isMoving)
        {
            animationFrame += animationSpeed;
            if (animationFrame >= maxAnimationFrame) animationFrame = 1;

            transform.position = transform.position + moveVector * moveSpeed;
        }
        else
        {
            animationFrame = 0;
        }

        //update sprite
        switch (animationDirection)
        {
            case DIR_UP:
                sr.sprite = upSprites[Mathf.FloorToInt(animationFrame)];
                break;
            case DIR_LEFT:
                sr.sprite = leftSprites[Mathf.FloorToInt(animationFrame)];
                break;
            case DIR_RIGHT:
                sr.sprite = rightSprites[Mathf.FloorToInt(animationFrame)];
                break;
            case DIR_DOWN:
                sr.sprite = downSprites[Mathf.FloorToInt(animationFrame)];
                break;

        }

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
            case DIR_UP:
                offset.y = 1;
                break;

            case DIR_DOWN:
                offset.y = -1;
                break;

            case DIR_RIGHT:
                offset.x = 1;
                break;

            case DIR_LEFT:
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
            case DIR_UP:
                checkDir.y = 1;
                break;

            case DIR_DOWN:
                checkDir.y = -1;
                break;

            case DIR_RIGHT:
                checkDir.x = 1;
                break;

            case DIR_LEFT:
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
                Fungus.GameObjectVariable fGO = null;
                fGO = fc.GetVariable("Self") as Fungus.GameObjectVariable;
                fGO.Value = hitList[i].collider.gameObject;

                fc.ExecuteBlock("InteractStart");
                break;
            }//end if
        }//end for
    }//end method

            
}
