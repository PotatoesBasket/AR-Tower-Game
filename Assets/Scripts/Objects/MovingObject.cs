﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingObject : MonoBehaviour
{
    // create 2 empty gameObjects and assign them to this script as waypoints

    // recommended hierarchy setup:
    // blank object parent
    //   cube w/script attached
    //   startpoint object
    //   endpoint object

    // bug me to make an editor script for setting up the object if that's too hard lol

    // points will be visualised as red and blue spheres (start and end respectively)
    public GameObject startPoint;
    public GameObject endPoint;
    public float animationLength;
    float timer;
    bool reverse = false;

    private void Update()
    {
        if (timer >= animationLength)
            reverse = true;
        if (timer <= 0)
            reverse = false;

        if (!reverse)
            timer += Time.deltaTime;
        else
            timer -= Time.deltaTime;

        if (animationLength > 0)
        {
            transform.position = Vector3.Lerp(
            startPoint.transform.position,
            endPoint.transform.position,
            timer / animationLength);
        }
    }

    private void OnDrawGizmos()
    {
        if (startPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(startPoint.transform.position, 0.2f);
        }

        if (endPoint != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(endPoint.transform.position, 0.2f);
        }
    }
}