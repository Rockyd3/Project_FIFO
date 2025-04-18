using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.IO;

public class Upgrade_manager : MonoBehaviour
{
    // This prefab is a self-reference to this gameObject's prefab. Used mostly for clarity.
    public GameObject upgradePrefab;

    // This prefab is a reference to the upgrade's UI label (unused if this upgrade is a shopItem)
    private GameObject MainUI;

    // The upgrade this gameObject is working with 
    public Upgrade upgrade;

    // The transform of the shopItem this upgrade is associated with, if it exists (unused if this upgrade is a UI icon)
    [System.NonSerialized]
    public Transform shopItem;

    // The wall the shopItems are placed on (unused if this upgrade is a UI icon)
    [System.NonSerialized]
    public string wallDirection;

    // The index of this upgrade within the PlayerHeldUpgrades list in the Level manager (unused if this upgrade is a ShopItem)
    public int upgradeIndex;

    // The GameObject that will be instantiated if this upgrade is a UI icon
    public GameObject upgradeUIIcon;

    // The UI label corresponding to this upgrade (unused if this upgrade is a shopItem)
    public GameObject Label;

    // The screen space positions of the corners of this gameObject (unused if this upgrade is a shopItem).
    private Vector3[] UIIconCorners = new Vector3[4];

    void Start()
    {
        gameObject.SetActive(false);
    }

    // Makes different GameObjects depending on whether the upgrade is supposed to be a UI icon or ShopItem.
    public void CreateGameObjects()
    {
        if (upgrade.UIOrShopItem.Equals("UI"))
        {
            InstantiateUpgradeUIIcon();
        }
        else if (upgrade.UIOrShopItem.Equals("ShopItem"))
        {
            InstantiateUpgradeShopItem();
        }
    }

    // Renders the upgrade UI 
    // All magic numbers are positions relative to a 1920 x 1080 resolution.
    private void InstantiateUpgradeUIIcon()
    {   
        // Icons are children of the "Upgrades" child of the Main UI canvas"
        GameObject parentUI = Level_manager.instance.parentUI;
        gameObject.transform.SetParent(Level_manager.instance.transform);
        upgradeUIIcon = Instantiate(upgradePrefab.transform.GetChild(1).gameObject);
        upgradeUIIcon.transform.SetParent(parentUI.transform.Find("MainCanvas/Upgrades"));
        upgradeUIIcon.name = upgradeUIIcon.name + "_" + gameObject.name.Substring(gameObject.name.Length - 1, 1);

        // Icons render on bottom layer
        upgradeUIIcon.transform.SetSiblingIndex(0);

        // Get the image data from the file path, convert to a Sprite, and set the Image to the Sprite.
        byte[] imageBytes = GetImageBytes(upgrade.SpriteFilePath);
        Texture2D tex = new(2, 2, TextureFormat.RGBA32, false);
        ImageConversion.LoadImage(tex, imageBytes);
        Texture2D scaledTex = new(150, 102, TextureFormat.RGBA32, false);
        Graphics.ConvertTexture(tex, scaledTex);
        Sprite sprite = Sprite.Create(scaledTex, new Rect(0, 0, scaledTex.width, scaledTex.height), new Vector2(0.5f, 0.5f));
        Image uiImage = upgradeUIIcon.GetComponent<Image>();
        uiImage.sprite = sprite;

        // Set the position based on which player held upgrade index this is, set the size according to a 1920 x 1080 resolution
        Vector2 pos = new Vector2(108, 691 - (108) * upgradeIndex);
        upgradeUIIcon.GetComponent<RectTransform>().anchoredPosition = pos;
        upgradeUIIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 102);
        upgradeUIIcon.GetComponent<RectTransform>().GetWorldCorners(UIIconCorners);
        UIIconCorners[0] += new Vector3(39, 24, 0);
        UIIconCorners[1] += new Vector3(39, -6, 0);
        UIIconCorners[2] += new Vector3(-39, -6, 0);
        UIIconCorners[3] += new Vector3(-39, 24, 0);
        upgradeUIIcon.SetActive(true);
        MainUI = GameObject.Find("UI");
        Label = MainUI.transform.Find("MainCanvas/Upgrades/Label").gameObject;
        Label.GetComponent<UpgradeLabelMainUI>().Initialize();
    }

    // Instantiates an upgrade and draws it on top of a ShopItem
    private void InstantiateUpgradeShopItem()
    {
        Transform upgradeShopItem;
        upgradeShopItem = Instantiate(upgradePrefab.transform.GetChild(0), transform.position + new Vector3(0, 0.585f, 0), Quaternion.Euler(new Vector3(48.59f, -135, 0)));

        upgradeShopItem.transform.SetParent(shopItem);
        upgradeShopItem.gameObject.name = "Upgrade ShopItem Icon " + upgrade.Name;

        // Makes the material of the mesh an image of the upgrade
        if (!File.Exists(upgrade.SpriteFilePathVert))
        {
            Debug.LogError($"Image file not found at path: {upgrade.SpriteFilePathVert}");
            return;
        }

        byte[] imageBytes = File.ReadAllBytes(upgrade.SpriteFilePathVert);
        Texture2D texture = new Texture2D(2, 2); // placeholder size

        if (texture.LoadImage(imageBytes))
        {
            Sprite newSprite = Sprite.Create(
                texture,
                new Rect(0, 0, texture.width, texture.height),
                new Vector2(0.5f, 0.5f)
            );

            upgradeShopItem.GetComponent<SpriteRenderer>().sprite = newSprite;
            upgradeShopItem.GetComponent<SpriteRenderer>().transform.localScale = Vector3.one * 2.5f;
        }
        else
        {
            Debug.LogError("Failed to create texture from image bytes.");
        }
        upgradeShopItem.gameObject.SetActive(true);
    }

    // Helper method to convert image as path to byte array.
    private byte[] GetImageBytes(string path)
    {
        if (File.Exists(path))
        {
            return File.ReadAllBytes(path);
        }
        else
        {
            Debug.LogError("File not found at: " + path);
            return null;
        }
    }

    // When the UI icon for this upgrade is clicked, update the index of the currently selected upgrade in the player held upgrades list in the level manager to reflect this.
    public void OnUIIconClick() {
        Debug.Log("Upgrade with name: " + upgrade.Name + " clicked! (Upgrade slot index " + upgradeIndex + ")");
        Level_manager.instance.CurrentlySelectedUpgradeIndex = upgradeIndex;
    }

    public void OnUIHoverEnter()
    {
        if (Level_manager.instance.isPaused) 
        {
            Level_manager.instance.isHoveringUpgradeIcon = true;
            Debug.Log("Hovered");
            upgradeUIIcon.GetComponent<RectTransform>().GetWorldCorners(UIIconCorners);
            UIIconCorners[0] += new Vector3(39, 24, 0);
            UIIconCorners[1] += new Vector3(39, -6, 0);
            UIIconCorners[2] += new Vector3(-39, -6, 0);
            UIIconCorners[3] += new Vector3(-39, 24, 0);
            Level_manager.instance.CurrentlyHoveredUpgradeIndex = upgradeIndex;
            Label.GetComponent<RectTransform>().anchoredPosition = new Vector2(-440, -60);
            Label.transform.parent.Find("LineBL").gameObject.SetActive(true);
            Label.transform.parent.Find("LineTL").gameObject.SetActive(true);
            Label.transform.parent.Find("LineTR").gameObject.SetActive(true);
            Label.transform.parent.Find("LineBR").gameObject.SetActive(true);
            Label.transform.parent.Find("HoverSquare").gameObject.SetActive(true);
            Label.SetActive(true);
            Label.GetComponent<UpgradeLabelMainUI>().upgradeIconCorners = UIIconCorners;
            Label.GetComponent<UpgradeLabelMainUI>().MakeFullFormattedString(upgrade);
        }
    }

    public void OnUIHoverExit()
    {
        Level_manager.instance.isHoveringUpgradeIcon = false;
        Debug.Log("Unhovered");
        Label.SetActive(false);
        Label.transform.parent.Find("LineBL").gameObject.SetActive(false);
        Label.transform.parent.Find("LineTL").gameObject.SetActive(false);
        Label.transform.parent.Find("LineTR").gameObject.SetActive(false);
        Label.transform.parent.Find("LineBR").gameObject.SetActive(false);
        Label.transform.parent.Find("HoverSquare").gameObject.SetActive(false);
    }
}

// All the relevant data/behaviors an upgrade has
public class Upgrade
{
    // See constructor for info about fields
    private string upgrade_name;
    private string desc;
    private float x;
    private float n;
    private string type;
    private int id;
    private int cost;
    private string spriteFilePath;
    private string spriteFilePathVert;
    private string initDesc;
    private string color;

    // "UI" for the upgrade manager to instantiate a UI icon. "ShopItem" for the upgrade manager to instantiate a ShopItem upgrade.
    private string uiOrShopItem;

    // Params:
    //  string name: name of upgrade
    //  string desc: description of what the upgrade does. Format variables (if present) with square brackets i.e. "[x]%" or "[type]" to replace them with the scalars automatically.
    //               Percent signs should be left outside the brackets.
    //  float x: some scalar that controls the power of the upgrade i.e. Deal [x] more damage per hit. Set to 0.0f if not used.
    //  float n: some scalar that controls the power of the upgrade. Set to 0.0f if not used.
    //  string type: the type of enemy that the upgrade affects. Set to "" if not used.
    //  int id: index of upgrade in game_constants array. Must be unique to this upgrade.
    //  int cost: cost of upgrade
    //  string spriteFilePath: file path of the sprite of this Upgrade.
    public Upgrade(string name, string desc, float x, float n, string type, int id, int cost, string spriteFilePath, string spriteFilePathVert, string color)
    {
        upgrade_name = name;
        this.desc = desc;
        initDesc = desc;
        this.x = x;
        this.n = n;
        this.type = type;
        this.id = id;
        this.cost = cost;
        this.spriteFilePath = spriteFilePath;
        this.spriteFilePathVert = spriteFilePathVert;
        this.desc = this.desc.Replace("[x]", ((int)this.x).ToString());
        this.desc = this.desc.Replace("[X]", ((int)this.x).ToString());
        this.desc = this.desc.Replace("[n]", ((int)this.n).ToString());
        this.desc = this.desc.Replace("[N]", ((int)this.n).ToString());
        this.desc = this.desc.Replace("[type]", this.type);

        this.uiOrShopItem = "ShopItem";
        this.Color = color;
    }
    public void UpdateDesc() {
        desc = initDesc;
        desc = desc.Replace("[x]", ((int)x).ToString());
        desc = desc.Replace("[X]", ((int)x).ToString());
        desc = desc.Replace("[n]", ((int)n).ToString());
        desc = desc.Replace("[N]", ((int)n).ToString());
        desc = desc.Replace("[type]", type);
    }

    // Getters, Setters
    public string Name
    {
        get { return upgrade_name; }
        set { upgrade_name = value; }
    }

    public string Desc
    {
        get { return desc; }
        set { desc = value; }
    }

    public float X
    {
        get { return x; }
        set { x = value; }
    }

    public float N
    {
        get { return n; }
        set { n = value; }
    }

    public string Type
    {
        get { return type; }
        set { type = value; }
    }

    public int Id
    {
        get { return id; }
        set { id = value; }
    }

    public string UIOrShopItem
    {
        get { return uiOrShopItem; }
        set { uiOrShopItem = value; }
    }

    public string SpriteFilePath
    {
        get { return spriteFilePath; }
        set { spriteFilePath = value; }
    }

    public int Cost
    { 
        get { return cost; }
        set { cost = value; }
    }
    public string SpriteFilePathVert { get => spriteFilePathVert; set => spriteFilePathVert = value; }
    public string Color { get => color; set => color = value; }
}