using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class Upgrade_manager : MonoBehaviour
{
    // This prefab is a self-reference to this gameObject's prefab. Used mostly for clarity.
    public GameObject upgradePrefab;

    // UI panel where the upgrade-related things will go
    private Transform upgradeSlotsPanel;

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

    // Update is called once per frame
    void Update()
    {
        // Destroy this game object immediately post-start as it does nothing, it is only used to pass data down to children
        Destroy(gameObject);
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
        // Icons are children of the MainCanvas
        GameObject mainUI = GameObject.Find("UI");
        upgradeSlotsPanel = mainUI.transform.Find("MainCanvas").Find("UpgradeUISlots");
        GameObject upgradeUIIcon = Instantiate(upgradePrefab.transform.GetChild(1).gameObject);
        upgradeUIIcon.transform.SetParent(mainUI.transform.Find("MainCanvas"));
        upgradeUIIcon.gameObject.name = upgradeUIIcon.gameObject.name + "_" + gameObject.name.Substring(gameObject.name.Length - 1, 1);

        // Icons render on bottom layer
        upgradeUIIcon.transform.SetSiblingIndex(0);

        // Get the image data from the file path, convert to a Sprite, and set the Image to the Sprite.
        byte[] imageBytes = GetImageBytes(upgrade.SpriteFilePath);
        Texture2D tex = new Texture2D(2, 2, TextureFormat.RGBA32, false);
        ImageConversion.LoadImage(tex, imageBytes);
        Texture2D scaledTex = new Texture2D(150, 102, TextureFormat.RGBA32, false);
        Graphics.ConvertTexture(tex, scaledTex);
        Sprite sprite = Sprite.Create(scaledTex, new Rect(0, 0, scaledTex.width, scaledTex.height), new Vector2(0.5f, 0.5f));
        Image uiImage = upgradeUIIcon.GetComponent<Image>();
        uiImage.sprite = sprite;

        // Set the position based on which upgrade this is, set the size according to a 1920 x 1080 resolution
        upgradeUIIcon.GetComponent<RectTransform>().anchoredPosition = new Vector2(108, 761 - (108) * upgradeIndex);
        upgradeUIIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(150, 102);
    }

    // Instantiates an upgrade and draws it on top of a ShopItem
    private void InstantiateUpgradeShopItem()
    {
        Transform upgradeShopItem;
        upgradeShopItem = Instantiate(upgradePrefab.transform.GetChild(0), transform.position + new Vector3(0.07f, 0, 0.06f), Quaternion.Euler(new Vector3(45, 34.9999924f, 0)));

        upgradeShopItem.transform.SetParent(shopItem);
        upgradeShopItem.gameObject.name = upgradeShopItem.gameObject.name + "_" + gameObject.name.Substring(gameObject.name.Length - 1, 1);

        // Makes the material of the mesh an image of the upgrade
        Renderer renderer = upgradeShopItem.GetComponent<MeshRenderer>();
        byte[] imageBytes = GetImageBytes(upgrade.SpriteFilePath);
        Texture2D tex = new Texture2D(0, 0);
        ImageConversion.LoadImage(tex, imageBytes);
        if (tex != null)
        {
            renderer.material.mainTexture = tex;

            // Customize material settings to allow transparent rendering, and full color regardless of lighting.
            Shader transparentShader = Shader.Find("Unlit/Transparent");
            if (transparentShader != null)
            {
                renderer.material.shader = transparentShader;
                renderer.material.SetFloat("_Mode", 3); // 3 corresponds to Transparent mode
                renderer.material.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
                renderer.material.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                renderer.material.SetInt("_ZWrite", 0); // Disable depth write for transparency
                renderer.material.DisableKeyword("_ALPHATEST_ON");
                renderer.material.EnableKeyword("_ALPHABLEND_ON");
                renderer.material.DisableKeyword("_ALPHAPREMULTIPLY_ON");
                renderer.material.renderQueue = 3000; // Transparent queue
            }
        }
        upgradeShopItem.localScale = new Vector3(0.0900000036f, 0.765000045f, 0.0612000041f);
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
    public Upgrade(string name, string desc, float x, float n, string type, int id, int cost, string spriteFilePath)
    {
        this.upgrade_name = name;
        this.desc = desc;
        this.x = x;
        this.n = n;
        this.type = type;
        this.id = id;
        this.cost = cost;
        this.spriteFilePath = spriteFilePath;

        this.desc = desc.Replace("[x]", ((int)this.x).ToString());
        this.desc = this.desc.Replace("[X]", ((int)this.x).ToString());
        this.desc = this.desc.Replace("[n]", ((int)this.n).ToString());
        this.desc = this.desc.Replace("[N]", ((int)this.n).ToString());
        this.desc = this.desc.Replace("[type]", this.type);

        this.uiOrShopItem = "ShopItem";
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
}