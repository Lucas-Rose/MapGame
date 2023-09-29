using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HTNPManager : MonoBehaviour
{
    private List<HTNPBrain> npcs;
    // Start is called before the first frame update
    void Start()
    {
        npcs = new List<HTNPBrain>();
    }

    public void AddNPC(HTNPBrain newBrain)
    {
        npcs.Add(newBrain);
    }
}
