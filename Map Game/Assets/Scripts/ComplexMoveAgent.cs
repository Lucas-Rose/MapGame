using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class ComplexMoveAgent : Agent
{
    [SerializeField] private List<Transform> targetLocations;
    [SerializeField] private Transform activeTargetLocation;
    private List<int> targetStack;
    [SerializeField] private float moveSpeed;
    [SerializeField] private Vector3 spawnPoint;
    private float prevDist;

    private void Start()
    {
        spawnPoint = transform.localPosition;
        targetStack = new List<int>();
        GenerateNewStack();
        ShuffleStack();
        Debug.Log(targetLocations[targetStack[0]].name);
        activeTargetLocation = targetLocations[targetStack[0]];
        targetStack.RemoveAt(0);
        prevDist = Vector3.Distance(transform.localPosition, activeTargetLocation.localPosition);
    }
    private void Update()
    {
        float dist = Vector3.Distance(transform.localPosition, activeTargetLocation.localPosition);
        if(dist < prevDist)
        {
            SetReward(+0.01f * (80/dist));
        }
        prevDist = dist;
    }
    public override void OnEpisodeBegin()
    {
        transform.localPosition = spawnPoint;
        prevDist = Vector3.Distance(transform.localPosition, activeTargetLocation.localPosition);
    }
    public override void CollectObservations(VectorSensor sensor)
    {
        sensor.AddObservation(transform.localPosition);
        sensor.AddObservation(activeTargetLocation.localPosition);
        sensor.AddObservation(targetStack[0]);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        float moveX = actions.ContinuousActions[0];
        float moveZ = actions.ContinuousActions[1];

        transform.localPosition += new Vector3(moveX, 0, moveZ) * Time.deltaTime * moveSpeed;
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        continuousActions[0] = Input.GetAxisRaw("Horizontal");
        continuousActions[1] = Input.GetAxisRaw("Vertical");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Goal") && other.transform.Equals(activeTargetLocation))
        {
            Debug.Log("Reached Goal!");
            spawnPoint = transform.localPosition;
            activeTargetLocation = targetLocations[targetStack[0]];
            targetStack.RemoveAt(0);
            if(targetStack.Count == 0)
            {
                GenerateNewStack();
                ShuffleStack();
            }
            SetReward(+100f);
            EndEpisode();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Obstacle"))
        {
            SetReward(-1f);
            EndEpisode();
        }
    }

    public void GenerateNewStack()
    {
        for (int i = 0; i < targetLocations.Count; i++)
        {
            targetStack.Add(i);
        }
    }

    public void ShuffleStack()
    {
        for(int i = 0; i < targetStack.Count; i++)
        {
            int randomIdx = Random.Range(0, targetStack.Count);
            int copy = targetStack[i];
            targetStack[i] = targetStack[randomIdx];
            targetStack[randomIdx] = copy;
        }
    }
}
