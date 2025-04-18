using System.Linq;
using UnityEngine;

public class Trashcan : MonoBehaviour
{
    [Header("Shop")]
    // UI label prefab that pops up when player comes near
    [SerializeField]
    private GameObject labelPrefab;

    // Position and rotation of UI label
    [SerializeField]
    private float[] labelRotationXYZ;
    [SerializeField]
    private float[] labelPositionOffsetXYZ;

    // Hitbox size of the UI label popup trigger
    [SerializeField]
    private float[] labelTriggerHitboxSize;

    // Instiantiated UI label object from the prefab
    [System.NonSerialized]
    public GameObject label;

    // The position of the center point of the top face of this ShopItem
    private Vector3 topFaceCenterPos;

    // The currently selected upgrade, as dictated by Level_manager's currentlySelectedUpgradeIndex
    public Upgrade upgrade;

    // The sprites associated with the Trashcan
    public Sprite trashcanEmpty;
    public Sprite trashcanGreen;
    public Sprite trashcanPurple;
    public Sprite trashcanRed;
    public Sprite trashcanYellow;

    private Transform colliderTransform;
    private Transform spriteTransform;

    public bool IsTrashcanActive { get; set; } = false;

    // Start is called before the first frame update
    void Start()
    {
        colliderTransform = transform.Find("Collider");
        label = Instantiate(labelPrefab, colliderTransform.position, Quaternion.identity);
        label.name = "Trashcan Label";
        label.GetComponent<UpgradeLabel>().IsTrashcan = true;
        label.GetComponent<UpgradeLabel>().Initialize(upgrade);
        label.transform.rotation = Quaternion.Euler(labelRotationXYZ[0], labelRotationXYZ[1], labelRotationXYZ[2]);
        label.transform.position += new Vector3(labelPositionOffsetXYZ[0], labelPositionOffsetXYZ[1], labelPositionOffsetXYZ[2]);
        label.transform.SetParent(colliderTransform);

        // Sets the hitbox size for the UI-label-popup-trigger of this ShopItem
        BoxCollider[] colliders = colliderTransform.GetComponents<BoxCollider>();
        BoxCollider triggerCollider = colliders.FirstOrDefault(c => c.isTrigger);
        if (triggerCollider)
        {
            triggerCollider.size = new Vector3(labelTriggerHitboxSize[0], labelTriggerHitboxSize[1], labelTriggerHitboxSize[2]);
        }

        // Assign the position of the center point of the top face of this ShopItem
        Renderer cubeRenderer = colliderTransform.GetComponent<MeshRenderer>();
        Vector3 center = cubeRenderer.bounds.center;
        float height = cubeRenderer.bounds.extents.y;
        topFaceCenterPos = center + new Vector3(0, height + 0.1f, 0);

        // Assign the trashcan sprite
        spriteTransform = transform.Find("Sprite");
        spriteTransform.GetComponent<SpriteRenderer>().sprite = trashcanEmpty;

        label.SetActive(false);
    }

    // Called when the player interacts with this instance's gameObject
    public void use()
    {
    }

    // Makes a single formatted string from this ShopItem's hotkey info, name, and description strings
    public string MakeFullFormattedTextString()
    {
        string text = "";
        if (label != null)
        {
            text = label.GetComponent<UpgradeLabel>().MakeFullFormattedTextString(label, true);
        }
        DrawLinesToLabelCorners();
        return text;
    }

    // Either draws the lines to the-corners-of-the-label from the-center-point-of-the-top-face-of-this-ShopItem
    // Or makes them invisible, depending on whether the shop is active.
    public void DrawLinesToLabelCorners()
    {
        if (label != null)
        {
            label.GetComponent<UpgradeLabel>().DrawLinesToLabelCorners(true, topFaceCenterPos, topFaceCenterPos, topFaceCenterPos, topFaceCenterPos);
        }
    }

    public GameObject Label
    {
        get { return label; }
        set { label = value; }
    }
}
