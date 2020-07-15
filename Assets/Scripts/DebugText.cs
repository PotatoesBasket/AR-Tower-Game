using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugText : MonoBehaviour
{
    public Text updateFPS;
    public Text fixedUpdateFPS;
    public Text isGrounded;

    public Player player;

    private void Update()
    {
        updateFPS.text = "Update FPS: " + (1.0 / Time.deltaTime).ToString("F2");
    }

    private void FixedUpdate()
    {
        fixedUpdateFPS.text = "Fixed update FPS: " + (1.0 / Time.fixedDeltaTime).ToString("F2");
        isGrounded.text = "Is Grounded: " + player.IsGrounded().ToString();
    }
}