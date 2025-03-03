using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Shop_interaction_manager : MonoBehaviour
{
    [Header("Shop")]

    // UI label prefab that pops up when player comes near
    [SerializeField]
    private GameObject labelPrefab;

    // Text to display within UI label
    [SerializeField]
    private string labelTextHotkeyInfo = "(E) Buy";
    [SerializeField]
    private string labelTextHotkeyInfoColor = "#F0FFFF"; // Pale, electric blue
    [SerializeField]
    private int labelTextItemCost = 0;
    [SerializeField]
    private string labelTextItemCostColor = "#FFDF00"; // Gold yellow
    [SerializeField]
    private string labelTextItemName = "Default Name";
    [SerializeField]
    private string labelTextItemDesc = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Nulla facilisi. Vestibulum ante ipsum primis in faucibus orci luctus et ultrices posuere cubilia curae; Donec eget justo at ligula vehicula tincidunt. Sed auctor, velit nec efficitur, nunc sapien";
    // Position and rotation of UI label
    [SerializeField]
    private float[] labelRotationXYZ;
    [SerializeField]
    private float[] labelPositionOffsetXYZ;

    // Hitbox size of the UI label popup trigger
    [SerializeField]
    private float[] labelTriggerHitboxSize;
    
    // Instiantiated UI label object from the prefab
    private GameObject label;

    private bool isShopActive = false;
    private bool bought = false;

    // LineRenderers for the lines that connect to each corner of the label
    // lrBl = "LineRenderer Bottom Left Corner", etc.
    private LineRenderer lrBl;
    private LineRenderer lrTl;
    private LineRenderer lrTr;
    private LineRenderer lrBr;

    // RectTransform of the label's main panel
    private RectTransform labelRectTransform;

    // Positions of the corners of the RectTransform of the label's main panel
    private Vector3 labelBottomLeft;
    private Vector3 labelTopLeft;
    private Vector3 labelTopRight;
    private Vector3 labelBottomRight;

    // The position of the center point of the top face of this ShopItem
    private Vector3 topFaceCenterPos;

    // The upgrade associated with this ShopItem.
    public Upgrade upgrade;

    // Start is called before the first frame update
    void Start()
    {
        // If we are given an upgrade object, then use that object's Name and Description for the label
        if (upgrade != null) { 
            labelTextItemName = upgrade.Name;
            labelTextItemDesc = upgrade.Desc;
            labelTextItemCost = upgrade.Cost;
        }
        // Instantiate the UI label with some text, rotation, and position
        label = Instantiate(labelPrefab, transform.position, Quaternion.identity);
        label.name = label.name + "_" + gameObject.name.Substring(gameObject.name.Length - 1, 1);
        TextMeshProUGUI tmpText = label.transform.Find("Panel").gameObject.transform.Find("TMP").gameObject.GetComponent<TextMeshProUGUI>();
        tmpText.text = MakeFullFormattedTextString();
        label.transform.rotation = Quaternion.Euler(labelRotationXYZ[0], labelRotationXYZ[1], labelRotationXYZ[2]);
        label.transform.position += new Vector3(labelPositionOffsetXYZ[0], labelPositionOffsetXYZ[1], labelPositionOffsetXYZ[2]);
        label.transform.SetParent(gameObject.transform);

        // Resize the label's panel accordingly
        Item_label_resize.instance.UpdateSize();

        // Assign the positions of the label's corners
        Vector3[] corners = new Vector3[4];
        labelRectTransform = (RectTransform)label.transform.Find("Panel");
        labelRectTransform.GetWorldCorners(corners);
        labelBottomLeft = corners[0];
        labelTopLeft = corners[1];
        labelTopRight = corners[2];
        labelBottomRight = corners[3];

        // Sets the hitbox size for the UI-label-popup-trigger of this ShopItem
        BoxCollider[] colliders = GetComponents<BoxCollider>();
        BoxCollider triggerCollider = colliders.FirstOrDefault(c => c.isTrigger);
        if (triggerCollider)
        {
            triggerCollider.size = new Vector3(labelTriggerHitboxSize[0], labelTriggerHitboxSize[1], labelTriggerHitboxSize[2]);
        }

        // Assign the position of the center point of the top face of this ShopItem
        Renderer cubeRenderer = GetComponent<MeshRenderer>();
        Vector3 center = cubeRenderer.bounds.center;
        float height = cubeRenderer.bounds.extents.y;
        topFaceCenterPos = center + new Vector3(0, height + 0.1f, 0);

        // Assign the LineRenderers for the lines to the corners of the label, and toggle off rendering for now
        lrBl = gameObject.transform.Find("LineRendererBottomLeft").GetComponent<LineRenderer>();
        lrTl = gameObject.transform.Find("LineRendererTopLeft").GetComponent<LineRenderer>();
        lrTr = gameObject.transform.Find("LineRendererTopRight").GetComponent<LineRenderer>();
        lrBr = gameObject.transform.Find("LineRendererBottomRight").GetComponent<LineRenderer>();
        lrBl.enabled = false;
        lrTl.enabled = false;
        lrTr.enabled = false;
        lrBr.enabled = false;

        // Make the label invisible
        label.SetActive(false);
    }

    // Called when another collider enters the trigger hitbox
    private void OnTriggerEnter(Collider other) 
    {
        // If the other collider is the player, and the item has not already been bought:
        // Make the UI label visible and allow the player to interact with this instance's gameObject; draw the lines
        if (other.gameObject.tag == "Player" && !bought) 
        {
            label.SetActive(true);
            Player_input_manager.instance.Interactable = gameObject;
            isShopActive = true;
            DrawLinesToLabelCorners();
        }
    }

    // Called when another collider exits the trigger hitbox
    private void OnTriggerExit(Collider other) 
    {
        // If the other collider is the player, and the item has not already been bought:
        // Make the UI label invisible and disallow the player to interact with this instance's gameObject; make the lines invisble
        if (other.gameObject.tag == "Player" && !bought)
        {
            label.SetActive(false);
            isShopActive = false;
            DrawLinesToLabelCorners();
        }
    }

    // Called when the player interacts with this instance's gameObject
    public void buy()
    {
        // Stops this gameObject's attempts to interact with the label
        bought = true;
        // Destroys all children of this instance's gameObject
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }

    // Makes a single formatted string from this ShopItem's hotkey info, name, and description strings
    public string MakeFullFormattedTextString()
    {
        string text = string.Empty;
        text += "<line-height=90%><b><color=" + labelTextHotkeyInfoColor + ">" + labelTextHotkeyInfo + "</color></b>\n";
        text += "<line-height=125%><b><size=75%><color=" + labelTextItemCostColor + "> Cost: " + labelTextItemCost.ToString() + " Coins</color></size></b>\n";
        text += "<line-height=95%><b>" + labelTextItemName + "</b>\n";
        text += "<i><size=75%>" + labelTextItemDesc + "</size></i>";
        return text;
    }

    // Either draws the lines to the-corners-of-the-label from the-center-point-of-the-top-face-of-this-ShopItem
    // Or makes them invisible, depending on whether the shop is active.
    private void DrawLinesToLabelCorners() 
    {
        if (isShopActive)
        {
            lrBl.enabled = true;
            lrTl.enabled = true;
            lrTr.enabled = true;
            lrBr.enabled = true;

            lrBl.SetPosition(0, topFaceCenterPos);
            lrBl.SetPosition(1, labelBottomLeft);

            lrTl.SetPosition(0, topFaceCenterPos);
            lrTl.SetPosition(1, labelTopLeft);

            lrTr.SetPosition(0, topFaceCenterPos);
            lrTr.SetPosition(1, labelTopRight);

            lrBr.SetPosition(0, topFaceCenterPos);
            lrBr.SetPosition(1, labelBottomRight);
        }
        else
        {
            lrBl.enabled = false;
            lrTl.enabled = false;
            lrTr.enabled = false;
            lrBr.enabled = false;
        }

    }

    // Getters, Setters
    public bool IsShopActive 
    {
        get { return isShopActive; }
        set { isShopActive = value; }
    }

    public string LabelTextHotkeyInfo
    {
        get { return labelTextHotkeyInfo; }
        set { labelTextHotkeyInfo = value; }
    }

    public string LabelTextHotkeyInfoColor
    {
        get { return labelTextHotkeyInfoColor; }
        set { labelTextHotkeyInfoColor = value; }
    }

    public string LabelTextItemName
    {
        get { return labelTextItemName; }
        set { labelTextItemName = value; }
    }

    public string LabelTextItemDesc
    {
        get { return labelTextItemDesc; }
        set { labelTextItemDesc = value; }
    }
}
