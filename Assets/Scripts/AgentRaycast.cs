using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;

public class AgentRaycast : Agent
{
    public Score scoreManager;

    private Rigidbody rb;

    // rewards
    public float buttonReward = 4f;
    public float badWallReward = -4.0f;
    public float goalReward = 0f;
    public float fallOffReward = -5f;
    public float platformReward = 0.2f;
    public float toolReward = 0.5f;
    public float basketReward = 0.5f;
    public float switchReward = 1.0f;

    // speed & rotation
    public float speedMultiplier = 0.1f;
    public float rotationSpeed = 5f;

    // variable for etc
    private float episodeDuration = 60f; // Duration of the episode in seconds
    private float elapsedTime = 0f; // Elapsed time since the episode started
    private Vector3 agentSpawnPosition;
    private bool droppedOff = false;

    // Jump related variables
    private bool isJumping = false;
    private float jumpForce = 7.5f;

    public override void Initialize()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        GameManager.Instance.Start();
        agentSpawnPosition = GameManager.Instance.agentSpawnPoint;
        //spawnpoint = transform.localPosition;
    }
    public override void OnEpisodeBegin()
    {
        this.transform.localPosition = agentSpawnPosition;
        this.transform.localRotation = Quaternion.identity;


        // Reset the environment and agent state
        elapsedTime = 0f;
        droppedOff = false;

        // Reset rewards
        buttonReward = 4f;
        badWallReward = -4.0f;
        goalReward = 0f;
        fallOffReward = -5f;
        platformReward = 0.2f;
        toolReward = 0.5f;
        GameManager.Instance.Reset();


        // Reset any jump-related variables
        isJumping = false;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(rb.velocity);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Get the horizontal movement, forward/backward movement, and rotation signals
        float horizontalMovement = actionBuffers.ContinuousActions[0];
        float verticalMovement = actionBuffers.ContinuousActions[1];
        float rotationSignal = actionBuffers.ContinuousActions[2];
        float jumpMovement = actionBuffers.ContinuousActions[3];

        // Apply movement and rotation
        ApplyMovement(horizontalMovement, verticalMovement);
        ApplyRotation(rotationSignal);

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

        if (this.transform.localPosition.y < 0)
        {
            // if tool fell, reset position
            if (GameManager.Instance.Tool.transform.localPosition.y < 0)
                GameManager.Instance.ResetTool();
            else if (GameManager.Instance.State == GameState.Stage5)
                GameManager.Instance.ResetBox();

            // punish for falling
            print("afgevallen");
            SetReward(fallOffReward);
            EndEpisode();
        }
        if (GameManager.Instance.box.transform.localPosition.y < 0)
            GameManager.Instance.ResetBox();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("blockage"))
        {
            SetReward(badWallReward);
            if (GameManager.Instance.State == GameState.Stage5)
                GameManager.Instance.ResetBox();
            EndEpisode();
        }
        else if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("box") || collision.gameObject.CompareTag("platform"))
        {
            isJumping = false;
        }
        else if (collision.gameObject.CompareTag("platform"))
        {
            AddReward(platformReward);
            platformReward = 0;
        }
        else if (collision.gameObject.CompareTag("tool"))
        {
            AddReward(toolReward);
            toolReward = 0;

            // Stick the tool to the agent if it doesn't have a tool already and hasn't been dropped off
            if (!droppedOff && !HasTool())
            {
                collision.transform.parent = transform;
                AddReward(toolReward);
                toolReward = 0;
            }
        }
        else if (collision.gameObject.CompareTag("basket"))
        {
            if (HasTool())
            {
                // Drop the tool if colliding with a basket and the agent has the tool
                Transform tool = GetTool();
                if (tool != null)
                    tool.parent = null;

                droppedOff = true;
                GameManager.Instance.moveBlockages();
                AddReward(basketReward);
                basketReward = 0;
            }
        }
    }

    // Alternative for Switch (bc wall moves up too fast)
    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.CompareTag("switch"))
    //    {
    //        episodeDuration += 30;

    //        // reset rewards per room
    //        AddReward(switchReward);
    //        buttonReward = 4;
    //        platformReward = 0.2f;
    //        toolReward = 0.5f;
    //        basketReward = 0.5f;

    //        // Change state
    //        if (GameManager.Instance.State == GameState.Stage1)
    //            GameManager.Instance.UpdateGameState(GameState.Stage2);
    //        else if (GameManager.Instance.State == GameState.Stage2)
    //            GameManager.Instance.UpdateGameState(GameState.Stage3);
    //        else if (GameManager.Instance.State == GameState.Stage3)
    //            GameManager.Instance.UpdateGameState(GameState.Stage4);
    //        else if (GameManager.Instance.State == GameState.Stage4)
    //            GameManager.Instance.UpdateGameState(GameState.Stage5);
    //        else if (GameManager.Instance.State == GameState.Stage5)
    //            GameManager.Instance.UpdateGameState(GameState.Stage6);
    //    }
    //}

    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("goal"))
        {
            // update score
            scoreManager.score++;
            scoreManager.UpdateScoreText();

            // update rewards
            SetReward(goalReward);
            goalReward = 0;

            //EndEpisode();
        }
        else if (other.gameObject.CompareTag("button"))
        {
            AddReward(buttonReward);
            GameManager.Instance.moveBlockages();
            buttonReward = 0;
            goalReward = 7;
        }
        else if (other.gameObject.CompareTag("switch"))
        {
            episodeDuration += 30;

            // reset rewards per room
            AddReward(switchReward);
            buttonReward = 4;
            platformReward = 0.2f;
            toolReward = 0.5f;
            basketReward = 0.5f;
            
            // Change state
            if (GameManager.Instance.State == GameState.Stage1)
                GameManager.Instance.UpdateGameState(GameState.Stage2);
            else if (GameManager.Instance.State == GameState.Stage2)
                GameManager.Instance.UpdateGameState(GameState.Stage3);
            else if (GameManager.Instance.State == GameState.Stage3)
                GameManager.Instance.UpdateGameState(GameState.Stage4);
            else if (GameManager.Instance.State == GameState.Stage4)
                GameManager.Instance.UpdateGameState(GameState.Stage5);
            else if (GameManager.Instance.State == GameState.Stage5)
                GameManager.Instance.UpdateGameState(GameState.Stage6);
            else if (GameManager.Instance.State == GameState.Stage6)
                GameManager.Instance.UpdateGameState(GameState.Stage7);
        }
        else if (other.gameObject.CompareTag("finish"))
        {
            // update score
            scoreManager.score++;
            scoreManager.UpdateScoreText();

            SetReward(15f);
            GameManager.Instance.UpdateGameState(GameState.Stage1);
            agentSpawnPosition = GameManager.Instance.agentSpawnPoint;
            GameManager.Instance.Reset();
            EndEpisode();
            //GameManager.Instance.UpdateGameState(GameState.Victory);
        }
    }

    // Apply movement based on horizontal and vertical input
    private void ApplyMovement(float horizontalMovement, float verticalMovement)
    {
        // Move the agent based on input signals
        float movementSpeed = 3f; // Adjust the movement speed as needed
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
        if (isJumping == false)
        {
            // Apply jump force
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            isJumping = true;
        }
    }

    // manual movement
    public override void Heuristic(in ActionBuffers actionsOut)
    {
        // Get the continuous actions from input
        var continuousActionsOut = actionsOut.ContinuousActions;
        float horizontalInput = Input.GetAxis("Horizontal");
        float verticalInput = Input.GetAxis("Vertical");

        // Rotate the input based on the character's current rotation
        Vector3 rotatedInput = transform.rotation * new Vector3(horizontalInput, 0f, verticalInput);

        continuousActionsOut[0] = rotatedInput.x;
        continuousActionsOut[1] = rotatedInput.z;
        continuousActionsOut[2] = Input.GetAxis("Rotation");

        // Get the discrete action from input for jumping
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
        if (discreteActionsOut[0] == 1)
        {
            Jump();
        }
    }

    // checks if the agent has a tool as a child
    bool HasTool()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (!child.CompareTag("MainCamera"))
            {
                return true;
            }
        }
        return false;
    }

    // retrieves the tool object if the agent has one
    Transform GetTool()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            if (!child.CompareTag("MainCamera"))
            {
                return child;
            }
        }
        return null;
    }

    private void Update()
    {
        agentSpawnPosition = GameManager.Instance.agentSpawnPoint;
    }
}
