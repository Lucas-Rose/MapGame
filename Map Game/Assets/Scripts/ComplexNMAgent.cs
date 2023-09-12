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

    [Header("Settings")]
    [SerializeField] private float waypointSensitivity;
    [SerializeField] private Vector2 xOffset;
    [SerializeField] private Vector2 zOffset;

    private List<Vector3> locationGoals;
    private List<Vector3> waypoints;
    private NavMeshAgent agent;

    private void Start()
    {
        locationGoals = new List<Vector3>();
        waypoints = new List<Vector3>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.isStopped = true;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit))
            {
                AddLocationGoal(hit.point);
            }
        }
        if(waypoints.Count > 0)
        {
            if (!agent.hasPath && !agent.pathPending)
            {
                agent.SetDestination(waypoints[0]);
            }
            
            if (Vector3.Distance(transform.position, waypoints[0]) <= waypointSensitivity)
            {
                waypoints.RemoveAt(0);
            }
        }

    }
    private void AddLocationGoal(Vector3 goalLocation)
    {
        locationGoals.Add(goalLocation);
        AddToWaypoints(goalLocation);
        OffSetWayPoints();
        Instantiate(goalIndicator, goalLocation, Quaternion.identity, indicatorContainer);
    }
    private void GenerateWaypoints()
    {
        waypoints = new List<Vector3>();
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            NavMeshPath newPath = new NavMeshPath();
            agent.CalculatePath(hit.point, newPath);
            foreach (Vector3 i in newPath.corners)
            {
                waypoints.Add(i);
            }
            agent.isStopped = false;
        }
    }
    public void AddToWaypoints(Vector3 target)
    {
        NavMeshPath newPath = new NavMeshPath();
        agent.CalculatePath(target, newPath);
        foreach (Vector3 i in newPath.corners)
        {
            waypoints.Add(i);
        }
        agent.isStopped = false;
    }
    private void DrawWaypoints(bool offset)
    {        
        foreach(Vector3 i in waypoints)
        {
            if (offset)
            {
                Instantiate(offsetIndicator, i, Quaternion.identity, indicatorContainer);
            }
            else
            {
                Instantiate(indicator, i, Quaternion.identity, indicatorContainer);
            }
            
        }
    }
    private void OffSetWayPoints()
    {
        for(int i = 1; i < waypoints.Count; i++)
        {
            float xPush = Random.Range(xOffset.x, xOffset.y);
            float zPush = Random.Range(zOffset.x, zOffset.y);
            waypoints[i] = new Vector3(waypoints[i].x + xPush, waypoints[i].y, waypoints[i].z + zPush);
        }
        for(int i = 0; i < waypoints.Count -1; i++)
        {
            Debug.DrawLine(waypoints[i], waypoints[i + 1]);
        }
    }
}
