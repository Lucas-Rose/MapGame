using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourDispensor : MonoBehaviour
{
    private HTNPManager htnpManager;
    [SerializeField] private GameObject HTNPBoard;
    [SerializeField] private GameObject agentPrefab;
    [SerializeField] private List<Transform> spawnPoints;
    [SerializeField] private RuntimeAnimatorController BTree;
    private GameObject[] npcs;
    
    public enum AIMode { 
        FSM,
        BTree,
        GOAP,
        HTNP
    }

    [SerializeField] private AIMode mode;
    private void Start()
    {
        npcs = GameObject.FindGameObjectsWithTag("AI");
        DisperseBehaviours();
    }

    private void Update()
    {
        if(npcs.Length == 0)
        {
            GameManager.FinishGame(true);
        }
    }
    public void DisperseBehaviours()
    {
        switch (mode) {
            case (AIMode.FSM):
                foreach(GameObject i in npcs)
                {
                    i.AddComponent<FSMBrain>();
                    i.GetComponent<LookBrain>().AddBehaviourDispensor(this);
                    i.GetComponent<LookBrain>().AddFSMBrain(i.GetComponent<FSMBrain>());
                }
                break;
            case (AIMode.BTree):
                foreach (GameObject i in npcs)
                {
                    i.AddComponent<BTreeBrain>();
                    i.GetComponent<LookBrain>().AddBehaviourDispensor(this);
                    i.GetComponent<LookBrain>().AddBTreeBrain(i.GetComponent<BTreeBrain>());
                }
                break;
            case (AIMode.GOAP):
                foreach (GameObject i in npcs)
                {
                    i.AddComponent<GOAPBrain>();
                    i.GetComponent<LookBrain>().AddBehaviourDispensor(this);
                }
                break;
            case (AIMode.HTNP):
                htnpManager = Instantiate(HTNPBoard).GetComponent<HTNPManager>();
                foreach (GameObject i in npcs)
                {
                    i.AddComponent<HTNPBrain>();
                    i.GetComponent<HTNPBrain>().Link(htnpManager);
                    i.GetComponent<LookBrain>().AddBehaviourDispensor(this);
                }
                break;
        }
    }
    public AIMode getMode()
    {
        return mode;
    }
    public RuntimeAnimatorController GetBehaviourTree()
    {
        return BTree;
    }
    public void RespawnAgent()
    {
        GameObject newAgent = Instantiate(agentPrefab, spawnPoints[Random.Range(0, spawnPoints.Count)].position, Quaternion.identity);
        switch (mode)
        {
            case (AIMode.FSM):
                newAgent.AddComponent<FSMBrain>();
                newAgent.GetComponent<LookBrain>().AddBehaviourDispensor(this);
                newAgent.GetComponent<LookBrain>().AddFSMBrain(newAgent.GetComponent<FSMBrain>());
                break;
            case (AIMode.GOAP):
                newAgent.AddComponent<GOAPBrain>();
                newAgent.GetComponent<LookBrain>().AddBehaviourDispensor(this);
                break;
            case (AIMode.BTree):
                newAgent.AddComponent<BTreeBrain>();
                newAgent.GetComponent<LookBrain>().AddBehaviourDispensor(this);
                break;
            case (AIMode.HTNP):
                newAgent.AddComponent<HTNPBrain>();
                newAgent.GetComponent<HTNPBrain>().Link(htnpManager);
                newAgent.GetComponent<LookBrain>().AddBehaviourDispensor(this);
                break;
        }
    }

    public void ToggleMove(bool state)
    {
        foreach (GameObject i in npcs)
        {
            switch (mode)
            {
                case (AIMode.FSM):
                    i.GetComponent<FSMBrain>().SetCanMove(state);
                    break;
                case (AIMode.GOAP):
                    i.GetComponent<GOAPBrain>().SetCanMove(state);
                    break;
                case (AIMode.BTree):
                    i.GetComponent<BTreeBrain>().SetCanMove(state);
                    break;
                case (AIMode.HTNP):
                    i.GetComponent<HTNPBrain>().SetCanMove(state);
                    break;
            }
            i.GetComponent<LookBrain>().SetCanMove(state);
            i.GetComponent<ComplexNMAgent>().SetCanMove(state);
        }
    }
}
