using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOptions : MonoBehaviour
{
    public static GameOptions Instance;

    public bool MuteSFX = false;
    public bool MuteMusic = false;
    public bool TestARInEditor = false;

    void Awake()
    {
        KeepObject();
    }

    void KeepObject()
    {
        if (Instance == null) //set single instance
            Instance = this;
        else //destroy dummy gms
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject); //object will persist between scenes
    }
}