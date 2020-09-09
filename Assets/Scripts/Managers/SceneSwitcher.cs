using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneSwitcher : MonoBehaviour
{
    public void LoadMenu()
    {
        SceneManager.LoadScene("Menu");
    }

    public void LoadLV1()
    {
        SceneManager.LoadScene("PuzzleTower1T3");
    }

    public void LoadLV2()
    {
        SceneManager.LoadScene("PuzzleTower2T3");
    }
}