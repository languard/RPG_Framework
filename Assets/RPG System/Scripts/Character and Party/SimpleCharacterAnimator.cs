using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCharacterAnimator : MonoBehaviour {

    [SerializeField]
    Sprite[] downSprites;
    [SerializeField]
    Sprite[] leftSprites;
    [SerializeField]
    Sprite[] rightSprites;
    [SerializeField]
    Sprite[] upSprites;

    [SerializeField, Tooltip("For 32 pixel tiles, set around 0.1")]
    float animationSpeed = 1;

    float animationFrame = 0;
    int maxAnimationFrame;

    SpriteRenderer sr;

    // Use this for initialization
    void Start () {
        sr = GetComponent<SpriteRenderer>();
        maxAnimationFrame = downSprites.Length;

        GetComponent<CharController_RPG_Framework>().AnimationEvent += DoAnimation;
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

            
        }
        else
        {
            animationFrame = 0;
        }

        //update sprite
        switch (controler.moveDirection)
        {
            case StaticData.DIR_UP:
                sr.sprite = upSprites[Mathf.FloorToInt(animationFrame)];
                break;
            case StaticData.DIR_LEFT:
                sr.sprite = leftSprites[Mathf.FloorToInt(animationFrame)];
                break;
            case StaticData.DIR_RIGHT:
                sr.sprite = rightSprites[Mathf.FloorToInt(animationFrame)];
                break;
            case StaticData.DIR_DOWN:
                sr.sprite = downSprites[Mathf.FloorToInt(animationFrame)];
                break;

        }
    }
}
