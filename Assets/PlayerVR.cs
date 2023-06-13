using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVR : MonoBehaviour
{
    private MainManager manager;

    void Start()
    {
        manager = FindObjectOfType<MainManager>();
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("switch"))
        {
            manager.OpenAIDoor();
            manager.UpdateVR();
            manager.allowMovement = true;
        }
    }
}
