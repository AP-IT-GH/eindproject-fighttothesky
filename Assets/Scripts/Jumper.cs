using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using static UnityEngine.GraphicsBuffer;

public class JumperAgent : Agent
{
    public float force = 30f;
    public Transform reset = null;
    public Transform target;

    private Rigidbody rb = null;
    private float elapsedTime = 0f;
    private float lastDistanceToTarget;
    private float distanceToTarget;
    private float REWARD_HIT_ROAD_MULTIPLIER = 0.5f;
    private float REWARD_HIT_WALLTOP_MULTIPLIER = 0.75f;
    private float REWARD_HIT_OBSTACLE_MULTIPLIER = 1.5f;
    private const float REWARD_PER_SECOND = 0.1f;
    private const float REWARD_DISTANCE_MULTIPLIER = 0.1f;
    private LayerMask obstacleLayerMask;
    private LayerMask wallTopLayerMask;
    private LayerMask roadLayerMask;

    public override void Initialize()
    {
        rb = this.GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ;

        obstacleLayerMask = LayerMask.GetMask("obstacle");
        wallTopLayerMask = LayerMask.GetMask("wallTop");
        roadLayerMask = LayerMask.GetMask("road");

        ResetMyAgent();
    }

    private void ResetMyAgent()
    {
        this.transform.position = new Vector3(reset.position.x, reset.position.y, reset.position.z);
        distanceToTarget = Mathf.Infinity;
    }

    public override void OnEpisodeBegin()
    {
        ResetMyAgent();
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        if (Input.GetKey(KeyCode.UpArrow))
            continuousActionsOut[0] = 1.0f;
        else if (Input.GetKey(KeyCode.DownArrow))
            continuousActionsOut[0] = -1.0f;
        else
            continuousActionsOut[0] = 0.0f;
    }

    public override void OnActionReceived(ActionBuffers actions)
    {
        float upForce = actions.ContinuousActions[0];

        if (upForce > 0.5f)
        {
            UpForce();
        }
        else if (upForce < -0.5f)
        {
            DownForce();
        }

        // Calculate distance to target
        distanceToTarget = Vector3.Distance(transform.position, target.position);

        // Reward for increasing distance to target
        AddReward((lastDistanceToTarget - distanceToTarget) * REWARD_DISTANCE_MULTIPLIER);

        if (elapsedTime >= 1f)
        {
            AddReward(REWARD_PER_SECOND);
            elapsedTime -= 1f;
        }

        // negative reward if agent is too close to target 
        if (distanceToTarget < lastDistanceToTarget)
            AddReward(REWARD_DISTANCE_MULTIPLIER);
        else
            AddReward(-REWARD_DISTANCE_MULTIPLIER);

        lastDistanceToTarget = distanceToTarget;

        // Negative rewards for hitting obstacles, wallTop, and road
        float hitObstacle = Physics.Raycast(transform.position, Vector3.left, 3f, obstacleLayerMask)
            || Physics.Raycast(transform.position, Vector3.right, 3f, obstacleLayerMask)
            || Physics.Raycast(transform.position, Vector3.forward, 3f, obstacleLayerMask)
            || Physics.Raycast(transform.position, Vector3.back, 3f, obstacleLayerMask)
            || Physics.Raycast(transform.position, Vector3.up, 3f, obstacleLayerMask)
            || Physics.Raycast(transform.position, Vector3.down, 3f, obstacleLayerMask) ? 1.0f : 0.0f;

        float hitWallTop = Physics.Raycast(transform.position, Vector3.up, 3f, wallTopLayerMask)
            || Physics.Raycast(transform.position, Vector3.down, 3f, wallTopLayerMask) ? 1.0f : 0.0f;

        float hitRoad = Physics.Raycast(transform.position, Vector3.up, 3f, roadLayerMask)
            || Physics.Raycast(transform.position, Vector3.down, 3f, roadLayerMask) ? 1.0f : 0.0f;

        AddReward(-hitObstacle * REWARD_HIT_OBSTACLE_MULTIPLIER);
        AddReward(-hitWallTop * REWARD_HIT_WALLTOP_MULTIPLIER);
        AddReward(-hitRoad * REWARD_HIT_ROAD_MULTIPLIER);

        if (hitObstacle > 0)
        {
            EndEpisode();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Agent positie
        sensor.AddObservation(this.transform.localPosition);

        // Distance to target
        distanceToTarget = Vector3.Distance(this.transform.localPosition, target.localPosition);
        sensor.AddObservation(distanceToTarget);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("obstacle"))
        {
            AddReward(-1.5f);
            Destroy(collision.gameObject);
            EndEpisode();
        }
        if (collision.gameObject.CompareTag("wallTop"))
        {
            AddReward(-1.0f);
        }
        if (collision.gameObject.CompareTag("road"))
        {
            AddReward(-0.5f);
        }
    }

    private void Update()
    {
        elapsedTime += Time.deltaTime;
    }

    private void UpForce()
    {
        rb.AddForce(Vector3.up * force * 2, ForceMode.Acceleration);
    }

    private void DownForce()
    {
        rb.AddForce(Vector3.down * force, ForceMode.Acceleration);
    }
}
