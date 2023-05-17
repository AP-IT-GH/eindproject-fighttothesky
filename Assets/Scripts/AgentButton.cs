using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using static UnityEditor.Searcher.SearcherWindow.Alignment;
using static UnityEngine.GraphicsBuffer;

public class AgentButton : Agent
{
    public GameObject Button;
    //private Rigidbody button;

    public GameObject Goal;
    //private Rigidbody goal;

    public GameObject BadWall;
    //private Rigidbody badWall;

    private Rigidbody rb;

    public float speedMultiplier = 0.1f;
    public float rotationSpeed = 10f;
    public float buttonReward = 12f;
    public float badWallReward = -4.0f;
    public float WallReward = 0f;
    public float FallOffReward = -5f;


    private bool BadWallExists = true;

    private Vector3 spawnpoint = new Vector3(0, 0.5f, 0);
    private Vector3 spawnpointBadWall = new Vector3(0f, 2f, -3);


    public override void Initialize()
    {
        rb = this.GetComponent<Rigidbody>();
        //button = Button.GetComponent<Rigidbody>();
        //goal = Goal.GetComponent<Rigidbody>();
        //badWall = BadWall.GetComponent<Rigidbody>();
        spawnpoint = transform.localPosition;
    }
    public override void OnEpisodeBegin()
    {
        this.transform.localPosition = spawnpoint;

        BadWallExists = true;

        BadWall.transform.localPosition = spawnpointBadWall;

        buttonReward = 12f;
        badWallReward = -4.0f;
        WallReward = 0f;
        FallOffReward = -5f;

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(Button.transform.localPosition);
        sensor.AddObservation(Goal.transform.localPosition);
        sensor.AddObservation(BadWall.transform.localPosition);

        sensor.AddObservation(rb.velocity);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Acties, size = 2
        Vector3 controlSignal = Vector3.zero;
        controlSignal.x = actionBuffers.ContinuousActions[0];
        controlSignal.z = actionBuffers.ContinuousActions[1];
        transform.Translate(controlSignal * speedMultiplier);
        if (BadWallExists == true)
        {
            float distanceToTarget = Vector3.Distance(this.transform.localPosition, Button.transform.localPosition);
            if (distanceToTarget < 1.42f)
            {
                SetReward(0.2f);
            }
        }
        if (BadWallExists == false)
        {
            WallReward = 12;
        }
        if (this.transform.localPosition.y < 0)
        {
            print("afgevallen");
            AddReward(FallOffReward);
            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("badWall"))
        {
            AddReward(badWallReward);
            EndEpisode();
        }
        if (collision.gameObject.CompareTag("goToWall"))
        {
            AddReward(WallReward);
            EndEpisode();
            // eens de nieuwe kamer binnen doe .. (moet nog coderen)
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("goodButton"))
        {
            AddReward(buttonReward);
            buttonReward = 0;
            BadWall.transform.localPosition = new Vector3(5.5f, -20f, 0);
            badWallReward = 0;
            BadWallExists = false;
        }
    }



}
