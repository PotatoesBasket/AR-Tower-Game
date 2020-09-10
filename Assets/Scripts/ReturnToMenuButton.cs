using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnToMenuButton : MonoBehaviour
{
    SceneSwitcher s;

    public void GoToMenu()
    {
        s = GameObject.FindGameObjectWithTag("Manager").GetComponent<SceneSwitcher>();
        s.LoadMenu();
    }
}