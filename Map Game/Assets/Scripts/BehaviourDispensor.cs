using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourDispensor : MonoBehaviour
{
    [SerializeField] private GameObject HTNPBoard;
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
                HTNPManager htnpManager = Instantiate(HTNPBoard).GetComponent<HTNPManager>();
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
}
