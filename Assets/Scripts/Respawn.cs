using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    private Vector3 spawnpoint;
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        spawnpoint = this.transform.localPosition;
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        if (this.transform.localPosition.y < -1)
        {
            this.transform.localPosition = spawnpoint;
            rb.velocity = Vector3.zero;
            Debug.Log("test");
        }
    }
}
