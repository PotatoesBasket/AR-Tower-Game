using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateArena : MonoBehaviour
{
    public float speed = 10;
    [Header("Press P to toggle")]
    public bool auto = false;

    Vector3 lastPos;
    Vector3 moved;

    Player player;

    private void Start()
    {
        lastPos = transform.position;

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
            auto = !auto;
    }

    private void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Z))
            transform.Rotate(new Vector3(0, -speed, 0) * Time.fixedDeltaTime);
        if (Input.GetKey(KeyCode.X))
            transform.Rotate(new Vector3(0, speed, 0) * Time.fixedDeltaTime);

        if (auto)
            transform.Rotate(new Vector3(0, speed, 0) * Time.fixedDeltaTime);

        moved = transform.position - lastPos;
        lastPos = transform.position;
        player.transform.position += moved;
    }
}