using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject[] Blockages;
    public GameObject[] Walls;
    public GameObject[] Goals;
    public GameObject Tool;
    public GameObject box;
    public GameObject buttonRoom3;
    public GameObject buttonRoom1;
    public GameObject Agent;

    
    private Vector3[] spawnPositionsToolS6 = new Vector3[]
    {
        new Vector3(-12, 0.75f, 13),
        new Vector3(-12, 0.75f, 4),
        new Vector3(-20, 0.75f, -5),
    };

    private Vector3[] spawnPositionsButtonS1 = new Vector3[]
    {
        new Vector3(-8, 0.04f, -5),
        new Vector3(-1, 0.04f, -5),
        new Vector3(-8, 0.04f, 4),
    };

    private Vector3[] spawnPositionsButtonS3 = new Vector3[]
    {
        new Vector3(-14, 0.04f, -14),
        new Vector3(-8f, 0.04f, -15),
        new Vector3(-14, 0.04f, 14),
        new Vector3(-8f, 0.04f, 15)
    };

    public Transform Room6;

    [HideInInspector]
    public Vector3 agentSpawnPoint;

    [HideInInspector]
    public GameState State;

    [HideInInspector]
    public bool GateOpen;

    public static event Action<GameState> OnGameStateChanged;

    private void Awake(){
        Instance = this;
    }

    public void Start()
    {
        int y = SceneManager.GetActiveScene().buildIndex;
        if (y == 0)
            UpdateGameState(GameState.Stage1);
        else if(y == 1)
            UpdateGameState(GameState.Stage2);
        else if (y == 2)
            UpdateGameState(GameState.Stage5);
        else if (y == 3)
            UpdateGameState(GameState.Stage6);
        GateOpen = false;

        //if (State == GameState.Stage3)
        //    randomButtonPos();
        //else if (State == GameState.Stage6)
        //    ResetTool();
    }

    public void UpdateGameState(GameState newState){
        State = newState;
        int rotationIndex = Random.Range(0, 4);
        float yRotation = rotationIndex * 90f;
        Agent.transform.localRotation = Quaternion.Euler(0f, yRotation, 0f);

        switch (newState)
        {
            case GameState.Start:
                break;
            case GameState.Stage1:
                //agentSpawnPoint = new Vector3(0, 0.5f, 0);

                //training
                RandomSpawn();

                break;
            case GameState.Stage2:
                Goals[0].transform.localPosition = new Vector3(6.5f, -20, 0);
                Walls[0].transform.localPosition = new Vector3(6f, 3.5f, 0);
                //agentSpawnPoint = new Vector3(12, 0.5f, 0);

                //training
                RandomSpawn();
                break;
            case GameState.Stage3:
                Goals[1].transform.localPosition = new Vector3(6.5f, -20, 0);
                //agentSpawnPoint = new Vector3(27.5f, 0.5f, 0);

                //training
                RandomSpawn();

                Walls[1].transform.localPosition = new Vector3(6f, 3.5f, 0);
                //randomButtonPos();
                break;
            case GameState.Stage4:
                Goals[2].transform.localPosition = new Vector3(6.5f, -20, 0);
                //agentSpawnPoint = new Vector3(50, 0.5f, 0);

                //training
                RandomSpawn();

                Walls[2].transform.localPosition = new Vector3(6f, 3.5f, 0);
                break;
            case GameState.Stage5:
                Goals[3].transform.localPosition = new Vector3(6.5f, -20, 0);
                //agentSpawnPoint = new Vector3(72, 0.5f, 0);

                //for training
                RandomSpawn();

                Walls[3].transform.localPosition = new Vector3(6f, 3.5f, 0);
                break;
            case GameState.Stage6:
                Goals[4].transform.localPosition = new Vector3(6.5f, -20, 0);
                //agentSpawnPoint = new Vector3(100, 0.5f, 0);

                //for training
                RandomSpawn();

                Walls[4].transform.localPosition = new Vector3(6f, 3.5f, 0);
                break;
            case GameState.Stage7:
                Goals[5].transform.localPosition = new Vector3(6.5f, -20, 0);
                agentSpawnPoint = new Vector3(130, 0.5f, 0);
                Walls[5].transform.localPosition = new Vector3(6f, 3.5f, 0);
                break;
            case GameState.Loss:
                //agentSpawnPoint = new Vector3(0, 0.5f, 0);
                //respawnAgent();
                break;
            case GameState.Victory:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(newState), newState, null);
        }

        OnGameStateChanged?.Invoke(newState);
    }

    private void RandomSpawn()
    {
        int spawnIndex = Random.Range(0, 3);
        switch (State)
        {
            case GameState.Start:
                break;
            case GameState.Stage1:
                switch (spawnIndex)
                {
                    case 0:
                        agentSpawnPoint = new Vector3(0, 0.5f, 0);
                        break;
                    case 1:
                        agentSpawnPoint = new Vector3(-7.5f, 0.5f, 0);
                        break;
                    case 2:
                        agentSpawnPoint = new Vector3(-7.5f, 0.5f, 4f);
                        break;
                }
                break;
            case GameState.Stage2:
                switch (spawnIndex)
                {
                    case 0:
                        agentSpawnPoint = new Vector3(11f, 0.5f, 0);
                        break;
                    case 1:
                        agentSpawnPoint = new Vector3(12f, 0.5f, 4);
                        break;
                    case 2:
                        agentSpawnPoint = new Vector3(14f, 0.5f, 4f);
                        break;
                }
                break;
            case GameState.Stage3:
                switch (spawnIndex)
                {
                    case 0:
                        agentSpawnPoint = new Vector3(27.5f, 0.5f, 0);
                        break;
                    case 1:
                        agentSpawnPoint = new Vector3(27.5f, 0.5f, 4);
                        break;
                    case 2:
                        agentSpawnPoint = new Vector3(27.5f, 0.5f, -4.5f);
                        break;
                }
                break;
            case GameState.Stage4:
                switch (spawnIndex)
                {
                    case 0:
                        agentSpawnPoint = new Vector3(50, 0.5f, 0);
                        break;
                    case 1:
                        agentSpawnPoint = new Vector3(60, 0.5f, 0);
                        break;
                    case 2:
                        agentSpawnPoint = new Vector3(60, 0.5f, -5f);
                        break;
                }
                break;
            case GameState.Stage5:
                switch (spawnIndex)
                {
                    case 0:
                        agentSpawnPoint = new Vector3(72, 0.5f, 0);
                        break;
                    case 1:
                        agentSpawnPoint = new Vector3(72, 0.5f, 3.5f);
                        break;
                    case 2:
                        agentSpawnPoint = new Vector3(72, 0.5f, -4f);
                        break;
                }
                break;
            case GameState.Stage6:
                switch (spawnIndex)
                {
                    case 0:
                        agentSpawnPoint = new Vector3(100, 0.5f, 0);
                        break;
                    case 1:
                        agentSpawnPoint = new Vector3(115, 0.5f, 3.5f);
                        break;
                    case 2:
                        agentSpawnPoint = new Vector3(107.5f, 0.5f, -4f);
                        break;
                }
                break;
            case GameState.Stage7:
                break;
            case GameState.Loss:
                break;
            case GameState.Victory:
                break;
            default:
                break;
        }
    }

    public void Reset()
    {
        foreach (var wall in Walls)
            wall.transform.localPosition = new Vector3(6f, -20f, 0);
        foreach (var blockage in Blockages)
            blockage.transform.localPosition = new Vector3(5f, 3.5f, 0);
        foreach (var goal in Goals)
            goal.transform.localPosition = new Vector3(6.5f, 3.5f, 0);

        GateOpen = false;
        ResetBox();
        ResetTool();
    }

    public void moveBlockages()
    {
        switch (State)
        {
            case GameState.Start:
                break;
            case GameState.Stage1:
                Blockages[0].transform.localPosition = new Vector3(5f, -20f, 0);
                break;
            case GameState.Stage2:
                Blockages[1].transform.localPosition = new Vector3(5f, -20f, 0);
                break;
            case GameState.Stage3:
                Blockages[2].transform.localPosition = new Vector3(5f, -20f, 0);
                break;
            case GameState.Stage4:
                Blockages[3].transform.localPosition = new Vector3(5f, -20f, 0);
                break;
            case GameState.Stage5:
                Blockages[4].transform.localPosition = new Vector3(5f, -20f, 0);
                break;
            case GameState.Stage6:
                Blockages[5].transform.localPosition = new Vector3(5f, -20f, 0);
                break;
            case GameState.Stage7:
                Blockages[6].transform.localPosition = new Vector3(5f, -20f, 0);
                break;
            case GameState.Loss:
                break;
            case GameState.Victory:
                break;
            default:
                break;
        }
    }

    public void ResetTool()
    {
        int randomIndex = Random.Range(0, spawnPositionsToolS6.Length);
        Vector3 spawnPosition = spawnPositionsToolS6[randomIndex];
        Tool.transform.localPosition = spawnPosition;
        Tool.transform.parent = Room6.transform;
    }

    public void ResetBox()
    {
        box.transform.localPosition = new Vector3(-5, 1, -3);
    }

    public void SetGateTrue()
    {
        if (!GateOpen)
            GateOpen = true;
    }

    public void randomButtonPos()
    {
        int randomIndex = Random.Range(0, spawnPositionsButtonS3.Length);
        Vector3 spawnPosition = spawnPositionsButtonS3[randomIndex];
        buttonRoom3.transform.localPosition = spawnPosition;
    }
    public void randomButtonPosS1()
    {
        int randomIndex = Random.Range(0, spawnPositionsButtonS1.Length);
        Vector3 spawnPosition = spawnPositionsButtonS1[randomIndex];
        buttonRoom1.transform.localPosition = spawnPosition;
    }
}

public enum GameState
{
    Start,
    Stage1,
    Stage2,
    Stage3,
    Stage4,
    Stage5,
    Stage6,
    Stage7,
    Loss,
    Victory
}
