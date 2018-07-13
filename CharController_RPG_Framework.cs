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

    }

    // Update is called once per frame
    void FixedUpdate() {

        //determine if on grid
        //float deltaX = Mathf.Abs(transform.position.x - Mathf.Floor(transform.position.x));
        //float deltaY = Mathf.Abs(transform.position.y - Mathf.Floor(transform.position.y));

        isOnGrid = false;

        float deltaX = Mathf.Abs(Mathf.RoundToInt(transform.position.x / grid) - (transform.position.x / grid));
        float deltaY = Mathf.Abs(Mathf.RoundToInt(transform.position.y / grid) - (transform.position.y / grid));

        if (deltaX <= delta && deltaY <= delta) isOnGrid = true;

        //Vector3 temp = Vector3.zero;

        //if(deltaX <= delta)
        //{
        //    temp = transform.position;
        //    temp.x = Mathf.Floor(temp.x);
        //}
        //if (deltaY <= delta)
        //{
        //    temp = transform.position;
        //    temp.y = Mathf.Floor(temp.y);
        //}



        //movement
        if (isOnGrid)
        {
            isMoving = false;
            if (currentVertical < -0.2f)
            {
                //Move up
                //check for collision
                if (!CheckForCollision(DIR_DOWN))
                {
                    isMoving = true;
                    moveVector = Vector3.zero;
                    moveVector.y = -1;
                    animationDirection = DIR_DOWN;
                }
            }
            if (currentVertical > 0.2f)
            {
                //Move down
                if (!CheckForCollision(DIR_UP))
                {
                    isMoving = true;
                    moveVector = Vector3.zero;
                    moveVector.y = 1;
                    animationDirection = DIR_UP;
                }
            }
            if (currentHorizontal > 0.2f)
            {
                //Move right
                if (!CheckForCollision(DIR_RIGHT))
                {
                    isMoving = true;
                    moveVector = Vector3.zero;
                    moveVector.x = 1;
                    animationDirection = DIR_RIGHT;
                }
            }
            if (currentHorizontal < -0.2f)
            {
                //Move left
                if (!CheckForCollision(DIR_LEFT))
                {
                    isMoving = true;
                    moveVector = Vector3.zero;
                    moveVector.x = -1;
                    animationDirection = DIR_LEFT;
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

    bool CheckForCollision(int direction)
    {
        bool result = false;
        RaycastHit2D[] junk = new RaycastHit2D[5];


        Vector3 offset = Vector3.zero;

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

        offset = offset * grid;

        if (rb.Cast(offset, junk) > 0) result = true;

        return result;
    }
}
