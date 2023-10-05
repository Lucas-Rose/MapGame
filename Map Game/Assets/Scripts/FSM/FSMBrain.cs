using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FSMBrain : MonoBehaviour
{
    //Objects
    private LookBrain lookBrain;
    private ComplexNMAgent moveBrain;

    [Header("IDLE SETTINGS")]
    private Vector2 waitTime = new Vector2(2, 4);
    private float waitTimer = 0;

    public enum State
    {
        Patrolling,
        Waiting,
        Shooting, 
        Dead
    }
    private State aiState;

    private GameObject[] patrolPoints;
    private Transform lastPointAssigned;

    private void Start()
    {
        lookBrain = GetComponent<LookBrain>();
        moveBrain = GetComponent<ComplexNMAgent>();
        moveBrain.AddFSMBrain(this);
        patrolPoints = GameObject.FindGameObjectsWithTag("FSMPoint");
    }
    private void Update()
    {
        switch (aiState)
        {
            case (State.Patrolling):
                if (moveBrain.RemainingWaypoints() == 0)
                {
                    SelectPatrolPoint();
                }
                break;
            case (State.Waiting):
                waitTimer -= Time.deltaTime;
                if(waitTimer < 0)
                {
                    SetMode(State.Patrolling);
                }
                break;
            case (State.Shooting):
                break;
            case (State.Dead):
                break;
        }
    }
    public void SetMode(State state)
    {
        aiState = state;
        switch (aiState)
        {
            case (State.Patrolling):
                break;
            case (State.Waiting):
                waitTimer = Random.Range(waitTime.x, waitTime.y);
                break;
            case (State.Shooting):
                break;
            case (State.Dead):
                break;
        }
    }
    public void SelectPatrolPoint()
    {
        GameObject[] patrolCopy;
        Transform targetPoint;
        if (lastPointAssigned != null)
        {
            patrolCopy = new GameObject[patrolPoints.Length - 1];
            int idx = 0;
            for (int i = 0; i < patrolPoints.Length; i++)
            {
                if (patrolPoints[i].transform == lastPointAssigned)
                {
                    continue;
                }
                else
                {
                    patrolCopy[idx] = patrolPoints[i];
                    idx++;
                }
            }
            targetPoint = patrolCopy[Random.Range(0, patrolCopy.Length)].transform;
            moveBrain.AddLocationGoal(targetPoint.position);
            lastPointAssigned = targetPoint;
        }
        else
        {
            targetPoint = patrolPoints[Random.Range(0, patrolPoints.Length)].transform;
            moveBrain.AddLocationGoal(targetPoint.position);
            lastPointAssigned = targetPoint;
        }
    }

    public void Activate()
    {
        aiState = State.Patrolling;
        SelectPatrolPoint();
    }
    public LookBrain getLookBrain()
    {
        return lookBrain;
    }
}
