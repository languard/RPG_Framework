using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Demo {

    static Demo _instance;

    public Demo Instance
    {
        get
        {
            if (_instance == null) _instance = new Demo();
            return _instance;
        }
    }
    private Demo()
    {

    }

    public void TestOfDoom()
    {
        throw new System.NotImplementedException("BOOOOOOM!");
    }

}
