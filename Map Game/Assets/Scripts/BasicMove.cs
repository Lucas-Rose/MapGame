using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMove : MonoBehaviour
{
    [SerializeField] private float walkSpeed;
    [SerializeField] private float sprintSpeed;
    private float currSpeed;
    private Rigidbody rb;
    private int camIndex;
    // Update is called once per frame

    private void Start()
    {
        currSpeed = walkSpeed;
        rb = GetComponent<Rigidbody>();
    }
    void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");
        if(moveX != 0)
        {
            rb.position += moveX > 0 ? currSpeed * Time.deltaTime * transform.right : currSpeed * Time.deltaTime * -transform.right;
        }
        if (moveY != 0)
        {
            rb.position += moveY > 0 ? currSpeed * Time.deltaTime * transform.forward : currSpeed * Time.deltaTime * -transform.forward;
        }
        if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            currSpeed = sprintSpeed;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            currSpeed = walkSpeed;
        }
    }
}
