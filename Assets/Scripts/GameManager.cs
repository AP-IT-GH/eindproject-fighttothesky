using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public GameObject[] Blockages;
    public GameObject[] Switches;
    public GameObject[] Walls;
    public GameObject[] Goals;
    public GameObject Tool;
    public GameObject box;
    //public GameObject Agent;
    public Transform Room6;

    [HideInInspector]
    public Vector3 agentSpawnPoint;

    [HideInInspector]
    public GameState State;

    public static event Action<GameState> OnGameStateChanged;

    private void Awake(){
        Instance = this;
    }

    public void Start()
    {
        UpdateGameState(GameState.Stage4);        
    }

    public void UpdateGameState(GameState newState){
        State = newState;

        switch (newState)
        {
            case GameState.Start:
                break;
            case GameState.Stage1:
                agentSpawnPoint = new Vector3(0, 0.5f, 0);
                break;
            case GameState.Stage2:
                Goals[0].transform.localPosition = new Vector3(6.5f, -20, 0);
                Walls[0].transform.localPosition = new Vector3(6f, 3.5f, 0);
                agentSpawnPoint = new Vector3(12, 0.5f, 0);
                Switches[0].transform.localPosition = new Vector3(8f, -20f, 0);
                break;
            case GameState.Stage3:
                Goals[1].transform.localPosition = new Vector3(6.5f, -20, 0);
                agentSpawnPoint = new Vector3(27.5f, 0.5f, 0);
                Walls[1].transform.localPosition = new Vector3(6f, 3.5f, 0);
                Switches[1].transform.localPosition = new Vector3(8, -20f, 0);
                break;
            case GameState.Stage4:
                Goals[2].transform.localPosition = new Vector3(6.5f, -20, 0);
                agentSpawnPoint = new Vector3(50, 0.5f, 0);
                Walls[2].transform.localPosition = new Vector3(6f, 3.5f, 0);
                Switches[2].transform.localPosition = new Vector3(8f, -20f, 0);
                break;
            case GameState.Stage5:
                Goals[3].transform.localPosition = new Vector3(6.5f, -20, 0);
                agentSpawnPoint = new Vector3(72, 0.5f, 0);
                Walls[3].transform.localPosition = new Vector3(6f, 3.5f, 0);
                Switches[3].transform.localPosition = new Vector3(8f, -20f, 0);
                break;
            case GameState.Stage6:
                Goals[4].transform.localPosition = new Vector3(6.5f, -20, 0);
                agentSpawnPoint = new Vector3(100, 0.5f, 0);
                Walls[4].transform.localPosition = new Vector3(6f, 3.5f, 0);
                Switches[4].transform.localPosition = new Vector3(8f, -20f, 0);
                break;
            case GameState.Stage7:
                Goals[5].transform.localPosition = new Vector3(6.5f, -20, 0);
                agentSpawnPoint = new Vector3(130, 0.5f, 0);
                Walls[5].transform.localPosition = new Vector3(6f, 3.5f, 0);
                Switches[5].transform.localPosition = new Vector3(8f, -20f, 0);
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

    public void Reset()
    {
        foreach (var wall in Walls)
            wall.transform.localPosition = new Vector3(6f, -20f, 0);
        foreach (var swich in Switches)
            swich.transform.localPosition = new Vector3(7f, 3.5f, 0);
        foreach (var blockage in Blockages)
            blockage.transform.localPosition = new Vector3(5f, 3.5f, 0);
        foreach (var goal in Goals)
            goal.transform.localPosition = new Vector3(6.5f, 3.5f, 0);
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
        Tool.transform.localPosition = new Vector3(-12, 0.75f, 21);
        Tool.transform.parent = Room6.transform;
    }

    public void ResetBox()
    {
        box.transform.localPosition = new Vector3(-5, 1, -3);
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
