using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine;

public class Level_manager : MonoBehaviour
{
    public static Level_manager instance;
    private bool isPaused = false;
  
     private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
     }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) // Press 'P' to toggle pause
        {
            TogglePause();
        }
    }

    void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f; // Pause game
            Debug.Log("Game Paused");
        }
        else
        {
            Time.timeScale = 1f; // Resume game
            Debug.Log("Game Resumed");
        }
    }
}
