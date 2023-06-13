using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Barracuda;
using Unity.MLAgents.Policies;
using UnityEngine;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class MainManager : MonoBehaviour
{
    public static MainManager Instance;

    public GameObject[] preFabsAI;
    public GameObject[] preFabsVR;
    public NNModel[] brains;
    public GameObject AgentPreFab;
    public bool allowMovement;

    private GameObject Agent;
    private List<GameObject> activeRoomsAI = new List<GameObject>();
    public int[] prefabIndexesAI = new int[5];
    private List<GameObject> activeRoomsVR = new List<GameObject>();
    public int[] prefabIndexesVR = new int[5];

    [HideInInspector]
    public NNModel currentBrain;
    [HideInInspector]
    public Vector3 agentSpawn;
    [HideInInspector]
    public Vector3 agentRoomSpawn = new Vector3(0, 0f, 0);
    [HideInInspector]
    public Vector3 playerRoomSpawn = new Vector3(18, 0f, 0);
    [HideInInspector]
    public Level level;
    [HideInInspector]
    public VRState VRstate;
    [HideInInspector]
    public AIState AIstate;
    [HideInInspector]
    public AIState nextSate;
    [HideInInspector]
    public bool GateOpen;

    private bool openedVR;
    private bool openedAI;

    public static event Action<Level> OnLevelChanged;
    public static event Action<AIState> OnAIStateChanged;
    public static event Action<VRState> OnVRStateChanged;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        GateOpen = false;
        level = Level.Level1;
        VRstate = VRState.Room1;
        agentSpawn = new Vector3(-10f, 0.5f, 0);

        GameObject AI1 = preFabsAI[0];
        activeRoomsAI.Add(Instantiate(AI1, agentRoomSpawn, Quaternion.Euler(0, 90, 0)));
        GameObject VR1 = preFabsVR[0];
        activeRoomsVR.Add(Instantiate(VR1, playerRoomSpawn, Quaternion.identity));
        currentBrain = brains[0];
        Agent = Instantiate(AgentPreFab, agentSpawn, Quaternion.Euler(0, -180, 0));
        Agent.transform.SetParent(activeRoomsAI[0].transform);
        

        

        allowMovement = true;
        openedVR = false;
        openedAI = false;

        // Spawn 5 random rooms
        int ZAI = 0;
        int ZVR = 0;
        for (int i = 0; i < 5; i++)
        {
            //AI
            int randomIndexAI = Random.Range(1, preFabsAI.Length);
            prefabIndexesAI[i] = randomIndexAI;
            if (randomIndexAI == 3)
                ZAI -= 5;
            ZAI -= 25;
            //GameObject AIroom = preFabsAI[randomIndexAI];
            agentRoomSpawn.z = ZAI;
            activeRoomsAI.Add(Instantiate(preFabsAI[randomIndexAI], agentRoomSpawn, Quaternion.Euler(0, 90, 0)));

            //VR
            int randomIndexVR = Random.Range(1, preFabsVR.Length);
            prefabIndexesVR[i] = randomIndexVR;
            if (randomIndexVR == 4)
                ZVR -= 10;
            ZVR -= 10;
            //GameObject VRroom = preFabsVR[randomIndexVR];
            playerRoomSpawn.z = ZVR;
            activeRoomsVR.Add(Instantiate(preFabsVR[randomIndexVR], playerRoomSpawn, Quaternion.identity));
        }
    }

    public void UpdateLevel(Level newLevel)
    {


        OnLevelChanged?.Invoke(newLevel);
    }

    public void UpdateAI()
    {
        // Depending on prefab of next room --> update ai state
        // Parameter empty 
        int currentIndex = (int)VRstate;
        int nextIndex = (currentIndex + 1) % prefabIndexesVR.Length;
        VRstate = (VRState)nextIndex;

        //OnAIStateChanged?.Invoke();
    }

    public void UpdateVR()
    {
        int currentIndex = (int)AIstate;
        int nextIndex = (currentIndex + 1) % prefabIndexesAI.Length;
        AIstate = (AIState)nextIndex;

    }

    public void OpenVRDoor()
    {
        if (!openedVR)
        {
            for (int i = 0; i < 5; i++)
            {
                int currentIndex = (int)AIstate;
                if (currentIndex == i)
                {
                    GameObject door = activeRoomsVR[0].transform.Find("Deur")?.gameObject;
                    Vector3 newLocalPosition = door.transform.localPosition;
                    newLocalPosition.y -= 20.0f; // Move it down by 1 unit
                    door.transform.localPosition = newLocalPosition;
                    openedVR = true;
                    allowMovement = false;
                }
            }
        }

        //// AI can open VR door
        //switch (vRState)
        //{
        //    case VRState.Room1:
        //        if (!openedVR)
        //        {
        //            GameObject door = activeRoomsVR[0].transform.Find("Deur")?.gameObject;
        //            //Transform door = preFabsAI[0].transform.Find("Blockage");
        //            Vector3 newLocalPosition = door.transform.localPosition;
        //            newLocalPosition.y -= 20.0f; // Move it down by 1 unit
        //            door.transform.localPosition = newLocalPosition;
        //            openedVR = true;
        //            allowMovement = false;
        //        }
        //        break;
        //    case VRState.Room2:
        //        break;
        //    case VRState.Room3:
        //        break;
        //    case VRState.Room4:
        //        break;
        //    case VRState.Room5:
        //        break;
        //    default:
        //        break;
        //}
    }

    public void OpenAIDoor()
    {
        if (!openedAI)
        {
            for (int i = 0; i < 5; i++)
            {
                int currentIndex = (int)AIstate;
                if (currentIndex == i)
                {
                    GameObject door = activeRoomsAI[i].transform.Find("Blockage")?.gameObject;
                    Vector3 newLocalPosition = door.transform.localPosition;
                    newLocalPosition.y -= 20.0f; // Move it down by 1 unit
                    door.transform.localPosition = newLocalPosition;
                    openedAI = true;
                    GateOpen = true;
                    allowMovement = true;
                }
            }
        }
        //// VR can open AI door
        //switch (AIstate)
        //{
        //    case AIState.Room1:
        //        if (!openedAI)
        //        {
        //            for (int i = 0; i < 5; i++)
        //            {
        //                int currentIndex = (int)AIstate;
        //                if (currentIndex == i)
        //                {
        //                    GameObject door = activeRoomsAI[0].transform.Find("Blockage")?.gameObject;
        //                    Vector3 newLocalPosition = door.transform.localPosition;
        //                    newLocalPosition.y -= 20.0f; // Move it down by 1 unit
        //                    door.transform.localPosition = newLocalPosition;
        //                    openedAI = true;
        //                    GateOpen = true;
        //                    allowMovement = true;
        //                }
        //            }
        //        }
        //        break;
        //    case AIState.Room2:
        //        if (!openedAI)
        //        {
        //            for (int i = 0; i < 5; i++)
        //            {
        //                int currentIndex = (int)AIstate;
        //                if (currentIndex == i)
        //                {
        //                    GameObject door = activeRoomsAI[i].transform.Find("Blockage")?.gameObject;
        //                    Vector3 newLocalPosition = door.transform.localPosition;
        //                    newLocalPosition.y -= 20.0f; // Move it down by 1 unit
        //                    door.transform.localPosition = newLocalPosition;
        //                    openedAI = true;
        //                    GateOpen = true;
        //                    allowMovement = true;
        //                }
        //            }
        //        }
        //        break;
        //    case AIState.Room3:
        //        if (!openedAI)
        //        {
        //            for (int i = 0; i < 5; i++)
        //            {
        //                int currentIndex = (int)AIstate;
        //                if (currentIndex == i)
        //                {
        //                    GameObject door = activeRoomsAI[i].transform.Find("Blockage")?.gameObject;
        //                    Vector3 newLocalPosition = door.transform.localPosition;
        //                    newLocalPosition.y -= 20.0f; // Move it down by 1 unit
        //                    door.transform.localPosition = newLocalPosition;
        //                    openedAI = true;
        //                    GateOpen = true;
        //                    allowMovement = true;
        //                }
        //            }
        //        }
        //        break;
        //    case AIState.Room4:
        //        if (!openedAI)
        //        {
        //            for (int i = 0; i < 5; i++)
        //            {
        //                int currentIndex = (int)AIstate;
        //                if (currentIndex == i)
        //                {
        //                    GameObject door = activeRoomsAI[i].transform.Find("Blockage")?.gameObject;
        //                    Vector3 newLocalPosition = door.transform.localPosition;
        //                    newLocalPosition.y -= 20.0f; // Move it down by 1 unit
        //                    door.transform.localPosition = newLocalPosition;
        //                    openedAI = true;
        //                    GateOpen = true;
        //                    allowMovement = true;
        //                }
        //            }
        //        }
        //        break;
        //    default:
        //        break;
        //}
    }

    //todo
    public void ResetTool()
    {
        //int randomIndex = Random.Range(0, spawnPositionsToolS6.Length);
        //Vector3 spawnPosition = spawnPositionsToolS6[randomIndex];
        //Tool.transform.localPosition = spawnPosition;
        //Tool.transform.parent = Room6.transform;
    }

    //todo
    public void ResetBox()
{
        //box.transform.localPosition = new Vector3(-5, 1, -3);
    }

    // Move blockages
    public void moveBlockages()
    {
        // probably get object from object (bc we spawn the room) PER LEVEL/STATE
        // then move it down, NOT ALL
    }

    public void SetGateTrue()
    {
        if (!GateOpen)
            GateOpen = true;
    }

    public void Reset()
    {   // reset enkel gebruikt vr agent fouten ? 
        GateOpen = false;
        if (openedVR)
        {
            openedVR = false;
            GameObject door = activeRoomsVR[0].transform.Find("onzichtbareMuur")?.gameObject;
            //Transform door = preFabsAI[0].transform.Find("Blockage");
            Vector3 newLocalPosition = door.transform.localPosition;
            newLocalPosition.y = 2.5f; 
            door.transform.localPosition = newLocalPosition;
            allowMovement = true;
        }
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
    Room4
}

public enum VRState
{
    Room1,
    Room2,
    Room3,
    Room4,
    Room5
}
