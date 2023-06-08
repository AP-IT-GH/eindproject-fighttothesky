using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBox : MonoBehaviour
{
    public AgentRaycast agent;
    public GameManager gm;

    private float reward = 5.5f;

    private void Update()
    {
        if (this.transform.localPosition.y < 0)
        {
            gm.ResetBox();
            agent.SetReward(-1f);
            agent.EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("pedestal"))
        {
            //agent.AddReward(1.5f);
            gm.SetGateTrue();

            //tijdelijk blockage weg als hij block goed zet 
            //fase 1
            agent.SetReward(reward);
            //gm.moveBlockages();
            //reward = 0;
            //agent.goalReward = 15f;

            agent.scoreManager.score++;
            agent.scoreManager.UpdateScoreText();
            agent.EndEpisode();
        }
    }
}
