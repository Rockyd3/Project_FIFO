using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Singleton holding all the game constants
public class GameState : MonoBehaviour
{
    private static GameState instance;

    public static GameState Instance
    {
        get
        {
            if (instance == null)
            {
                Initialize();
                Debug.Log("GameState singleton initialized");
            }

            return instance;
        }
    }

    public int count;

    // Use this for intiialization logic instead of Start(), because Start() may be called
    // too late for other scripts to see data in this object.
    private static void Initialize()
    {
        instance = new GameObject().AddComponent<GameState>();
        instance.name = "Game State";

        instance.count = 7;

        DontDestroyOnLoad(instance.gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

}
