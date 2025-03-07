using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class Level_manager : MonoBehaviour
{
    public static Level_manager instance;

    // Most upgrades the player can have at once
    public const int MAX_PLAYER_UPGRADES = 5;

    // The longest an element pattern can be
    public const int MAX_PATTERN_LEN = 3;

    [Header("UI")]
    [SerializeField] private GameObject mainUIPrefab;
    [SerializeField] private GameObject upgradePrefab;

    //FIXME: Add this list to a game_constants file
    [System.NonSerialized]
    public List<string> types = new List<string>() {"Earth", "Fire", "Ice", "Wind"};

    //FIXME: Add this to a game_constants file
    [System.NonSerialized]
    public List<List<(int, string)>> Patterns = new List<List<(int, string)>>();

    //FIXME: Add to game_constants
    [System.NonSerialized] 
    public List<Upgrade> Upgrades = new List<Upgrade>();

    [System.NonSerialized]
    public List<string> Pattern_record = new List<string>();

    // The upgrades the player currently has
    [System.NonSerialized] 
    public List<Upgrade> PlayerHeldUpgrades = new List<Upgrade>();

    // The UI icon gameObjects for the upgrades that the player currently has
    [System.NonSerialized]
    public List<GameObject> PlayerHeldUpgradeIcons = new List<GameObject>();

    // The upgrade the player clicks on when replacing/selling
    [System.NonSerialized]
    public int CurrentlySelectedUpgradeIndex = 0;

    [Header("Upgrades")]
    public int[] PlayerHeldUpgradeIds = new int[MAX_PLAYER_UPGRADES];
    

    private void Awake() //Makes levelmanager callable in any script: Level_manager.instance.[]
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GameObject mainUI = Instantiate(mainUIPrefab);
        mainUI.name = "UI";
        // FIXME: Setting up the Patterns List should be move to gameconstants when one exists.
        //Create lists for all of the Patterns
        List<(int, string)> Temp_1 = new List<(int, string)>();
        List<(int, string)> Temp_2 = new List<(int, string)>() {
            (11, "Pair")
        };
        List<(int, string)> Temp_3 = new List<(int, string)>() {
            (121, "Sandwich"), (111, "Three of a kind")
        };

        //Add all of the Patterns to the Patterns double list
        Patterns.Add(Temp_1);
        Patterns.Add(Temp_2);
        Patterns.Add(Temp_3);

        // Add all upgrades to the Upgrade list; move to game_constants when one exists
        // See the constructor for the Upgrade class in 'Upgrade_manager.cs' to find detailed info about parameters.
        // Only one upgrade sprite asset is finished (Precision, as of 02/28), so all others will use the default
        Upgrades.Add(new Upgrade("Precision", "Deal [X] extra damage on every hit", 10.0f, 0.0f, "", 0, 5, "Assets/Sprites/Upgrades/dmgUpgrade.png"));
        Upgrades.Add(new Upgrade("Hardware Acceleration", "Increase dash range by [X]%", 10.0f, 0.0f, "", 0, 5, "Assets/Sprites/Upgrades/upgradeBlank.png"));
        Upgrades.Add(new Upgrade("Two Birds", "Your attacks hit twice, second attack does [X]% and also applies on-hit effects", 10.0f, 0.0f, "", 0, 5, "Assets/Sprites/Upgrades/upgradeBlank.png"));
        Upgrades.Add(new Upgrade("Fortified", "Enemy projectiles deal [X]% less damage", 10.0f, 0.0f, "", 0, 5, "Assets/Sprites/Upgrades/upgradeBlank.png"));
        Upgrades.Add(new Upgrade("Boot Up", "Gain a [X]% speed boost for the first [N] sec of each room", 10.0f, 15.0f, "", 0, 5, "Assets/Sprites/Upgrades/upgradeBlank.png"));
        Upgrades.Add(new Upgrade("Spice of Life", "Gain [X]% additional damage for each unique combo used this run", 1.0f, 0.0f, "", 0, 5, "Assets/Sprites/Upgrades/upgradeBlank.png"));
        Upgrades.Add(new Upgrade("git restore", "When entering a new non-shop room, restore [X]% of max health", 10.0f, 0.0f, "", 0, 5, "Assets/Sprites/Upgrades/upgradeBlank.png"));
        Upgrades.Add(new Upgrade("Bloodthirsty", "Gain [X] health upon killing [N] enemies", 1.0f, 5.0f, "", 0, 5, "Assets/Sprites/Upgrades/upgradeBlank.png"));
        Upgrades.Add(new Upgrade("Greedy", "Gain [X]% more gold from enemy kills", 5.0f, 0.0f, "", 0, 5, "Assets/Sprites/Upgrades/upgradeBlank.png"));
        Upgrades.Add(new Upgrade("Thorns", "When you take damage, deal [X]% to the enemy that hit you", 10.0f, 0.0f, "", 0, 5, "Assets/Sprites/Upgrades/upgradeBlank.png"));

        // Id should always = index in Upgrades list
        for (int i = 0; i < Upgrades.Count; i++)
        {
            Upgrades[i].Id = i;
        }

        // Sets which upgrades the player has based on the Id array
        setPlayerHeldUpgradesFromIds();

        // Instantiate the UI icons of those held upgrades
        InstantiateIcons();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetButtonDown("Dummy")){
            Dummy();
        }

        // Key inputs for testing patterns- feel free to delete/ignore
        if(Input.GetKeyDown("1")){
            UpdatePattern("Earth");
        }
        if(Input.GetKeyDown("2")){
            UpdatePattern("Fire");
        }
        if(Input.GetKeyDown("3")){
            UpdatePattern("Ice");
        }
        if(Input.GetKeyDown("4")){
            UpdatePattern("Wind");
        }
    }

    void Dummy(){
        Debug.Log("Dummy key pressed");
    }

    void UpdatePattern(string type){
        // Adds a type to the pattern record. Should be called whenever an enemy is killed.
        // This then checks the Pattern Record to see if any Patterns have occurred.
        AddToPattern(type);
        int Cur_Pattern = TypeToChar();
        print(Cur_Pattern);
        string success = CheckPatterns(Cur_Pattern);
        if (success != null){
            Debug.Log(success);
        }
    }

    void AddToPattern(string type){
        //Add the passed type to the pattern_record
        Pattern_record.Add(type);
        if(Pattern_record.Count > MAX_PATTERN_LEN){
            Pattern_record.Remove(Pattern_record[0]);
        }
        //int temp = TypeToChar();
    }

    int TypeToChar(){
        //Translates the 5 most recent slain enemy types to a 5 int number to compare with patterns
        int ret = 0;
        int counter = 1;

        int c = Pattern_record.Count;
        Dictionary <string, int> Translations = new Dictionary<string, int>();
        //Iterate from most recent to oldest of saved types
        for (int i = c-1; i >= 0; i--){
            string t = Pattern_record[i];
            if(!Translations.ContainsKey(t)){
                Translations.Add(t, counter);
                counter++;
            }
            ret += (int)(Mathf.Pow(10, i) * Translations[t]);
        }
        Debug.Log(ret);
        return ret;
    }

    string CheckPatterns(int Seq){
        int s = Pattern_record.Count -1;
        //Loop through all patterns of size and smaller
        for (int l=s; l>=0; l--){ //Loop through pattern sizes
            for (int i=0; i < Patterns[l].Count; i++) {
                if (Patterns[l][i].Item1 == Seq){
                    //Found Matching Pattern! Return the name
                    return Patterns[l][i].Item2;
                }
            }
            //Go to smaller pattern if no pattern found in that list.
            Seq = (int)(Seq/10);
        }

        return null;
    }

    // Adds upgrades to the PlayerHeldUpgrades list based on the array of upgrade Ids
    void setPlayerHeldUpgradesFromIds()
    {
        for (int i = 0; i < MAX_PLAYER_UPGRADES; i++)
        {
            if (PlayerHeldUpgradeIds[i] != -1)
            {
                PlayerHeldUpgrades.Add(Upgrades[PlayerHeldUpgradeIds[i]]);
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
        if (Array.IndexOf(PlayerHeldUpgradeIds, upgrade.Id) > -1)
        {
            // POST MVP FIXME: The player already holds this upgrade, so it won't be added to a new slot.
            // For now it doesn't do anything when you try to add an upgrade the player already has (i.e. it lets the add attempt go through without actually adding anything)
            // But we potentially want it to level up the stats provided by the upgrade, so you have some long-term scaling options
            Debug.Log("Add suceeded, duplicate upgrade");
            return true;
        }
        // Otherwise, if the player's max upgrade slots would be exceeded by adding this upgrade, then...
        else if (PlayerHeldUpgrades.Count + 1 > MAX_PLAYER_UPGRADES)
        {
            // Start the coroutine prompt to select/confirm/replace a currently held upgrade
            ReplacePlayerUpgrade(upgrade, shop);
            // Return false while the above function runs its coroutine, so the upgrade doesn't yet disappear from the ShopItem.
            return false;
        }
        // Otherwise (if the upgrade is not held by the player AND the max upgrade slots would not be exceeded by adding this upgrade), then...
        else 
        {
            // Upgrade addition succeeds and adds to the player upgrade list.
            PlayerHeldUpgrades.Add(upgrade);
            PlayerHeldUpgradeIds[PlayerHeldUpgrades.Count - 1] = upgrade.Id;
            // UI is updated accordingly
            GameObject upgradeUIIcon = Instantiate(upgradePrefab);
            upgradeUIIcon.SetActive(false);
            upgrade.UIOrShopItem = "UI";
            upgradeUIIcon.GetComponent<Upgrade_manager>().upgrade = upgrade;
            upgradeUIIcon.GetComponent<Upgrade_manager>().upgradeIndex = PlayerHeldUpgrades.Count - 1;
            upgradeUIIcon.GetComponent<Upgrade_manager>().CreateGameObjects();
            upgradeUIIcon.SetActive(true);
            PlayerHeldUpgradeIcons.Add(upgradeUIIcon);
            Debug.Log("Add suceeded");

            return true;
        }
    }
    // Functionality: Starts a coroutine defined in Player_input_manager that prompts the player to select and confirm which upgrade they want to swap
    //                Player can leave the radius if they choose not to confirm and this effectively cancels the coroutine.
    // TODO: call some "sell upgrade" function to return some of the purchase cost of the replaced upgrade to the player?
    // Params:
    //  Upgrade newUpgrade: the upgrade you want to replace the one in the player list with
    //  GameObject shop: the ShopItem that the incoming upgrade is correspondent with
    void ReplacePlayerUpgrade(Upgrade newUpgrade, GameObject shop)
    {
        StartCoroutine(Player_input_manager.instance.SelectAndConfirmReplace(newUpgrade, shop));
    }

    // Called within the coroutine in Player_input_manager (structured this way to keep all player inputs inside that script... it's a bit clunky but it works
    // Updates all the necessary lists that track the player's currently held upgrades
    // Updates the UI with the new upgrade's icon and destroys the old upgrade's icon.
    public void swapOutUpgrade(Upgrade newUpgrade, GameObject shop)
    {
        PlayerHeldUpgrades[CurrentlySelectedUpgradeIndex] = newUpgrade;
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

        shop.GetComponent<Shop_interaction_manager>().destroyChildren();
    }
}