using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn_Point : MonoBehaviour
{
    [SerializeField]
    private GameObject SpawneePrefab;
    [SerializeField]
    private GameObject ShopItemPrefab;

    private GameObject spawnee;

    // Start is called before the first frame update
    void Start()
    {
        Spawn();
    }

    void Spawn()
    {
        spawnee = Instantiate(SpawneePrefab, transform.position, Quaternion.identity);
        Instantiate(ShopItemPrefab, new Vector3(5.85f, 0.4f, 0.6f), Quaternion.identity);
    }
}
