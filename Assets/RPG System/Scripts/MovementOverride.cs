using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementOverride : MonoBehaviour
{

    [SerializeField] int gridSize = 32;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.I)) this.transform.position += new Vector3(0, gridSize, 0);
        if (Input.GetKeyDown(KeyCode.J)) this.transform.position += new Vector3(-gridSize, 0, 0);
        if (Input.GetKeyDown(KeyCode.K)) this.transform.position += new Vector3(0, -gridSize, 0);
        if (Input.GetKeyDown(KeyCode.L)) this.transform.position += new Vector3(gridSize, 0, 0);
    }
}
