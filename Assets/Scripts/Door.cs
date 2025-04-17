using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Door : MonoBehaviour
{
    [SerializeField]
    string room_name;

    void OnTriggerEnter(Collider other)
    {
        GameState.Instance.roomCount++;

        // Load the Basic L Room scene when this is entered
        // FIXME: make this adjustable room type
        // Bugfix: made it so only the player can trigger the door.
        if (other.tag == "Player")
        {
            SceneManager.LoadScene(room_name);
        }
        Player_Controller.controller.UpgradeBoost();
    }
}



