using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform player;

    void Update()
    {
        transform.LookAt(new Vector3(player.localPosition.x, player.localPosition.y, player.localPosition.z));
    }
}
