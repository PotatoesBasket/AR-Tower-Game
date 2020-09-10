using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneSwitchButtons : MonoBehaviour
{
    SceneSwitcher s;

    public void GoToMenu()
    {
        s = GameObject.FindGameObjectWithTag("Manager").GetComponent<SceneSwitcher>();
        s.LoadMenu();
    }

    public void GoToLv1()
    {
        s = GameObject.FindGameObjectWithTag("Manager").GetComponent<SceneSwitcher>();
        s.LoadLV1();
    }

    public void GoToLv2()
    {
        s = GameObject.FindGameObjectWithTag("Manager").GetComponent<SceneSwitcher>();
        s.LoadLV2();
    }
}