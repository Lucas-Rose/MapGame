using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookBrain : MonoBehaviour
{
    private List<Vector3> shotPoints;
    private bool canMove;
    private bool reacted;
    private float currTime;
    private float movingTime;
    private int shotTotal;

    [Header("Components")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GameObject missIndicator;
    private Camera cam;

    [Header("Parameters")]
    [SerializeField] private float reactionTime; //from idle to rotating towards target
    [SerializeField] private float aimingGradient;
    [SerializeField] private float missGradient;
    [SerializeField] private float xAccuracy;
    [SerializeField] private float yAccuracy;


    // Start is called before the first frame update
    void Start()
    {
        cam = cameraTransform.GetComponent<Camera>();
        shotPoints = new List<Vector3>();
        canMove = true;
        reacted = false;
    }

    private void Update()
    {

        if (Input.GetMouseButtonDown(1))
        {
            shotPoints = new List<Vector3>();
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit)){
                StartAiming(hit.point);
            }
        }
        if (canMove)
        {
            currTime += Time.deltaTime;
            if (currTime > reactionTime && shotPoints.Count != 0)
            {
                reacted = true;
            }
            else
            {
                reacted = false;
            }
            if (reacted)
            {
                movingTime += Time.deltaTime;
                var bodyLookPos = shotPoints[0] - transform.position;
                Quaternion bodyRot = Quaternion.LookRotation(bodyLookPos);
                bodyRot.eulerAngles = new Vector3(transform.rotation.eulerAngles.x, bodyRot.eulerAngles.y, transform.rotation.eulerAngles.z);

                var camLookPos = shotPoints[0] - cameraTransform.position;
                Quaternion camRot = Quaternion.LookRotation(camLookPos);
                camRot.eulerAngles = new Vector3(camRot.eulerAngles.x, cameraTransform.rotation.eulerAngles.y, cameraTransform.rotation.eulerAngles.z);

                transform.rotation = Quaternion.Slerp(transform.rotation, bodyRot, Time.deltaTime * 15 * Vector3.Distance(transform.position, shotPoints[0]) / shotTotal);
                cameraTransform.rotation = Quaternion.Slerp(cameraTransform.rotation, camRot, Time.deltaTime * 15 * Vector3.Distance(transform.position, shotPoints[0]) / shotTotal);
                cameraTransform.rotation = Quaternion.Euler(cameraTransform.localEulerAngles.x, transform.localEulerAngles.y, 0);

                if (movingTime > aimingGradient * Vector3.Distance(transform.position, shotPoints[0]) / shotTotal)
                {
                    shotPoints.RemoveAt(0);
                    movingTime = 0;
                }
                if(shotPoints.Count == 0)
                {
                    reacted = false;
                }
            }   
        }
    }

    public void StartAiming(Vector3 targetPos)
    {
        float accuracyMeasure = Vector3.Distance(transform.position, targetPos) * missGradient;

        float numberOfMiss = 0;
        numberOfMiss += (accuracyMeasure >= 1) ? Mathf.Floor(accuracyMeasure) : 0;

        float remainderChance = Mathf.Floor(accuracyMeasure) >= 1 ? accuracyMeasure / 10f : accuracyMeasure;

        numberOfMiss += (Random.value > remainderChance) ? 1 : 0;

        GenerateShotVectors(targetPos, (int)numberOfMiss);
        currTime = 0;
    }

    public void GenerateShotVectors(Vector3 target, int n)
    {
        shotPoints = new List<Vector3>();
        float offSetX;
        float offSetY;
        float xLoc;
        float yLoc;

        for (int i = 0; i < n; i++)
        {
            offSetX = Random.value > .5f ? Random.Range(-xAccuracy * 3, 0) : Random.Range(0, xAccuracy * 3);
            offSetY = Random.value > .5f ? Random.Range(-yAccuracy * 3, 0) : Random.Range(0, yAccuracy * 3);
            xLoc = target.x + offSetX;
            yLoc = target.y + offSetY;
            shotPoints.Add(new Vector3(xLoc, yLoc, target.z));
            Instantiate(missIndicator, new Vector3(shotPoints[shotPoints.Count - 1].x, shotPoints[shotPoints.Count - 1].y, target.z), Quaternion.identity);
        }
        offSetX = Random.value > .5f ? Random.Range(-xAccuracy, 0) : Random.Range(0, xAccuracy);
        offSetY = Random.value > .5f ? Random.Range(-yAccuracy, 0) : Random.Range(0, yAccuracy);
        xLoc = target.x + offSetX;
        yLoc = target.y + offSetY;
        shotPoints.Add(new Vector3(xLoc, yLoc, target.z));
        shotTotal = shotPoints.Count;
    }

    public void SetCanMove(bool state)
    {
        canMove = state;
    }

    public void ResetRotation()
    {
        transform.localRotation = Quaternion.Euler(0, 0, 0);
        cameraTransform.localRotation = Quaternion.Euler(0, 0, 0);
    }
}
