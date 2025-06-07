using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;

public class Level_manager : MonoBehaviour
{
    public static Level_manager instance;
    public bool isPaused = false;

    public int curr_room;
    private EnemySpawning ES;

    // Most upgrades the player can have at once
    public const int MAX_PLAYER_UPGRADES = 5;

    // The longest an element pattern can be
    public const int MAX_PATTERN_LEN = 5;

    [SerializeField]
    public PatternFuncs PF;

    [Header("Enemy Spawning")]
    public float EnemySpawnWarningTime;

    [Header("UI")]
    [SerializeField] private GameObject mainUIPrefab;
    [SerializeField] private GameObject musicManagerPrefab;
    public GameObject parentUI;
    private GameObject upgradesUI;
    private GameObject pauseMenuUI;
    private GameObject healthBarUI;
    private Button resumeButton;
    private Button pauseButton;
    private Button quitButton;
    private Button menuButton;
    [SerializeField] private GameObject upgradePrefab;
    [SerializeField] private ProgressBar healthBar;
    [SerializeField] private TMP_Text T_Currency;
    [SerializeField] public float PlayerHealth = -1;
    [SerializeField] public float PlayerMaxHealth = 20;


    public float UpgradeIconUnplugOffset; // how far the upgrade UI icons "unplug" when you select them
    [System.NonSerialized] public GameObject MusicManager;


    //FIXME: Add this list to a game_constants file
    [System.NonSerialized]
    public List<string> types = new List<string>() { "Earth", "Fire", "Ice", "Wind" };
    //[System.NonSerialized]
    public List<Color> typeColors = new List<Color>() { Color.green, Color.red, Color.blue, Color.cyan};
    static Pattern_UI_manager pat_man;
    [Header("Patterns")]
    [SerializeField]
    (int, int) currentPattern = (-1, -1);

    [SerializeField]
    public Transform Left_point;
    [SerializeField]
    public Transform Right_point;


    //FIXME: Add this to a game_constants file
    [System.NonSerialized]
    public List<List<(int, string, Action)>> Patterns = new List<List<(int, string, Action)>>();


    //FIXME: Add to game_constants
    [System.NonSerialized]
    public List<Upgrade> Upgrades = new List<Upgrade>();

    //[System.NonSerialized]
    [SerializeField]
    public List<int> Pattern_record = new List<int>();

    // The upgrades the player currently has
    [System.NonSerialized]
    public List<Upgrade> PlayerHeldUpgrades = new List<Upgrade>();

    // The UI icon gameObjects for the upgrades that the player currently has
    public List<GameObject> PlayerHeldUpgradeIcons = new List<GameObject>();

    // The upgrade the player clicks on when replacing/selling
    [System.NonSerialized]
    public int CurrentlySelectedUpgradeIndex = 0;

    // The upgrade the player is hovering over when paused
    [System.NonSerialized]
    public int CurrentlyHoveredUpgradeIndex = 0;

    [System.NonSerialized]
    public bool isHoveringUpgradeIcon = false;

    // If the player is currently selecting an upgrade to replace/sell
    [System.NonSerialized]
    public bool IsCurrentlySelectingUpgrade = false;

    [System.NonSerialized]
    public bool IsCurrentlySelectingRecycle = false;

    [Header("Upgrades")]
    public int[] PlayerHeldUpgradeIds = new int[MAX_PLAYER_UPGRADES];

    public int Currency;

    // Upgrade modifiers indicate the amount of effect an upgrade actually applies during runtime
    public int precisionUpgradeModifier = 0;
    public int hardwareAccelUpgradeModifier = 0;
    public int twoBirdsUpgradeModifier = 0;
    public int fortifiedUpgradeModifier = 0;
    public int bootUpUpgradeModifier = 15;
    public int spiceLifeUpgradeModifier = 0;
    public int gitRestoreUpgradeModifier = 0;
    public int bloodthirstyUpgradeModifier = 5;
    public int greedyUpgradeModifier = 0;
    public int thornsUpgradeModifier = 0;


    // Upgrade values are the fixed amount that the upgrade should affect by
    // FIXME: add all upgrade values to game_constants file
    [System.NonSerialized]
    public int precisionUpgradeModifierValue = 3;
    public int hardwareAccelUpgradeModifierValue = 10;
    public int twoBirdsUpgradeModifierValue = 10;
    public int fortifiedUpgradeModifierValue = 10;
    public int bootUpUpgradeModifierValue = 10;
    public int spiceLifeUpgradeModifierValue = 1;
    public int gitRestoreUpgradeModifierValue = 10;
    public int bloodthirstyUpgradeModifierValue = 1;
    public int greedyUpgradeModifierValue = 5;
    public int thornsUpgradeModifierValue = 10;

    private void Awake() //Makes levelmanager callable in any script: Level_manager.instance.[]
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

            //Instantiate the main UI
            parentUI = Instantiate(mainUIPrefab);
            parentUI.name = "UI";
            parentUI.transform.Find("MainCanvas").GetComponent<Canvas>().sortingOrder = 1000;
            DontDestroyOnLoad(parentUI);

            //Get HUD elements to keep them up to date
            healthBarUI = parentUI.transform.Find("MainCanvas/Healthbar").gameObject;
            healthBar = parentUI.GetComponent<ProgressBar>();
            T_Currency = parentUI.transform.Find("MainCanvas/Healthbar/Currency").GetComponent<TMP_Text>();

            //Assign the upgradesUI gameobject for easier access
            upgradesUI = parentUI.transform.Find("MainCanvas/Upgrades").gameObject;

            //Assign the pauseMenuUI gameobject for easier access
            pauseMenuUI = parentUI.transform.Find("MainCanvas/PauseMenu").gameObject;

            //Get Buttons for PauseMenu
            resumeButton = pauseMenuUI.transform.Find("ResumeButton").GetComponent<Button>();
            pauseButton = pauseMenuUI.transform.Find("PauseButton").GetComponent<Button>();
            quitButton = pauseMenuUI.transform.Find("QuitButton").GetComponent<Button>();
            menuButton = pauseMenuUI.transform.Find("MenuButton").GetComponent<Button>();
        }
        else
        {
            Destroy(gameObject); // Prevents duplicates on scene reload
        }

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Start is called before the first frame update
    void Start()
    { 
        curr_room = 0;
        ES = gameObject.GetComponent<EnemySpawning>();

        // Give self 0 coins to set HUD currency
        GainCoin(0);
        MusicManager = Instantiate(musicManagerPrefab);

        // FIXME: Setting up the Patterns List should be move to gameconstants when one exists.
        //Create lists for all of the Patterns
        List<(int, string, Action)> Len1_Patterns = new List<(int, string, Action)>();
        List<(int, string, Action)> Len2_Patterns = new List<(int, string, Action)>() {
            (11, "Pair", PF.StartSpeedBoost) };
        List<(int, string, Action)> Len3_Patterns = new List<(int, string, Action)>() {
            (121, "Sandwich", PF.DamageSweep), (111, "Three of a kind", PF.DamageSweep)
        };
        List<(int, string, Action)> Len4_Patterns = new List<(int, string, Action)>() {
            (1221, "Big Sandwich", PF.DamageSweep), (1111, "Four of a kind", PF.DamageSweep), (4321, "Rainbow", PF.DamageSweep), (2211, "Two Pair", PF.DamageSweep), (1321, "Mini Club", PF.DamageSweep)
        };
        List<(int, string, Action)> Len5_Patterns = new List<(int, string, Action)>() {
            (12121, "Big Mac", PF.DamageSweep), (11111, "Five of a kind", PF.DamageSweep), (14321, "Club Sandwich", PF.DamageSweep), (22211, "Full House", PF.DamageSweep), (12321, "Double Decker", PF.DamageSweep), (11211, "Fat Sandwich", PF.DamageSweep)
        };

        //Add all of the Patterns to the Patterns double list
        Patterns.Add(Len1_Patterns);
        Patterns.Add(Len2_Patterns);
        Patterns.Add(Len3_Patterns);
        Patterns.Add(Len4_Patterns);
        Patterns.Add(Len5_Patterns);

        resumeButton.onClick.AddListener(ResumeGame);
        pauseButton.onClick.AddListener(PauseGame);
        quitButton.onClick.AddListener(QuitGame);
        menuButton.onClick.AddListener(GoToMainMenu);

        pauseMenuUI.SetActive(false); // Ensure menu is hidden at start

        // FIXME: add to game_constants
        // Add all upgrades to the Upgrade list; move to game_constants when one exists
        // See the constructor for the Upgrade class in 'Upgrade_manager.cs' to find detailed info about parameters.
        // Only one upgrade sprite asset is finished (Precision, as of 02/28), so all others will use the default
        Upgrades.Add(new Upgrade("Precision", "Deal +[X] extra damage on every hit (currently adding [N] damage)", (float)precisionUpgradeModifierValue, (float)precisionUpgradeModifier, "", 0, 5, "Assets/Sprites/Upgrades/whetstone.png", "Assets/Sprites/Upgrades/whetstoneVert.png", "red"));
        Upgrades.Add(new Upgrade("Hardware Acceleration", "Increase dash range by [X]%", (float)hardwareAccelUpgradeModifierValue, (float)hardwareAccelUpgradeModifier, "", 0, 5, "Assets/Sprites/Upgrades/hardwareAccel.png", "Assets/Sprites/Upgrades/hardwareAccelVert.png", "purple"));
        Upgrades.Add(new Upgrade("Two Birds", "Your attacks hit twice, second attack does [X]% and also applies on-hit effects", (float)twoBirdsUpgradeModifierValue, (float)twoBirdsUpgradeModifier, "", 0, 5, "Assets/Sprites/Upgrades/twobirds.png", "Assets/Sprites/Upgrades/twobirdsVert.png", "red"));
        Upgrades.Add(new Upgrade("Fortified", "Enemy projectiles deal [X]% less damage", (float)fortifiedUpgradeModifierValue, (float)fortifiedUpgradeModifier, "", 0, 5, "Assets/Sprites/Upgrades/fortify.png", "Assets/Sprites/Upgrades/fortifyVert.png", "green"));
        Upgrades.Add(new Upgrade("Boot Up", "Gain a [X]% speed boost for the first [N] sec of each room", (float)bootUpUpgradeModifierValue, (float)bootUpUpgradeModifier, "", 0, 5, "Assets/Sprites/Upgrades/bootUp.png", "Assets/Sprites/Upgrades/bootUpVert.png", "purple"));
        Upgrades.Add(new Upgrade("Spice of Life", "Gain [X]% additional damage for each unique combo used this run", (float)spiceLifeUpgradeModifierValue, (float)spiceLifeUpgradeModifierValue, "", 0, 5, "Assets/Sprites/Upgrades/spiceOfLife.png", "Assets/Sprites/Upgrades/spiceOfLifeVert.png", "red"));
        Upgrades.Add(new Upgrade("git restore", "When entering a new non-shop room, restore [X]% of max health", (float)gitRestoreUpgradeModifierValue, (float)gitRestoreUpgradeModifier, "", 0, 5, "Assets/Sprites/Upgrades/gitRestore.png", "Assets/Sprites/Upgrades/gitRestoreVert.png", "green"));
        Upgrades.Add(new Upgrade("Bloodthirsty", "Gain [X] health upon killing [N] enemies", (float)bloodthirstyUpgradeModifierValue, (float)bloodthirstyUpgradeModifier, "", 0, 5, "Assets/Sprites/Upgrades/bloodthirsty.png", "Assets/Sprites/Upgrades/bloodthirstVert.png", "red"));
        Upgrades.Add(new Upgrade("Greedy", "Gain [X]% more gold from enemy kills", (float)greedyUpgradeModifierValue, (float)greedyUpgradeModifier, "", 0, 5, "Assets/Sprites/Upgrades/chipMagnet.png", "Assets/Sprites/Upgrades/chipMagnetVert.png", "yellow"));
        Upgrades.Add(new Upgrade("Thorns", "When you take damage, deal [X]% to the enemy that hit you", (float)thornsUpgradeModifierValue, (float)thornsUpgradeModifier, "", 0, 5, "Assets/Sprites/Upgrades/thorns.png", "Assets/Sprites/Upgrades/thornsVert.png", "green"));

        // Id should always = index in Upgrades list
        for (int i = 0; i < Upgrades.Count; i++)
        {
            Upgrades[i].Id = i;
        }

        // Sets which upgrades the player has based on the Id array
        SetPlayerHeldUpgradesFromIds();

        // Instantiate the UI icons of those held upgrades
        InstantiateIcons();
        parentUI.transform.Find("MainCanvas/Upgrades/LineBL").gameObject.SetActive(false);
        parentUI.transform.Find("MainCanvas/Upgrades/LineTL").gameObject.SetActive(false);
        parentUI.transform.Find("MainCanvas/Upgrades/LineTR").gameObject.SetActive(false);
        parentUI.transform.Find("MainCanvas/Upgrades/LineBR").gameObject.SetActive(false);
        parentUI.transform.Find("MainCanvas/Upgrades/HoverSquare").gameObject.SetActive(false);
        parentUI.transform.Find("MainCanvas/Upgrades/Label").gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)) // Press 'P' to toggle pause
        {
            TogglePause();
        }
    }

    void Dummy()
    {
        Debug.Log("Dummy key pressed");
    }

    public void GainCoin(int val) {
        Currency += val;
        T_Currency.text = Currency.ToString() + " CHIPS";
    }

    public void SetMaxHealth(float val){
        PlayerMaxHealth = val;
    }

    public void SetHealth(float val){
        PlayerHealth = val;
        healthBar.SetProgress(PlayerHealth/PlayerMaxHealth);
    }

    public void ResetPlayerHealth(){
        SetHealth(PlayerMaxHealth);
    }

    //---------------------------------Functions for Patterns----------------------------
    public static void AddPatternUI(GameObject new_pat_man){
        pat_man = new_pat_man.GetComponent<Pattern_UI_manager>();
    }

    public void UpdatePattern(int type) {
        // Adds a type to the pattern record. Should be called whenever an enemy is killed.
        // This then checks the Pattern Record to see if any Patterns have occurred.
        AddToPattern(type);
        ((int, int), (int, int)) ActivePattern = CheckPatterns();
        (int, int) success = ActivePattern.Item1;
        (int, int) subpat = ActivePattern.Item2;
        string pat = "";
        if (success.Item1 != -1)
        {
            pat = Patterns[success.Item1][success.Item2].Item2;
        }

        currentPattern = success;
        if (pat_man != null){
            pat_man.UpdatePatternName(pat);
            pat_man.UpdateQueueColors(type, subpat.Item1, subpat.Item2);
        }
    }

    void AddToPattern(int type)
    {
        Debug.Log($"Add to Pattern {type}");
        //Add the passed type to the pattern_record
        Pattern_record.Add(type);
        if (Pattern_record.Count > MAX_PATTERN_LEN) {
            Pattern_record.Remove(Pattern_record[0]);
        }
    }

    int TypeToChar(){
        return TypeToChar(Pattern_record.Count, 0);
    }
    int TypeToChar(int start, int end)
    {
        //Translates the 5 most recent slain enemy types to a 5 int number to compare with patterns
        int ret = 0;
        int counter = 1;

        Dictionary<int, int> Translations = new Dictionary<int, int>();
        //Iterate from most recent to oldest of saved types
        for (int i = start; i >= end; i--)
        {
            int t = Pattern_record[i];
            if (!Translations.ContainsKey(t))
            {
                Translations.Add(t, counter);
                counter++;
            }
            ret += (int)(Mathf.Pow(10, start-i) * Translations[t]);
        }
        return ret;
    }

    ((int, int), (int, int)) CheckPatterns(){
        return CheckPatterns(-1, Pattern_record.Count-1, 0);
    }
    ((int, int), (int, int)) CheckPatterns(int Seq_in, int start, int end)
    {
        if(Seq_in == 0){return ((-1, -1), (-1, -1)); }
        int Seq = Seq_in;
        if(Seq_in == -1){
            if (start < end){
                return ((-1, -1), (-1, -1));
            }
            Seq = TypeToChar(start, end);
        }

        int l = start-end;
        int s = Pattern_record.Count - 1;
        //Loop through all patterns of size and smaller
        for (int i = 0; i < Patterns[l].Count; i++)
        {
            if (Patterns[l][i].Item1 == Seq)
            {
                //Found Matching Pattern! Return the name
                return ((l, i), (start, end));
            }
        }
        //Go to smaller pattern if no pattern found in that list.
        //Left
        ((int, int), (int, int)) left = CheckPatterns(-1, start, end+1);
        (int, int) sub_left = left.Item1;
        //Right
        ((int, int), (int, int)) right = CheckPatterns(-1, start-1, end);
        (int, int) sub_right = right.Item1;

        if (sub_left.Item1 == sub_right.Item1){
            return sub_left.Item2 > sub_right.Item2 ? left : right;
        }
        return sub_left.Item1 > sub_right.Item1 ? left : right;
    }

    public void UsePattern(){
        //Use the current Pattern's ability
        string patternName = "";
        if(currentPattern.Item1 != -1){
            //Update the name for debugging and use ability
            patternName = Patterns[currentPattern.Item1][currentPattern.Item2].Item2;
            int l = currentPattern.Item1; //length
            int j = currentPattern.Item2; //index
            Patterns[l][j].Item3();
        }
        Debug.Log($"Using {currentPattern}: {patternName}");
        //Reset the pattern state
        currentPattern = (-1, -1);
        ClearPatternQueue();
        pat_man.ClearQueue();
    }

    void ClearPatternQueue(){
        Pattern_record.RemoveAll(c => true);
    }

    //Sets the leftmost and rightmost points of the room - used for pattern func sweeps
    public void SetLeftPoint(Transform t){
        Left_point = t;
    }
    public void SetRightPoint(Transform t){
        Right_point = t;        
    }

    // Pause menu

    public void TogglePause()
    {
        if (!isPaused)
            PauseGame();
        else
            ResumeGame();
    }

    void PauseGame()
    {
        isPaused = true;
        pauseMenuUI.SetActive(true); // Show menu
        MusicManager.GetComponent<MusicManager>().AudioSource.Pause();
        Time.timeScale = 0f; // Pause game
    }

    void ResumeGame()
    {
        isPaused = false;
        isHoveringUpgradeIcon = false;
        pauseMenuUI.SetActive(false); // Hide menu
        MusicManager.GetComponent<MusicManager>().AudioSource.UnPause();
        Time.timeScale = 1f; // Resume game
    }

    void QuitGame()
    {
        Time.timeScale = 1f; // Reset before quitting
        Application.Quit(); // Works only in a built game

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // ✅ Stops play mode in the Unity Editor
#endif
    }

    void GoToMainMenu()
    {
        Time.timeScale = 1f; // Reset before loading new scene
        isPaused = false;
        pauseMenuUI.SetActive(false); // Hide menu
        SceneManager.LoadScene("Menu Room"); // Replace with actual Main Menu scene name
    }

    // Adds upgrades to the PlayerHeldUpgrades list based on the array of upgrade Ids
    public void SetPlayerHeldUpgradesFromIds()
    {
        for (int i = 0; i < MAX_PLAYER_UPGRADES; i++)
        {
            if (PlayerHeldUpgradeIds[i] != -1)
            {
                PlayerHeldUpgrades.Add(Upgrades[PlayerHeldUpgradeIds[i]]);
                ApplyUpgradeModifiers(Upgrades[PlayerHeldUpgradeIds[i]]);
            }
        }


    }

    // Instantiates upgrade UI icons according to the PlayerHeldUpgrades list
    void InstantiateIcons()
    {
        for (int i = 0; i < PlayerHeldUpgrades.Count; i++)
        {
            GameObject uiIcon = Instantiate(upgradePrefab);
            uiIcon.name = uiIcon.name + "_" + i.ToString();
            uiIcon.GetComponent<Upgrade_manager>().upgrade = PlayerHeldUpgrades[i];
            uiIcon.GetComponent<Upgrade_manager>().upgradeIndex = i;
            uiIcon.GetComponent<Upgrade_manager>().upgrade.UIOrShopItem = "UI";
            uiIcon.GetComponent<Upgrade_manager>().CreateGameObjects();
            PlayerHeldUpgradeIcons.Add(uiIcon);
        }
    }

    // Adds an upgrade to the player's held list
    // Returns true if the upgrade successfully adds to the list
    public bool AddPlayerUpgrade(Upgrade upgrade, GameObject shop)
    {
        // If the upgrade is already held by the player, then...
        if (Array.IndexOf(PlayerHeldUpgradeIds, upgrade.Id) > -1 && upgrade.Cost <= Currency && !IsCurrentlySelectingUpgrade)
        {
            // Increment the upgrade modifier by some specified amount (i.e. "level up" the upgrade)
            ApplyUpgradeModifiers(upgrade);
            GainCoin(-1 * upgrade.Cost);
            return true;
        }
        // Otherwise, if the player's max upgrade slots would be exceeded by adding this upgrade, then...
        else if (PlayerHeldUpgrades.Count + 1 > MAX_PLAYER_UPGRADES && upgrade.Cost <= Currency && !IsCurrentlySelectingUpgrade)
        {
            // Start the coroutine prompt to select/confirm/replace a currently held upgrade
            ReplacePlayerUpgrade(upgrade, shop);
            // Return false while the above function runs its coroutine, so the upgrade doesn't yet disappear from the ShopItem.
            return false;
        }
        // Otherwise (if the upgrade is not held by the player AND the max upgrade slots would not be exceeded by adding this upgrade), then...
        else if (upgrade.Cost <= Currency && !IsCurrentlySelectingUpgrade)
        {
            // Find the first empty slot in the PlayerHeldUpgradeIds array
            int index = 0;
            for (int i = 0; i < 5; i++)
            {
                if (PlayerHeldUpgradeIds[i] == -1)
                {
                    index = i;
                    break;
                }
            }
            ApplyUpgradeModifiers(upgrade);

            // Put the upgrade in the first empty slots of the PlayerHeldUpgradeIds array and PlayerHeldUpgrades list
            PlayerHeldUpgradeIds[index] = upgrade.Id;
            PlayerHeldUpgrades.Add(upgrade);

            // UI is updated accordingly
            GameObject upgradeUIIcon = Instantiate(upgradePrefab);
            upgradeUIIcon.SetActive(false);
            upgrade.UIOrShopItem = "UI";
            upgradeUIIcon.GetComponent<Upgrade_manager>().upgrade = upgrade;
            upgradeUIIcon.GetComponent<Upgrade_manager>().upgradeIndex = index;
            upgradeUIIcon.GetComponent<Upgrade_manager>().CreateGameObjects();
            upgradeUIIcon.SetActive(true);

            // UI icon is added to the PlayerHeldUpgradeIcons list
            PlayerHeldUpgradeIcons.Add(upgradeUIIcon);
            GainCoin(-1 * upgrade.Cost);
            return true;
        }
        else { 
            // U ARE TOO BROKE TO AFFORD UPGRADE
            return false;
        }
    }
    // Functionality: Starts a coroutine defined in Player_input_manager that prompts the player to select and confirm which upgrade they want to swap
    //                Player can leave the radius if they choose not to confirm and this effectively cancels the coroutine.
    // Params:
    //  Upgrade newUpgrade: the upgrade you want to replace the one in the player list with
    //  GameObject shop: the ShopItem that the incoming upgrade is correspondent with
    void ReplacePlayerUpgrade(Upgrade newUpgrade, GameObject shop)
    {
        StartCoroutine(Player_input_manager.instance.SelectAndConfirmReplace(newUpgrade, shop));
    }

    // Functionality: Starts a coroutine defined in Player_input_manager that prompts the player to select and confirm which upgrade they want to recycle
    //                Player can leave the radius if they choose not to confirm and this effectively cancels the coroutine.
    public void RecyclePlayerUpgrade(GameObject trashcan)
    {
        StartCoroutine(Player_input_manager.instance.SelectAndConfirmRecycle(trashcan));
    }

    // Called within the coroutine SelectAndConfirmReplace in Player_input_manager (structured this way to keep all player inputs inside that script... it's a bit clunky but it works)
    // Updates all the necessary lists that track the player's currently held upgrades
    // Updates the UI with the new upgrade's icon and destroys the old upgrade's icon.
    public void SwapOutUpgrade(Upgrade newUpgrade, GameObject shop, Vector2 originalSlotPosition)
    {
        RemoveUpgradeModifiers(PlayerHeldUpgrades[CurrentlySelectedUpgradeIndex]);
        PlayerHeldUpgrades[CurrentlySelectedUpgradeIndex] = newUpgrade;
        ApplyUpgradeModifiers(newUpgrade);
        PlayerHeldUpgradeIds[CurrentlySelectedUpgradeIndex] = newUpgrade.Id;

        GameObject upgradeUIIcon = Instantiate(upgradePrefab);
        upgradeUIIcon.SetActive(false);
        newUpgrade.UIOrShopItem = "UI";
        upgradeUIIcon.GetComponent<Upgrade_manager>().upgrade = newUpgrade;
        upgradeUIIcon.GetComponent<Upgrade_manager>().upgradeIndex = CurrentlySelectedUpgradeIndex;
        upgradeUIIcon.GetComponent<Upgrade_manager>().CreateGameObjects();
        upgradeUIIcon.SetActive(true);

        Destroy(PlayerHeldUpgradeIcons[CurrentlySelectedUpgradeIndex].GetComponent<Upgrade_manager>().upgradeUIIcon);
        Destroy(PlayerHeldUpgradeIcons[CurrentlySelectedUpgradeIndex]);
        

        Debug.Log("Replacing upgrade: " + PlayerHeldUpgradeIcons[CurrentlySelectedUpgradeIndex].GetComponent<Upgrade_manager>().upgrade.Name + " (Upgrade slot index " + CurrentlySelectedUpgradeIndex + ")");
        PlayerHeldUpgradeIcons[CurrentlySelectedUpgradeIndex] = upgradeUIIcon;
        upgradeUIIcon.GetComponent<Upgrade_manager>().upgradeUIIcon.GetComponent<RectTransform>().anchoredPosition = originalSlotPosition;
        GainCoin(-1 * newUpgrade.Cost);
        shop.GetComponent<ShopItem>().destroyChildren();
    }

    // Called within the coroutine SelectAndConfirmRecycle in Player_input_manager (structured this way to keep all player inputs inside that script... it's a bit clunky but it works)
    // Updates all lists that track the player's currently held upgrades
    // Updates the UI with the new upgrade's icon and destroys the old upgrade's icon
    // Moves all UI icons up to fill the gap left by the old upgrade
    public void RemoveUpgrade(GameObject trashcan) {

        // Modifiers go poof
        RemoveUpgradeModifiers(PlayerHeldUpgrades[CurrentlySelectedUpgradeIndex]);
        Upgrade upgrade = PlayerHeldUpgrades[CurrentlySelectedUpgradeIndex];

        // Trashcan sprite is the same as the recycled upgrade color
        if (upgrade.Color == "green")
        {
            trashcan.transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite = trashcan.GetComponent<Trashcan>().trashcanGreen;
        }
        else if (upgrade.Color == "purple")
        {
            trashcan.transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite = trashcan.GetComponent<Trashcan>().trashcanPurple;
        }
        else if (upgrade.Color == "red") {
            trashcan.transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite = trashcan.GetComponent<Trashcan>().trashcanRed;
        }
        else if (upgrade.Color == "yellow")
        {
            trashcan.transform.Find("Sprite").GetComponent<SpriteRenderer>().sprite = trashcan.GetComponent<Trashcan>().trashcanYellow;
        }

        // Remove the upgrade data
        PlayerHeldUpgrades.RemoveAt(CurrentlySelectedUpgradeIndex);

        // Destroy the UI gameObject and remove it form the list
        Destroy(PlayerHeldUpgradeIcons[CurrentlySelectedUpgradeIndex].GetComponent<Upgrade_manager>().upgradeUIIcon);
        Destroy(PlayerHeldUpgradeIcons[CurrentlySelectedUpgradeIndex]);
        PlayerHeldUpgradeIcons.RemoveAt(CurrentlySelectedUpgradeIndex);

        // Move all UI icons up to fill the gap left by the old upgrade
        for (int i = CurrentlySelectedUpgradeIndex; i < PlayerHeldUpgradeIcons.Count; ++i)
        {
            PlayerHeldUpgradeIcons[i].GetComponent<Upgrade_manager>().upgradeUIIcon.GetComponent<RectTransform>().anchoredPosition += new Vector2(0, 108);
            PlayerHeldUpgradeIcons[i].GetComponent<Upgrade_manager>().upgradeIndex = i;
        }

        // Shift all Ids left by one, and set the last Id to -1
        PlayerHeldUpgradeIds[4] = -1;
        for (int i = CurrentlySelectedUpgradeIndex; i < 4; i++)
        {
            PlayerHeldUpgradeIds[i] = PlayerHeldUpgradeIds[i + 1];     
        }
        
        // Deactivate the label
        trashcan.GetComponent<Trashcan>().label.SetActive(false);
    }

    // Called when upgrades are added to the player held upgrades list, essentially applies the effects of upgrades.
    // POST-MVP: implement more effects
    void ApplyUpgradeModifiers(Upgrade upgrade) {
        switch (upgrade.Name) {
            case "Precision":
                precisionUpgradeModifier += precisionUpgradeModifierValue;
                upgrade.N = precisionUpgradeModifier;
                break;
            case "Hardware Acceleration":
                hardwareAccelUpgradeModifier += hardwareAccelUpgradeModifierValue;
                upgrade.N = hardwareAccelUpgradeModifier;
                break;
            case "Two Birds":
                twoBirdsUpgradeModifier += twoBirdsUpgradeModifierValue;
                upgrade.N = twoBirdsUpgradeModifier;
                break;
            case "Fortified":
                fortifiedUpgradeModifier += fortifiedUpgradeModifierValue;
                upgrade.N = fortifiedUpgradeModifier;
                break;
            case "Boot Up":
                bootUpUpgradeModifier += bootUpUpgradeModifierValue;
                upgrade.N = bootUpUpgradeModifier;
                break;
            case "Spice of Life":
                spiceLifeUpgradeModifier += spiceLifeUpgradeModifierValue;
                upgrade.N = spiceLifeUpgradeModifier;
                break;
            case "Git Restore":
                gitRestoreUpgradeModifier += gitRestoreUpgradeModifierValue;
                upgrade.N = gitRestoreUpgradeModifier;
                break;
            case "Bloodthirsty":
                bloodthirstyUpgradeModifier += bloodthirstyUpgradeModifierValue;
                upgrade.N = bloodthirstyUpgradeModifier;
                break;
            case "Greedy":
                greedyUpgradeModifier += greedyUpgradeModifierValue;
                upgrade.N = greedyUpgradeModifier;
                break;
            case "Thorns":
                thornsUpgradeModifier += thornsUpgradeModifierValue;
                upgrade.N = thornsUpgradeModifier;
                break;
            default:
                Debug.Log("Modifier not found for upgrade with name: \"" + upgrade.Name + "\"");
                break;
        }
        upgrade.UpdateDesc();
        if (Shop_room_setup.instance != null) 
        {
            Shop_room_setup.instance.UpdateShopItemLabel(upgrade);
        }
    }

    // Called when upgrades are removed from the player held upgrades list, essentially removes the effects of upgrades.
    // POST-MVP: implement more effects
    void RemoveUpgradeModifiers(Upgrade upgrade) {
        switch (upgrade.Name)
        {
            case "Precision":
                precisionUpgradeModifier = 0;
                upgrade.N = precisionUpgradeModifier;
                break;
            case "Hardware Acceleration":
                hardwareAccelUpgradeModifier = 0;
                upgrade.N = hardwareAccelUpgradeModifier;
                break;
            case "Two Birds":
                twoBirdsUpgradeModifier = 0;
                upgrade.N = twoBirdsUpgradeModifier;
                break;
            case "Fortified":
                fortifiedUpgradeModifier = 0;
                upgrade.N = fortifiedUpgradeModifier;
                break;
            case "Boot Up":
                bootUpUpgradeModifier = 0;
                upgrade.N = bootUpUpgradeModifier;
                break;
            case "Spice of Life":
                spiceLifeUpgradeModifier= 0;
                upgrade.N = spiceLifeUpgradeModifier;
                break;
            case "Git Restore":
                gitRestoreUpgradeModifier = 0;
                upgrade.N = gitRestoreUpgradeModifier;
                break;
            case "Bloodthirsty":
                bloodthirstyUpgradeModifier = 0;
                upgrade.N = bloodthirstyUpgradeModifier;
                break;
            case "Greedy":
                greedyUpgradeModifier = 0;
                upgrade.N = greedyUpgradeModifier;
                break;
            case "Thorns":
                thornsUpgradeModifier = 0;
                upgrade.N = thornsUpgradeModifier;
                break;
            default:
                Debug.Log("Modifier not found for upgrade with name: \"" + upgrade.Name + "\"");
                break;
        }
        upgrade.UpdateDesc();
        if (Shop_room_setup.instance != null)
        {
            Shop_room_setup.instance.UpdateShopItemLabel(upgrade);
        }
    }
    
    //Rooms
    public void IncRoom(){
        Debug.Log("NEXT ROOM!");
        curr_room++;
    }

    public void ResetRoom(){
        Debug.Log("RESET ROOM!");
        curr_room = 0;
        pat_man.ClearQueue();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode){
        Debug.Log($"In room {curr_room}");
        if(ES != null){
            ES.GenerateEnemies(curr_room);
        }
    }
}

