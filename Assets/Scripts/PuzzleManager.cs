using System;
using UnityEngine;
using UnityEngine.Events;

public class PuzzleManager : MonoBehaviour
{
    [SerializeField]
    private int numOfTasksToComplete;

    private int CurrentCompletedTasks = 0;

    public GameObject WallToDestroy;

    public void CompletedPuzzlePiece()
    {
        CurrentCompletedTasks++;
        CheckForPuzzleComplete();
    }

    private void CheckForPuzzleComplete()
    {
        if(CurrentCompletedTasks >= numOfTasksToComplete)
        {
            //Destroy(WallToDestroy);
            WallToDestroy.SetActive(false);
        }
    }

    public void PuzzlePieceRemoved()
    {
        CurrentCompletedTasks--;
    }
}
