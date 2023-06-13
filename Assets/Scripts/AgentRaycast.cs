using Unity.Barracuda;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AgentRaycast : Agent
{
    public Score scoreManager;
    //public GameManager gameManager;
    private MainManager manager;
    private BehaviorParameters behaviorParameters;
    private NNModel brain;

    // rewards
    public float buttonReward = 4f;
    public float badWallReward = -1.0f;
    public float goalReward = 0f;
    public float fallOffReward = -3f;
    public float platformReward = 0.2f;
    public float toolReward = 1.5f;
    public float basketReward = 3f;
    public float switchReward = 1.0f;
    public float boxReward = 0.5f;

    // speed & rotation
    public float speedMultiplier = 0.3f;
    public float rotationSpeed = 10f;

    // variable for etc
    private float episodeDuration = 90f; // Duration of the episode in seconds
    private float elapsedTime = 0f; // Elapsed time since the episode started
    private Vector3 agentSpawnPosition;
    private bool droppedOff = false;

    //private bool touchedButton = false;
    //private bool allowMovement = true;

    // Jump related variables
    private bool isJumping = false;
    private float jumpForce = 7.5f;

    private Rigidbody rb;

    public override void Initialize()
    {
        manager = FindObjectOfType<MainManager>();
        scoreManager = FindObjectOfType<Score>();
        rb = this.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        //gameManager.Start();
        agentSpawnPosition = manager.agentSpawn;
        behaviorParameters = GetComponent<BehaviorParameters>();
        brain = manager.currentBrain;
        behaviorParameters.Model = brain;

        //if (gameManager.State == GameState.Stage3)
        //    gameManager.randomButtonPos();
    }
    public override void OnEpisodeBegin()
    {
        //gameManager.Reset();
        this.transform.localPosition = agentSpawnPosition;
        this.transform.localRotation = Quaternion.identity;


        // Reset the environment and agent state
        elapsedTime = 0f;
        droppedOff = false;
        //allowMovement = true;
        //touchedButton = false;

        // Reset rewards
        buttonReward = 4f;
        badWallReward = -1.0f;
        switchReward = 1.0f;
        goalReward = 0f;
        fallOffReward = -3f;
        platformReward = 0.2f;
        toolReward = 1.5f;
        boxReward = 0.5f;
        basketReward = 3.0f;


        if (manager.AIstate == AIState.Room4)
            manager.ResetTool();

        // Reset any jump-related variables
        isJumping = false;
        //gameManager.UpdateGameState(gameManager.State);

        //if (gameManager.State == GameState.Stage3)
        //    gameManager.randomButtonPos();
        //else if (gameManager.State == GameState.Stage1)
        //    gameManager.randomButtonPosS1();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(this.transform.localPosition);
        sensor.AddObservation(rb.velocity);
        sensor.AddObservation(manager.GateOpen);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Get the horizontal movement, forward/backward movement, and rotation signals
        float horizontalMovement = actionBuffers.ContinuousActions[0];
        float verticalMovement = actionBuffers.ContinuousActions[1];
        float rotationSignal = actionBuffers.ContinuousActions[2];
        float jumpMovement = actionBuffers.ContinuousActions[3];

        
        if (manager.allowMovement)
        {
            // Apply movement and rotation
            ApplyMovement(horizontalMovement, verticalMovement);

            // Handle jumping
            if (jumpMovement > 0.6)
            {
                Jump();
                AddReward(-0.02f);
            }

            ApplyRotation(rotationSignal);
        }

        // Increase the elapsed time
        elapsedTime += Time.deltaTime;

        // Check if the desired duration has been reached
        if (elapsedTime >= episodeDuration)
        {
            //SetReward(-2f);
            //EndEpisode();
        }

        if (this.transform.localPosition.y < 0)
        {
            manager.Reset();
            manager.GateOpen = false;
            // if tool fell, reset position
            if (manager.AIstate == AIState.Room4)
                manager.ResetTool();
            else if (manager.AIstate == AIState.Room3)
                manager.ResetBox();

            // punish for falling
            SetReward(fallOffReward);
            EndEpisode();
        }

        //if (gameManager.box.transform.localPosition.y < 0)
        //    gameManager.ResetBox();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("blockage"))
        {
            SetReward(badWallReward);
            manager.GateOpen = false;
            if (manager.AIstate == AIState.Room4)
                manager.ResetTool();
            EndEpisode();
        }
        else if (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("box") || collision.gameObject.CompareTag("platform") )
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
                toolReward = 0.5f;
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

                //if (!touchedButton)
                //{
                //    allowMovement = false;
                //    touchedButton = true;
                //}
                manager.moveBlockages();

                AddReward(basketReward);
                basketReward = 0;
                manager.GateOpen = true;
                goalReward = 15;
            }
        }
        else if (collision.gameObject.CompareTag("borderWall"))
        {
            SetReward(-1.5f);
            EndEpisode();
        }
        else if (collision.gameObject.CompareTag("box"))
        {
            SetReward(boxReward);
            boxReward = 0;
        }
        else if (collision.gameObject.CompareTag("Untagged"))
        {
            AddReward(-0.2f);
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

            manager.moveBlockages();

            manager.GateOpen = true; // remove ? its VR door that opens..
            manager.OpenVRDoor(); // opens the door of the vr player
            buttonReward = 0;
            goalReward = 15;
        }
        else if (other.gameObject.CompareTag("switch"))
        {
            manager.GateOpen = false;
            episodeDuration += 30;
            //touchedButton = false;

            // reset rewards per room
            AddReward(switchReward);
            buttonReward = 4;
            platformReward = 0.4f;
            toolReward = 0.5f;
            basketReward = 0.5f;

            // Change state
            manager.UpdateAI();
            //if (manager.AIstate == AIState.Room1)
            //    manager.UpdateAI(manager.nextSate);
            //else if (manager.AIstate == AIState.Room2)
            //    manager.UpdateAI(manager.nextSate);
            //else if (manager.AIstate == AIState.Room3)
            //    manager.UpdateAI(manager.nextSate);
            //else if (manager.AIstate == AIState.Room4)
            //    manager.UpdateAI(manager.nextSate);
        }
        else if (other.gameObject.CompareTag("finish"))
        {
            manager.GateOpen = false;

            // update score
            scoreManager.score++;
            scoreManager.UpdateScoreText();

            SetReward(15f);
            // open VR door --> method
            manager.OpenVRDoor();

            //gameManager.UpdateGameState(GameState.Stage1);
            //agentSpawnPosition = gameManager.agentSpawnPoint;
            //gameManager.Reset();
            //EndEpisode();
            //GameManager.Instance.UpdateGameState(GameState.Victory);
        }
        else if (other.gameObject.CompareTag("jumpPlate"))
        {
            //AddReward(0.2f);
            Jump();
        }
    }

    // Apply movement based on horizontal and vertical input
    private void ApplyMovement(float horizontalMovement, float verticalMovement)
    {
        // Move the agent based on input signals
        float movementSpeed = 4f; // Adjust the movement speed as needed
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
        float jumpInput = Input.GetKey(KeyCode.Space) ? 1f : 0f;
        continuousActionsOut[3] = jumpInput;

        // If jump input is above 0.6, trigger jump
        if (jumpInput > 0.6f)
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
        agentSpawnPosition = manager.agentSpawn;
    }

    public void GiveModel(string behaviorName, NNModel model)
    {
        behaviorParameters.Model = model;
        behaviorParameters.BehaviorName = behaviorName;
    }
}
