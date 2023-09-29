using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HTNPBrain : MonoBehaviour
{
    // Start is called before the first frame update
    private HTNPManager manager;
    public void Link(HTNPManager newManager)
    {
        manager = newManager;
        newManager.AddNPC(this);
    }
}
