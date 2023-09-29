using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FSMBrain : MonoBehaviour
{
    private LookBrain lookBrain;
    private ComplexNMAgent moveBrain;
    private bool isActive;
    public enum State
    {
        Patrolling,
        Waiting,
        Shooting, 
        Dead
    }
    private State aiState;
    private bool canMove;
    private GameObject[] patrolPoints;

    private void Start()
    {
        lookBrain = GetComponent<LookBrain>();
        moveBrain = GetComponent<ComplexNMAgent>();
        isActive = false;
        patrolPoints = GameObject.FindGameObjectsWithTag("FSMPoint");
    }
    private void Update()
    {
        if (isActive)
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
                    break;
                case (State.Shooting):
                    break;
                case (State.Dead):
                    break;
            }
        }        
    }
    public void SetMode(State state)
    {
        aiState = state;
    }
    public void SelectPatrolPoint()
    {
        Transform targetPoint = patrolPoints[Random.Range(0, patrolPoints.Length)].transform;
        moveBrain.AddLocationGoal(targetPoint.position);
    }
    public void Activate()
    {
        isActive = true;
        aiState = State.Patrolling;
        SelectPatrolPoint();
    }
    public void Deactivate()
    {
        isActive = false;
    }
}
