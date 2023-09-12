using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.AI;

public class NMAgent : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private GameObject pointPrefab;

    private void Start()
    {
        agent.updateRotation = false;
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(SetLocationTarget());
        }
    }

    private IEnumerator SetLocationTarget()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            //Debug.Log(hit.point);
            agent.SetDestination(hit.point);
            yield return new WaitUntil(PathFound);
            foreach(Vector3 i in agent.path.corners){
                Instantiate(pointPrefab, i, Quaternion.identity);
            }
        }
    }

    private bool PathFound()
    {
        return agent.hasPath;
    }
}
