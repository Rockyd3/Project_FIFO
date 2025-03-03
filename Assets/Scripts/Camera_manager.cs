using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_manager : MonoBehaviour
{
    public static Camera_manager instance;
    [Header("Camera")]
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private float[] cameraOffsetXYZ = new float[3];

    private void Awake() // Camera_manager callable in any script: Camera_manager.instance.[]
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Confined; 
        GameObject p = GameObject.FindWithTag("Player");
        if (p != null) {
            player = p;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = new Vector3(player.transform.position.x + cameraOffsetXYZ[0], player.transform.position.y + cameraOffsetXYZ[1], player.transform.position.z + cameraOffsetXYZ[2]);
    }

    // Getters, Setters
    public GameObject Player
    {
        get { return player; }
        set { player = value; }
    }
}
