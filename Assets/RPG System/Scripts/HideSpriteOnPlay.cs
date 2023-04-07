using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HideSpriteOnPlay : MonoBehaviour
{
    public bool hideOnPlay = false;

    // Start is called before the first frame update
    void Start()
    {
        if(hideOnPlay)
        {
            GetComponent<SpriteRenderer>().enabled = false;
        }
        Destroy(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
