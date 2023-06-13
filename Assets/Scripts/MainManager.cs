using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

public enum Level
{
    Start,
    Level1,
    Level2,
    Level3,
    Level4,
    Level5,
    Level6,
    Level7,
    Level8,
    Level9,
    Level10,
    Finish
}

public enum AIState
{
    Room1,
    Room2,
    Room3,
    Room4,
    Wait
}

public enum VRState
{
    Room1,
    Room2,
    Room3,
    Room4,
    Room5
}
