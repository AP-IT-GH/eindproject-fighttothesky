using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentRaycast : Agent
{
    //public GameObject Button;
    //public GameObject Goal;
    public GameObject Blockage;

    public Score gameManager;

    private Rigidbody rb;

    // rewards
    public float buttonReward = 2f;
    public float badWallReward = -4.0f;
    public float goalReward = 0f;
    public float fallOffReward = -5f;

    // speed & rotation
    public float speedMultiplier = 0.1f;
    public float rotationSpeed = 5f;

    // variable for etc
    private float episodeDuration = 60f; // Duration of the episode in seconds
    private float elapsedTime = 0f;// Elapsed time since the episode started
    //private bool BadWallExists = true;

    // Jump related variables
    private bool isJumping = false;
    private float jumpForce = 7.5f;
    private float jumpCooldown = 1f;
    private float jumpTimer = 0f;

    //spawnPoints
    private Vector3 spawnpoint = new Vector3(0, 0.5f, 0);
    private Vector3 spawnpointBlockage = new Vector3(5f, 3.5f, 0);

    // Distances
    //float currentDistanceToGoal = 0;
    //float currentDistanceToButton = 0;
    //float currentDistanceToBlockage = 0;

    //float previousDistanceToGoal;
    //float previousDistanceToButton;
    //float previousDistanceToBlockage;
    public override void Initialize()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

        spawnpoint = transform.localPosition;
    }
    public override void OnEpisodeBegin()
    {
        this.transform.localPosition = spawnpoint;
        this.transform.localRotation = Quaternion.identity;

        //BadWallExists = true;

        Blockage.transform.localPosition = spawnpointBlockage;

        // Reset the environment and agent state
        elapsedTime = 0f;

        // Reset rewards
        buttonReward = 2.5f;
        badWallReward = -4.0f;
        goalReward = 0f;
        fallOffReward = -5f;

        // Reset any jump-related variables
        isJumping = false;
        jumpTimer = 0f;

        // Get distance to objects
        //previousDistanceToGoal = Vector3.Distance(this.transform.localPosition, Goal.transform.localPosition);
        //previousDistanceToButton = Vector3.Distance(this.transform.localPosition, Blockage.transform.localPosition);
        //previousDistanceToBlockage = Vector3.Distance(this.transform.localPosition, Button.transform.localPosition);

    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(rb.velocity);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //currentDistanceToGoal = Vector3.Distance(this.transform.localPosition, Goal.transform.localPosition);
        //currentDistanceToBlockage = Vector3.Distance(this.transform.localPosition, Blockage.transform.localPosition);
        //currentDistanceToButton = Vector3.Distance(this.transform.localPosition, Button.transform.localPosition);

        // Get the horizontal movement, forward/backward movement, and rotation signals
        float horizontalMovement = actionBuffers.ContinuousActions[0];
        float verticalMovement = actionBuffers.ContinuousActions[1];
        float rotationSignal = actionBuffers.ContinuousActions[2];
        float jumpMovement = actionBuffers.ContinuousActions[3];

        // Apply movement and rotation
        ApplyMovement(horizontalMovement, verticalMovement);
        ApplyRotation(rotationSignal);
        //Jump(jumpMovement);

        // Handle jumping
        if (jumpMovement > 0.6)
            Jump();

        // Increase the elapsed time
        elapsedTime += Time.deltaTime;

        // Check if the desired duration has been reached
        if (elapsedTime >= episodeDuration)
        {
            // End the episode
            SetReward(-2f);
            EndEpisode();
        }

        // Movement Origineel
        {
        //    // Acties, size = 2
        //    Vector3 controlSignal = Vector3.zero;
        //    controlSignal.x = actionBuffers.ContinuousActions[0];
        //    controlSignal.z = actionBuffers.ContinuousActions[1];
        //    float rotationSignal = actionBuffers.ContinuousActions[2];

        //    // move forward/back and left/right 
        //    transform.Translate(controlSignal.x * speedMultiplier, 0f, controlSignal.z * (speedMultiplier / 2));

        //    Vector3 newRotation = transform.rotation.eulerAngles;
        //    newRotation.y += rotationSignal * rotationSpeed * Time.fixedDeltaTime;
        //    transform.rotation = Quaternion.Euler(newRotation);
        }

        //if (BadWallExists == true)
        //{
        //    // negatieve beloning blockage
        //    if (currentDistanceToBlockage < previousDistanceToBlockage)
        //        AddReward(-0.01f);
        //    else if (currentDistanceToBlockage > previousDistanceToBlockage)
        //        AddReward(0.01f);

        //    // beloning button
        //    if (currentDistanceToButton < previousDistanceToButton)
        //        AddReward(0.02f);
        //    else if (currentDistanceToButton > previousDistanceToButton)
        //            AddReward(-0.02f);
        //}
        //else if (BadWallExists == false)
        //{
        //    // beloning goal
        //    if (currentDistanceToGoal < previousDistanceToGoal)
        //        AddReward(0.02f);
        //    else if (currentDistanceToGoal > previousDistanceToGoal)
        //        AddReward(-0.02f);
        //}
        if (this.transform.localPosition.y < 0)
        {
            print("afgevallen");
            SetReward(fallOffReward);
            EndEpisode();
        }

        //previousDistanceToGoal = currentDistanceToGoal;
        //previousDistanceToButton = currentDistanceToButton;
        //previousDistanceToBlockage = currentDistanceToBlockage;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("blockage"))
        {
            SetReward(badWallReward*2);
            EndEpisode();
        }
        if (collision.gameObject.CompareTag("goal"))
        {
            gameManager.score++;
            gameManager.UpdateScoreText();
            SetReward(goalReward*2);
            EndEpisode();
        }
        if (collision.gameObject.CompareTag("Ground"))
        {
            isJumping = false;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("button"))
        {
            AddReward(buttonReward);
            buttonReward = 0;
            goalReward = 5;
            badWallReward = 0;
            Blockage.transform.localPosition = new Vector3(5f, -20f, 0);
            //BadWallExists = false;
        }
    }

    // Apply movement based on horizontal and vertical input
    private void ApplyMovement(float horizontalMovement, float verticalMovement)
    {
        // Move the agent based on input signals
        float movementSpeed = 2f; // Adjust the movement speed as needed
        Vector3 movement = new Vector3(horizontalMovement, 0f, verticalMovement);
        movement.Normalize();
        transform.Translate(movement * Time.deltaTime * movementSpeed, Space.World);
    }

    // Apply rotation based on rotation input
    private void ApplyRotation(float rotationSignal)
    {
        // Rotate the agent based on input signal
        float rotationSpeed = 100f; // Adjust the rotation speed as needed
        transform.Rotate(0f, rotationSignal * rotationSpeed * Time.deltaTime, 0f);
    }

    // Make the agent jump
    private void Jump()
    {
        if (Time.time >= jumpTimer && isJumping == false)
        {
            // Apply jump force
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);

            // Set jumping flag and start jump cooldown
            isJumping = true;
            jumpTimer = Time.time + jumpCooldown;
        }
    }

    private void Update()
    {
        
    }
}
