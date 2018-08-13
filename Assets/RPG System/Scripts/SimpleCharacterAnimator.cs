using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCharacterAnimator : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DoAnimation(CharController_RPG_Framework controler)
    {
        //animate and move if moving
        if (controler.isMoving)
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
}
