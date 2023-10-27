using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class ComplexNMAgent : MonoBehaviour
{
    [SerializeField] private Camera cam;

    [Header("Indicators")]
    [SerializeField] private Transform indicatorContainer;
    [SerializeField] private GameObject indicator;
    [SerializeField] private GameObject offsetIndicator;
    [SerializeField] private GameObject goalIndicator;
    [SerializeField] private GameObject obstacleIndicator;

    [Header("Settings")]
    [SerializeField] private float waypointSensitivity;
    [SerializeField] private Vector2 xOffset;
    [SerializeField] private Vector2 zOffset;
    [SerializeField] private bool drawingIndicators;

    private List<Vector3> locationGoals;
    private List<Vector3> waypoints;
    private GameObject[] patrolPoints;
    [SerializeField] private float minWaitTime;
    private bool canMove;
    private float stuckTime;
    private List<GameObject> goalIndicators;
    private NavMeshAgent agent;
    private FSMBrain fsm;
    private BehaviourDispensor bd;
    private BTreeBrain BTree;

    private void Start()
    {
        locationGoals = new List<Vector3>();
        waypoints = new List<Vector3>();
        goalIndicators = new List<GameObject>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        patrolPoints = GameObject.FindGameObjectsWithTag("FSMPoint");
        bd = FindObjectOfType<BehaviourDispensor>();
    }
    void Update()
    {
        /*if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                AddLocationGoal(hit.point);
            }
        }
        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //Instantiate(obstacleIndicator, hit.point, Quaternion.identity);
            }
        }*/
        if (waypoints.Count > 0)
        {
            if (!agent.hasPath && !agent.pathPending)
            {
                if (canMove)
                {
                    agent.SetDestination(waypoints[0]);
                }
            }

            if (Vector3.Distance(transform.position, waypoints[0]) <= waypointSensitivity)
            {
                waypoints.RemoveAt(0);
                switch (bd.getMode())
                {
                    case BehaviourDispensor.AIMode.FSM:
                        fsm.getLookBrain().StartAiming(getFirstWaypoint() + Vector3.up);
                        break;
                    case BehaviourDispensor.AIMode.BTree:
                        break;
                    case BehaviourDispensor.AIMode.GOAP:
                        break;
                    case BehaviourDispensor.AIMode.HTNP:
                        break;
                }
                
            }
        }
        if (locationGoals.Count > 0)
        {
            if (Vector3.Distance(transform.position, locationGoals[0]) <= waypointSensitivity)
            {
                if (drawingIndicators)
                {
                    Destroy(goalIndicators[0]);
                    goalIndicators.RemoveAt(0);
                }
                locationGoals.RemoveAt(0);
                if(bd.getMode() == BehaviourDispensor.AIMode.FSM)
                {
                    fsm.SetMode(FSMBrain.State.Waiting);
                }
            }
            if (bd.getMode() == BehaviourDispensor.AIMode.FSM)
            {
                if (agent.velocity.magnitude < 0.5 && fsm.GetState() != FSMBrain.State.Patrolling)
                {
                    stuckTime += Time.deltaTime;
                    if (stuckTime > minWaitTime)
                    {
                        ResetWaypoints();
                        fsm.SelectPatrolPoint();
                        stuckTime = 0;
                    }
                }
            }
        }
    }
    public void AddLocationGoal(Vector3 goalLocation)
    {
        Debug.Log("Goal Added");
        agent.isStopped = false;
        locationGoals.Add(goalLocation);
        List<Vector3> newPoints = GenerateWaypoints(goalLocation);
        newPoints = OffSetWaypoints(newPoints);
        AddToWaypoints(newPoints);

        if (drawingIndicators)
        {
            DrawWaypoints(waypoints);
            goalIndicators.Add(Instantiate(goalIndicator, goalLocation, Quaternion.identity, indicatorContainer));
        }
    }
    public void AddLocation()
    {
        Debug.Log("Goal Added");
        agent.isStopped = false;
        locationGoals.Add(patrolPoints[Random.Range(0, patrolPoints.Length)].transform.position);
        List<Vector3> newPoints = GenerateWaypoints(locationGoals[0]);
        newPoints = OffSetWaypoints(newPoints);
        AddToWaypoints(newPoints);

        if (drawingIndicators)
        {
            DrawWaypoints(waypoints);
            goalIndicators.Add(Instantiate(goalIndicator, locationGoals[0], Quaternion.identity, indicatorContainer));
        }
    }
    public void AddFSMBrain(FSMBrain newBrain)
    {
        fsm = newBrain;
    }
    public void AddBTreeBrain(BTreeBrain newBrain)
    {
        BTree = newBrain;
    }
    private List<Vector3> GenerateWaypoints(Vector3 point)
    {
        List<Vector3> newList = new List<Vector3>();
        NavMeshPath newPath = new NavMeshPath();
        if(waypoints.Count == 0)
        {
            //agent.CalculatePath(point, newPath);
            NavMesh.CalculatePath(transform.position, point, NavMesh.AllAreas, newPath);
        }
        else
        {
            NavMesh.CalculatePath(waypoints[waypoints.Count - 1], point, NavMesh.AllAreas, newPath);
        }
        foreach (Vector3 i in newPath.corners)
        {
            newList.Add(i);
        }
        return newList;
    }
    public void AddToWaypoints(List<Vector3> newList)
    {
        foreach (Vector3 i in newList)
        {
            waypoints.Add(i);
        }
    }
    private void DrawWaypoints(List<Vector3> points)
    {
        foreach (Vector3 i in points)
        {
            Instantiate(indicator, i, Quaternion.identity, indicatorContainer);
        }
    }
    private List<Vector3> OffSetWaypoints(List<Vector3> newList)
    {
        for (int i = 1; i < newList.Count; i++)
        {
            float xPush = Random.Range(xOffset.x, xOffset.y);
            float zPush = Random.Range(zOffset.x, zOffset.y);
            newList[i] = new Vector3(newList[i].x + xPush, newList[i].y, newList[i].z + zPush);
        }
        return newList;
    }
    public int RemainingWaypoints()
    {
        return waypoints.Count;
    }
    public Vector3 getFirstLocationGoal()
    {
        if(locationGoals.Count > 0)
        {
            return locationGoals[0];
        }
        return Vector3.zero;
    }
    public Vector3 getFirstWaypoint()
    {
        if (waypoints.Count > 0)
        {
            return waypoints[0];
        }
        return Vector3.zero;
    }
    public void ResetLocationGoals()
    {
        ResetWaypoints();
        agent.isStopped = true;
    }
    public void ResetWaypoints()
    {
        locationGoals = new List<Vector3>();
        waypoints = new List<Vector3>();
    }
    public void SetCanMove(bool state)
    {
        canMove = state;
    }
}
