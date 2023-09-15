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
    private List<GameObject> goalIndicators;
    private NavMeshAgent agent;

    private void Start()
    {
        locationGoals = new List<Vector3>();
        waypoints = new List<Vector3>();
        goalIndicators = new List<GameObject>();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
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
        }
        if (waypoints.Count > 0)
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
            }
        }
    }
    private void AddLocationGoal(Vector3 goalLocation)
    {
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
}
