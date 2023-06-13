using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerButton : MonoBehaviour
{
    public GameObject wall;
    public void OnTriggerEnter(Collider other)
    {
        print("test");
        if (other.gameObject.tag == "Hand")
            Destroy(wall);    
    }
}
