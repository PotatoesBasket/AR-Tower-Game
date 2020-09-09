using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppearDisappear : MonoBehaviour
{
    MeshRenderer meshRenderer;
    BoxCollider boxCollider;

    public float appearTime = 1.0f;
    public float disappearTime = 1.0f;

    float timer = 0;
    bool on = true;

    private void Start()
    {
        meshRenderer = GetComponentInChildren<MeshRenderer>();
        boxCollider = GetComponentInChildren<BoxCollider>();
    }

    private void Update()
    {
        timer += Time.deltaTime;

        if (on)
        {
            if (timer > appearTime)
            {
                meshRenderer.enabled = false;
                boxCollider.enabled = false;
                timer = 0;
                on = false;
            }
        }
        else
        {
            if (timer > disappearTime)
            {
                meshRenderer.enabled = true;
                boxCollider.enabled = true;
                timer = 0;
                on = true;
            }
        }
    }
}