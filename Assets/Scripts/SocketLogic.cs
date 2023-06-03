using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SocketLogic : MonoBehaviour
{
    [SerializeField]
    private PuzzleManager puzzleManager;

    [SerializeField]
    private Transform correctPuzzlePiece;

    private XRSocketInteractor socket;

    private void Awake()
    {
        socket = GetComponent<XRSocketInteractor>();
    }

    private void OnEnable()
    {
        socket.selectEntered.AddListener(ObjectInSocket);
        socket.selectExited.AddListener(ObjectOutSocket);
    }

    private void OnDisable()
    {
        socket.selectEntered.RemoveListener(ObjectInSocket);
        socket.selectExited.RemoveListener(ObjectOutSocket);
    }

    private void ObjectInSocket(SelectEnterEventArgs arg0)
    {
        var objectName = arg0.interactable;
        if(objectName.transform.name == correctPuzzlePiece.name)
        {
            puzzleManager.CompletedPuzzlePiece();
        }
    }

    private void ObjectOutSocket(SelectExitEventArgs arg0)
    {
        var objectName = arg0.interactable;
        if (objectName.transform.name == correctPuzzlePiece.name)
        {
            puzzleManager.PuzzlePieceRemoved();
        }
    }
}
