using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level_manager : MonoBehaviour
{
    public static Level_manager instance;

    //FIXME: Add this list to a game_constants file
    List<string> types = new List<string>() { "Earth", "Fire", "Ice", "Wind" };
    int Max_pattern_len = 3;

    List<string> Pattern_record = new List<string>();

    //FIXME: Add this to a game_constants file
    List<List<(int, string)>> Patterns = new List<List<(int, string)>>();

    private void Awake() //Makes levelmanager callable in any script: Level_manager.instance.[]
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
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
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Dummy"))
        {
            Dummy();
        }

        // Key inputs for testing patterns- feel free to delete/ignore
        if (Input.GetKeyDown("1"))
        {
            UpdatePattern("Earth");
        }
        if (Input.GetKeyDown("2"))
        {
            UpdatePattern("Fire");
        }
        if (Input.GetKeyDown("3"))
        {
            UpdatePattern("Ice");
        }
        if (Input.GetKeyDown("4"))
        {
            UpdatePattern("Wind");
        }
    }

    void Dummy()
    {
        Debug.Log("Dummy key pressed");
    }

    void UpdatePattern(string type)
    {
        // Adds a type to the pattern record. Should be called whenever an enemy is killed.
        // This then checks the Pattern Record to see if any Patterns have occurred.
        AddToPattern(type);
        int Cur_Pattern = TypeToChar();
        print(Cur_Pattern);
        string success = CheckPatterns(Cur_Pattern);
        if (success != null)
        {
            print(success);
        }
    }

    void AddToPattern(string type)
    {
        //Add the passed type to the pattern_record
        Pattern_record.Add(type);
        if (Pattern_record.Count > Max_pattern_len)
        {
            Pattern_record.Remove(Pattern_record[0]);
        }
        int temp = TypeToChar();
    }

    int TypeToChar()
    {
        //Translates the 5 most recent slain enemy types to a 5 int number to compare with patterns
        int ret = 0;
        int counter = 1;

        int c = Pattern_record.Count;
        Dictionary<string, int> Translations = new Dictionary<string, int>();
        //Iterate from most recent to oldest of saved types
        for (int i = c - 1; i >= 0; i--)
        {
            string t = Pattern_record[i];
            if (!Translations.ContainsKey(t))
            {
                Translations.Add(t, counter);
                counter++;
            }
            ret += (int)(Mathf.Pow(10, i) * Translations[t]);
        }
        return ret;
    }

    string CheckPatterns(int Seq)
    {
        int s = Pattern_record.Count - 1;
        //Loop through all patterns of size and smaller
        for (int l = s; l >= 0; l--)
        { //Loop through pattern sizes
            for (int i = 0; i < Patterns[l].Count; i++)
            {
                if (Patterns[l][i].Item1 == Seq)
                {
                    //Found Matching Pattern! Return the name
                    return Patterns[l][i].Item2;
                }
            }
            //Go to smaller pattern if no pattern found in that list.
            Seq = (int)(Seq / 10);
        }

        return null;
    }
}
