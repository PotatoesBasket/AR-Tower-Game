using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInfo : MonoBehaviour
{
    public Touch touch;
    public TouchPhase touchPhase;

    public Vector3 startTouchPos;
    public Vector3 currentTouchPos;
    public float direction = 0;
    public float originRadius = 0.5f;

    public float touchTimer = 0;
    public float tapTimeLimit = 0.3f;

    public bool activateTap = false;
    public bool endFrame = false;

    public bool IsTouching { get; private set; } = false;

    private void FixedUpdate()
    {
        touchPhase = touch.phase;

        if ((touch.phase != TouchPhase.Ended || touch.phase != TouchPhase.Canceled))
            touchTimer += Time.fixedDeltaTime; //time how long touch occurs for

        switch (touch.phase)
        {
            case TouchPhase.Began:
                startTouchPos = touch.position;
                currentTouchPos = touch.position;
                IsTouching = true;
                endFrame = true; //reset end frame check
                break;

            case TouchPhase.Moved:
            case TouchPhase.Stationary:

                currentTouchPos = touch.position; //update touch position

                //get normalised direction of drag movement
                Vector2 dirVec = currentTouchPos - startTouchPos;

                if (dirVec.magnitude > originRadius)
                    direction = Vector2.Dot(Vector2.right, new Vector2(dirVec.normalized.x, 0));
                else
                    direction = 0;
                break;

            case TouchPhase.Ended:
                if (endFrame == true) //first frame of ended phase only
                {
                    if (touchTimer < tapTimeLimit + Time.fixedDeltaTime)
                        activateTap = true;

                    Debug.Log(touchTimer + " < " + (tapTimeLimit + Time.fixedDeltaTime).ToString());
                    startTouchPos = Vector2.zero; //reset touch positions
                    currentTouchPos = Vector2.zero;
                    touchTimer = 0;
                    direction = 0;
                    IsTouching = false;
                    endFrame = false;
                }

                break;
        }
    }
}