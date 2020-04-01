using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverrideMovement : MonoBehaviour
{

    [SerializeField] int gridSize = 32;

    

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            this.transform.position += Vector3.up * gridSize;
        }
        else if (Input.GetKeyDown(KeyCode.K))
        {
            this.transform.position += Vector3.down * gridSize;
        }
        else if (Input.GetKeyDown(KeyCode.J))
        {
            this.transform.position += Vector3.right * -gridSize;
        }
        else if (Input.GetKeyDown(KeyCode.L))
        {
            this.transform.position += Vector3.right * gridSize;
        }       
    }
}
