using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Corner : MonoBehaviour
{
    public float intendedAngle = 0;
    GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Corner"))
        {
            player.transform.localEulerAngles = new Vector3(0, intendedAngle, 0);
        }
    }
}