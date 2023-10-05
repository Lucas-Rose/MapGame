using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BehaviourDispensor : MonoBehaviour
{
    [SerializeField] private GameObject HTNPBoard;
    private GameObject[] npcs;
    private enum AIMode { 
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
                }
                break;
            case (AIMode.BTree):
                foreach (GameObject i in npcs)
                {
                    i.AddComponent<BTreeBrain>();
                }
                break;
            case (AIMode.GOAP):
                foreach (GameObject i in npcs)
                {
                    i.AddComponent<GOAPBrain>();
                }
                break;
            case (AIMode.HTNP):
                HTNPManager htnpManager = Instantiate(HTNPBoard).GetComponent<HTNPManager>();
                foreach (GameObject i in npcs)
                {
                    i.AddComponent<HTNPBrain>();
                    i.GetComponent<HTNPBrain>().Link(htnpManager);
                }
                break;
        }

    }
}
