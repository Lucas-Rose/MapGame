using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class BTreeBrain : MonoBehaviour
{
    private bool canMove;
    private Animator anim;
    private BehaviourDispensor bd;
    // Start is called before the first frame update
    private void Start()
    {
        anim = GetComponent<Animator>();
        bd = FindObjectOfType<BehaviourDispensor>();
        anim.runtimeAnimatorController = bd.GetBehaviourTree();
    }
    public void SetCanMove(bool state)
    {
        canMove = state;
    }
    public void ConfirmRayTest(bool state)
    {
        if(state == true)
        {
            anim.SetTrigger("RayTrue");
        }
        else
        {
            anim.SetTrigger("RayFalse");
        }
    }
}
