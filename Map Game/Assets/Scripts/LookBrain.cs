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
    public bool basicAiming;

    [Header("COMPONENTS")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private GameObject missIndicator;

    [Header("PARAMETERS")]
    [SerializeField] private float reactionTime; //from idle to rotating towards target
    [SerializeField] private float aimingGradient;
    [SerializeField] private float missGradient;
    [SerializeField] private float xAccuracy;
    [SerializeField] private float yAccuracy;
    [SerializeField] private bool drawingIndicators;

    [Header("SCAN SETTINGS")]
    [SerializeField] private float scanFrequency;
    [SerializeField] private float scanRange; //Degrees
    [SerializeField] private float scanRadius;
    [SerializeField] private bool drawViewRays;
    private List<Vector3> viewRays;

    private BehaviourDispensor bd;
    private FSMBrain fsm;


    // Start is called before the first frame update
    void Start()
    {
        shotPoints = new List<Vector3>();
        canMove = true;
        reacted = false;
    }

    private void Update()
    {
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
                    if(fsm.GetState() == FSMBrain.State.Shooting)
                    {
                        Shoot(shotPoints[0]);
                    }
                    shotPoints.RemoveAt(0);
                    movingTime = 0;
                }
                if(shotPoints.Count == 0)
                {
                    reacted = false;
                }
            }   
        }
        GenerateViewRays();
        if(bd.getMode() == BehaviourDispensor.AIMode.FSM)
        {
            if(fsm.GetState() != FSMBrain.State.Shooting)
            {
                TestViewRays();
            }
        }
        
        if (drawViewRays)
        {
            DrawViewRays();
        }
        
    }
    public void Shoot(Vector3 point)
    {
        Debug.Log("I'm shooting this now");
        RaycastHit hit;
        if (Physics.Raycast(new Ray(transform.position, point - transform.position), out hit, scanRadius + 20, Physics.AllLayers))
        {
            if (hit.transform.gameObject.CompareTag("Enemy"))
            {
                Debug.Log("I'm destroying this now");
                Destroy(hit.transform.gameObject);
                fsm.SetMode(FSMBrain.State.Patrolling);
            }
            if (hit.transform.gameObject.CompareTag("Player"))
            {
                Cursor.lockState = CursorLockMode.None;
                GameManager.FinishGame(false);
            }
        }
        Debug.DrawRay(transform.position, point, Color.red, 20);
    }

    public void GenerateViewRays()
    {
        viewRays = new List<Vector3>();
        float totalRange = scanRange - -scanRange;
        float segmentWidth = totalRange / scanFrequency;

        for(int i = 0; i < scanFrequency+1; i++)
        {
            Vector3 forward = transform.forward;
            forward = Quaternion.Euler(0, -scanRange + i * segmentWidth, 0) * forward * scanRadius;
            viewRays.Add(forward);
        }
    }
    public void DrawViewRays()
    {
        for (int i = 0; i < viewRays.Count; i++)
        {
            Debug.DrawRay(transform.position, viewRays[i], Color.green);
        }
    }
    public void TestViewRays()
    {
        for(int i = 0; i < viewRays.Count; i++)
        {
            RaycastHit hit;
            if(Physics.Raycast(new Ray(transform.position, viewRays[i]), out hit, scanRadius, Physics.AllLayers))
            {
                if (hit.transform.gameObject.CompareTag("Enemy") || hit.transform.gameObject.CompareTag("Player"))
                {
                    if(bd.getMode() == BehaviourDispensor.AIMode.FSM)
                    {
                        if(fsm.GetState() != FSMBrain.State.Shooting)
                        {
                            Debug.Log("Found enemy");
                            StartAiming(hit.point);
                            fsm.SetFocusEnemy(hit.transform.gameObject);
                            fsm.SetMode(FSMBrain.State.Shooting);
                        }
                    }
                    if (bd.getMode() == BehaviourDispensor.AIMode.BTree)
                    {

                    }
                    if (bd.getMode() == BehaviourDispensor.AIMode.GOAP)
                    {

                    }
                    if (bd.getMode() == BehaviourDispensor.AIMode.HTNP)
                    {

                    }
                    fsm.SetMode(FSMBrain.State.Shooting);
                    StartAiming(hit.point);
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
            if (drawingIndicators)
            {
                Instantiate(missIndicator, new Vector3(shotPoints[shotPoints.Count - 1].x, shotPoints[shotPoints.Count - 1].y, target.z), Quaternion.identity);
            }
            
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
    public void AddFSMBrain(FSMBrain newBrain)
    {
        fsm = newBrain;
    }
    public void AddBehaviourDispensor(BehaviourDispensor dispensor)
    {
        bd = dispensor;
    }
    public int GetShotPointsCount()
    {
        return shotPoints.Count;
    }
}
