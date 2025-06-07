using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public class EnemySpawning : MonoBehaviour
{    
    public List<GameObject> EnemyRanks = new List<GameObject>();

    public List<GameObject> allSpawners = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GenerateEnemies(Level_manager.instance.curr_room);
    }

    public void GenerateEnemies(int room){
        allSpawners.Clear();
        if (room == 0 || (room % 2 == 0)){
            return;
        }
        // Get all Respawn points
        Respawn_Point[] allObjects = (Respawn_Point[])FindObjectsByType<Respawn_Point>(FindObjectsSortMode.None);
        foreach (Respawn_Point RP in allObjects)
        {
            if(RP.name != "Respawn_point"){
                allSpawners.Add(RP.gameObject);
            }
        }
        SpawnEnemies(room);
    }

    void SpawnEnemies(int room){
        List<int> _rank = new List<int>(); //Holds indexes of ranks
        //Setup the list of how many of each rank is needed
        int effective_room = (int)(room + 1)/2;
        List<int> Amounts = GetEnemyAmounts(effective_room);
        for(int r=0; r < EnemyRanks.Count; r++){
            _rank.AddRange(Enumerable.Repeat(r, Amounts[r]));
        }
        if (Amounts.Count != EnemyRanks.Count){
            Debug.LogWarning("Mismatch in number of Enemy Ranks and number of provided Enemy Amounts!");
        }
        else{
            for(int i=0; i < _rank.Count; i++){
                //Select random position for enemy of rank i
                int p_rand = (int)Random.Range(0f, (float)allSpawners.Count-1);
                int r = _rank[i];
                int element = GetElement();
                //Spawn enemy of rank r at position p_rand
                allSpawners[p_rand].GetComponent<Respawn_Point>().SetSpawnee(EnemyRanks[r], true, true, element);
                Debug.Log($"Set {EnemyRanks[r].name} to spawn at position {p_rand}");
                allSpawners.RemoveAt(p_rand);
            }
        }
    }

    int GetElement(){
        // Returns an integer representing the element of the enemy.
        // This is a separate function to allow for more robust element spawning logic in the future.
        return (int)Random.Range(0f, 3f);
    }

    List<int> GetEnemyAmounts(int n){
        int type = (int)Random.Range(0f, 2f);
        List<int> ret = new List<int>();
        switch(type){
            case 0:
                ret.Add(n);
                ret.Add(0);
                return ret;
            case 1:
                ret.Add(0);
                ret.Add(n);
                return ret;
            case 2:
                ret.Add(n/2);
                ret.Add(n/2);
                return ret;
        }
        return new List<int>() {0, 0};
    }
}
