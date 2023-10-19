using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class FSMBrain : MonoBehaviour
{
    //Objects
    private LookBrain lookBrain;
    private ComplexNMAgent moveBrain;
    private bool canMove;
    private GameObject focusEnemy;

    [Header("IDLE SETTINGS")]
    private Vector2 waitTime = new Vector2(2, 4);
    private float waitTimer = 0;

    //State
    public enum State
    {
        Patrolling,
        Waiting,
        Shooting, 
        Dead
    }
    private State aiState;

    //Points
    private GameObject[] patrolPoints;
    private Transform lastPointAssigned;

    private void Start()
    {
        lookBrain = GetComponent<LookBrain>();
        moveBrain = GetComponent<ComplexNMAgent>();
        moveBrain.AddFSMBrain(this);
        patrolPoints = GameObject.FindGameObjectsWithTag("FSMPoint");
        canMove = false;
    }
    private void Update()
    {
        if (canMove)
        {
            switch (aiState)
            {
                case (State.Patrolling):
                    if (moveBrain.RemainingWaypoints() == 0)
                    {
                        Debug.Log(gameObject.name + " selecting waypoint");
                        SelectPatrolPoint();
                    }
                    break;
                case (State.Waiting):
                    waitTimer -= Time.deltaTime;
                    if (waitTimer < 0)
                    {
                        SetMode(State.Patrolling);
                    }
                    break;
                case (State.Shooting):
                    if(focusEnemy == null)
                    {
                        aiState = State.Patrolling;
                    }
                    if(lookBrain.GetShotPointsCount() == 0)
                    {
                        lookBrain.StartAiming(focusEnemy.transform.position + new Vector3(0, 0.5f, 0));
                    }
                    break;
                case (State.Dead):
                    break;
            }
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
                moveBrain.ResetLocationGoals();
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
    public State GetState()
    {
        return aiState;
    }
    public void SetCanMove(bool state)
    {
        canMove = state;
    }
    public void SetFocusEnemy(GameObject newEnemy)
    {
        focusEnemy = newEnemy;
    }
}
