using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using static UnityEditor.Searcher.SearcherWindow.Alignment;

public class prisonAgent : Agent
{
    public Transform Button;
    public Transform Goal;
    public Transform BlockagePos;
    public GameObject[] Borders;
    public GameObject BlockPrefab;

    public float speedMultiplier = 0.1f;
    public float rotationSpeed = 10f;
    public float jumpForce = 200f;
    public float buttonReward = 12f;
    public float badWallReward = -4.0f;
    public float goToWallReward = 0f;
    public float borderWallReward = -2.0f;
    public bool isGrounded;

    Rigidbody rBody;
    private GameObject Blockage;
    private bool doesBlockageExist;
    private float timeSinceStart = 0f;
    private const float badRewardInterval = 25f; // interval for giving bad reward
    private const float badRewardAmount = -0.5f; // amount of negative reward to give

    void Start()
    {
        rBody = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        this.transform.localPosition = new Vector3(0, 0.5f, 0);
        buttonReward = 12.0f;
        goToWallReward = 0f;
        // if there's no blockage yet, spawn it
        if (Blockage == null)
        {
            Blockage = Instantiate(BlockPrefab);
            doesBlockageExist = true;
        }     
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target en Agent posities
        sensor.AddObservation(this.transform.localPosition);

        // Other positions
        sensor.AddObservation(Button.localPosition);        
        sensor.AddObservation(Goal.localPosition);

        sensor.AddObservation(BlockagePos.localPosition);
        sensor.AddObservation(doesBlockageExist);
        foreach (GameObject border in Borders)
        {
            sensor.AddObservation(border.transform.localPosition);
        }

        // Agent velocity
        sensor.AddObservation(rBody.velocity);
        //sensor.AddObservation(rBody.velocity.z);
        //sensor.AddObservation(rBody.velocity.y);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Initialize previous distance to be the current distance to the target
        float previousDistanceToButton = Vector3.Distance(transform.localPosition, Button.localPosition);
        float previousDistanceToBlockage = Vector3.Distance(transform.localPosition, BlockagePos.localPosition);
        float previousDistanceToGoal = Vector3.Distance(transform.localPosition, Goal.localPosition);

        // movement
        {
            // Get the continuous actions
            Vector3 controlSignal = Vector3.zero;
            controlSignal.x = actionBuffers.ContinuousActions[0];
            controlSignal.z = actionBuffers.ContinuousActions[1];
            float rotationSignal = actionBuffers.ContinuousActions[2];

            // Rotation
            Vector3 newRotation = transform.rotation.eulerAngles;
            newRotation.y += rotationSignal * rotationSpeed * Time.fixedDeltaTime;
            transform.rotation = Quaternion.Euler(newRotation);

            // Move the agent and lock its rotation
            rBody.AddForce(controlSignal * speedMultiplier);
            transform.Translate(controlSignal * speedMultiplier);
            transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);

            // Get the discrete action for jumping and apply force
            if (actionBuffers.DiscreteActions[0] == 1 && isGrounded)
            {
                rBody.AddForce(Vector3.up * jumpForce);
                isGrounded = false;
            }
        }

        // Calculate the current distance to the target
        float currentDistanceToButton = Vector3.Distance(this.transform.localPosition, Button.localPosition);
        float currentDistanceToBlockage = Vector3.Distance(this.transform.localPosition, BlockagePos.localPosition);
        float currentDistanceToGoal = Vector3.Distance(this.transform.localPosition, Goal.localPosition);

        // check if agent is close to any of the border
        foreach (GameObject border in Borders)
        {
            float distanceToBorder = Vector3.Distance(transform.localPosition, border.transform.localPosition);
            if (distanceToBorder < 1f)
                SetReward(-0.5f);
        }

        // if agent can't continue yet
        if (Blockage != null)
        {
            // go to button
            if (currentDistanceToButton > previousDistanceToButton)
                SetReward(-1.5f);
            else if (currentDistanceToButton < previousDistanceToButton)
                SetReward(1.5f);

            // going close to blockage is bad
            if (currentDistanceToBlockage < 1f)
                SetReward(-1f);

        }
        // if next room is available
        else if (Blockage == null)
        {
            // go to goal
            if (currentDistanceToGoal > previousDistanceToGoal)
                SetReward(-1.0f);
            else if (currentDistanceToGoal < previousDistanceToGoal)
                SetReward(1.0f);
        }


        // Van het platform gevallen?
        if (this.transform.localPosition.y < 0)
        {
            SetReward(-5f);
            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // kan alleen springen als het op de grond staat (mag niet springen als hij op een doos staat ?)
        if (collision.gameObject.CompareTag("Ground"))
            isGrounded = true;

        if (collision.gameObject.CompareTag("badWall"))
        {
            SetReward(badWallReward);
            EndEpisode();
        }

        if (collision.gameObject.CompareTag("goToWall"))
        {
            SetReward(goToWallReward);
            EndEpisode();
            // eens de nieuwe kamer binnen doe .. (moet nog coderen)
        }
            
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("goodButton"))
        {
            SetReward(buttonReward);
            Destroy(Blockage);
            Blockage = null;
            doesBlockageExist = false;

            buttonReward = 0f;
            goToWallReward = 12f;
            print("button reward = " + buttonReward);
            print("goal reward = " + goToWallReward);

        }

        // Check if the agent is colliding with any of the border triggers
        if (other.gameObject.CompareTag("borderWall"))
        {
            SetReward(borderWallReward);
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Get the continuous actions from input
        var continuousActionsOut = actionsOut.ContinuousActions;
        continuousActionsOut[0] = Input.GetAxis("Horizontal");
        continuousActionsOut[1] = Input.GetAxis("Vertical");
        continuousActionsOut[2] = Input.GetAxis("Rotation");

        // Get the discrete action from input for jumping
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    private void Update()
    {
        timeSinceStart += Time.deltaTime;

        if (timeSinceStart >= badRewardInterval)
        {
            SetReward(badRewardAmount);
            timeSinceStart = 0f; // reset timeSinceStart
            EndEpisode();
        }
    }
}