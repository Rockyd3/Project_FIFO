using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class Shop_interaction_manager : MonoBehaviour
{
    [Header("Shop")]

    // UI label prefab that pops up when player comes near
    [SerializeField]
    private GameObject labelPrefab;

    // Text to display within UI label
    [SerializeField]
    private string labelText = "(E) Buy";

    // Position and rotation of UI label
    [SerializeField]
    private float[] labelRotationXYZ = {0.0f, -135.0f, 0.0f};
    [SerializeField]
    private float[] labelPositionOffsetXYZ = {0.0f, 0.75f, 0.0f};

    // Hitbox size of the UI label popup trigger
    [SerializeField]
    private float[] labelTriggerHitboxSize = {2.0f, 2.0f, 2.0f};
    
    // Instiantiated UI label object from the prefab
    private GameObject label;
    private bool isShopActive = false;

    // Start is called before the first frame update
    void Start()
    {
        // Instantiate the UI label with some text, rotation, and position, then make it invisible
        label = Instantiate(labelPrefab, transform.position, Quaternion.identity);
        label.transform.Find("Panel").gameObject.transform.Find("TMP").gameObject.GetComponent<TextMeshProUGUI>().text = labelText;
        label.transform.rotation = Quaternion.Euler(labelRotationXYZ[0], labelRotationXYZ[1], labelRotationXYZ[2]);
        label.transform.position += new Vector3(labelPositionOffsetXYZ[0], labelPositionOffsetXYZ[1], labelPositionOffsetXYZ[2]);
        label.SetActive(false);

        // Sets the hitbox size for the UI label popup trigger
        BoxCollider[] colliders = GetComponents<BoxCollider>();
        BoxCollider triggerCollider = colliders.FirstOrDefault(c => c.isTrigger);
        if (triggerCollider) 
        { 
            triggerCollider.size = new Vector3(labelTriggerHitboxSize[0],  labelTriggerHitboxSize[1], labelTriggerHitboxSize[2]);
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    // Called when another collider enters the trigger hitbox
    private void OnTriggerEnter(Collider other) 
    {
        // If the other collider is the player:
        // Make the UI label visible and allow the player to interact with this instance's gameObject
        if (other.gameObject.tag == "Player") 
        {
            label.SetActive(true);
            Player_input_manager.instance.Interactable = gameObject;
            isShopActive = true;
        }
    }

    // Called when another collider exits the trigger hitbox
    private void OnTriggerExit(Collider other) 
    {
        // If the other collider is the player:
        // Make the UI label invisible and disallow the player to interact with this instance's gameObject
        if (other.gameObject.tag == "Player")
        {
            label.SetActive(false);
            isShopActive = false;
        }
    }

    // Called when the player interacts with this instance's gameObject
    public void buy()
    {
        // Destroys this instance's gameObject
        Destroy(label);
        Destroy(gameObject);
    }

    // Getters, Setters
    public bool IsShopActive 
    {
        get { return isShopActive; }
        set { isShopActive = value; }
    }

    public string LabelText 
    {
        get { return labelText; }
        set { labelText = value; }
    }
}
