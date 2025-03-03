using UnityEngine;
using System;
using System.Collections.Generic;

public class Shop_room_setup : MonoBehaviour
{
    [Header("Shop")]
    // The ShopItem prefab
    [SerializeField]
    private GameObject ShopItemPrefab;
    [SerializeField]
    private GameObject ShopItemUpgradePrefab;
    // How many ShopItems in the room
    [SerializeField]
    private int numShopItems;
    // How many small floor squares in between each ShopItem
    [SerializeField]
    private int numSquaresBetweenEachShopItem;
    // How many squares from the empty wall of the room the first ShopItem should be placed
    [SerializeField]
    private int numSquaresFirstShopItemFromEdge;
    // Which wall to line the ShopItems up on
    [SerializeField]
    private string wallDirection;

    // List of the 3 upgrades this shop room will display
    List<Upgrade> shopItemUpgrades = new List<Upgrade>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateUpgrades();
        
        for (int i = 0; i < numShopItems; i++)
        {
            GameObject shopItem;
            // Context:
            // Vector3(0.371f, 0.288f, 7.126f) represents the top right corner of the large floor square.
            // The ShopItemPrefab position is at the bottom left corner of the large floor square.
            // 0.375f is the width of a single, small floor tile
            // 1.5f is to account for the fact that the position is calculated from the center of each ShopItem (so the expected "distance between" is effectively shortened by 2/3)
            if (wallDirection.Equals("Northwest"))
            {
                shopItem = Instantiate(ShopItemPrefab, new Vector3(0.371f, 0.288f, 7.126f) + new Vector3(0, 0, -0.375f * 1.5f * i * numSquaresBetweenEachShopItem + -0.375f * numSquaresFirstShopItemFromEdge), Quaternion.identity);
            }
            else
            {
                shopItem = Instantiate(ShopItemPrefab, ShopItemPrefab.transform.position + new Vector3(-0.375f * 1.5f * i * numSquaresBetweenEachShopItem + -0.375f * numSquaresFirstShopItemFromEdge, 0, 0), Quaternion.identity);
            }
            Renderer cubeRenderer = shopItem.GetComponent<MeshRenderer>();
            Vector3 center = cubeRenderer.bounds.center;
            float height = cubeRenderer.bounds.extents.y;

            // 0.12f is a good height above the Shop to make the upgrade icon appear more 3D
            GameObject shopItemUpgrade = Instantiate(ShopItemUpgradePrefab, center + new Vector3(0, height + 0.12f, 0), Quaternion.identity);

            // Append a number to the name of the gameObject
            shopItemUpgrade.name = shopItemUpgrade.name + "_" + i.ToString();

            // Give the label and upgrade each a reference to the appropriate Upgrade object.
            shopItemUpgrade.GetComponent<Upgrade_manager>().upgrade = shopItemUpgrades[i];
            shopItem.GetComponent<Shop_interaction_manager>().upgrade = shopItemUpgrades[i];

            // Tell the Upgrade_manager that we want to make a ShopItem (rather than a UI icon)
            shopItemUpgrade.GetComponent<Upgrade_manager>().upgrade.UIOrShopItem = "ShopItem";
            shopItemUpgrade.GetComponent<Upgrade_manager>().shopItem = shopItem.transform;
            shopItemUpgrade.GetComponent<Upgrade_manager>().wallDirection = wallDirection;
            shopItemUpgrade.GetComponent<Upgrade_manager>().CreateGameObjects();

            // Append a number to the name of the gameObject
            shopItem.name = shopItem.name + "_" + i.ToString();
        }
    }

    // Generate a list of 3 distinct, random upgrades to place on the shops
    void GenerateUpgrades()
    {
        int[] generatedIds = new int[numShopItems];

        // Make sure that none of the initial elements are 0s
        for (int i = 0; i < generatedIds.Length; i++)
        {
            generatedIds[i] = -1;
        }
        int id = -1;
        for (int i = 0; i < numShopItems; i++)
        {
            // If there aren't enough upgrades loaded in the Upgrades list in the Level manager to create 3 random, distinct ShopItems, then do not attempt to randomize them
            if (Level_manager.instance.Upgrades.Count > 3)
            {
                // If the randomly generated id has already been generated, keep trying
                do
                {
                    id = UnityEngine.Random.Range(0, Level_manager.instance.Upgrades.Count);
                } while (Array.IndexOf(generatedIds, id) != -1);
                generatedIds[i] = id;
                id = -1;
            }
            else
            {
                generatedIds[i] = i;
            }
        }
        // Add the upgrades corresponding to the selected Ids to the shopItemUpgrades list
        for (int i = 0; i < generatedIds.Length; i++)
        {
            shopItemUpgrades.Add(Level_manager.instance.Upgrades[generatedIds[i]]);
        }
    }
}

