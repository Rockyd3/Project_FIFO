using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Pattern_UI_manager : MonoBehaviour
{
    [SerializeField]
    public List<Image> Queue = new List<Image>();
    [SerializeField]
    public TMP_Text t_pattern;

    Color transparent;

    public List<Sprite> S_Unused = new List<Sprite>();
    public List<Sprite> S_Used = new List<Sprite>();
    public Sprite S_Empty;
    [SerializeField]
    Vector2 empty_size;
    [SerializeField]
    Vector2 sprite_size;
    [SerializeField]
    float middle_diff;
    float empty_y;
    float full_y;
    private List<RectTransform> RT = new List<RectTransform>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        int i = 0;
        foreach (Image img in Queue){
            RT.Add(img.gameObject.GetComponent<RectTransform>());
            i++;
        }
        
        transparent = new Color(0f, 0f, 0f, 0f);

        full_y = RT[0].anchoredPosition.y;
        empty_y = full_y + middle_diff;
        
        ClearQueue();

        Level_manager.AddPatternUI(gameObject);
    }

    public void UpdateQueueColors(int new_type, int min_used, int max_used){
        Debug.Log($"Updating Pattern UI {min_used} to {max_used}");
        for(int i=Level_manager.instance.Pattern_record.Count-1; i>=0; i--){
            bool used = (min_used >= i && i >= max_used);
            SetType(i, Level_manager.instance.Pattern_record[i], used);
        }
    }

    public void UpdatePatternName(string name){
        t_pattern.text = name;
    }

    public void ClearQueue(){
        for(int i =0; i < Queue.Count; i++){
            SetEmpty(i);
        }
        t_pattern.text = "";
    }

    void SetType(int img_ix, int element_ix, bool use){
        Queue[img_ix].sprite = use? S_Used[element_ix] : S_Unused[element_ix];
        RT[img_ix].sizeDelta = sprite_size;
        RT[img_ix].anchoredPosition = SetPos(img_ix, full_y);
    }

    void SetEmpty(int img_ix){
        Queue[img_ix].sprite = S_Empty;
        RT[img_ix].sizeDelta = empty_size;
        RT[img_ix].anchoredPosition = SetPos(img_ix, empty_y);
    }

    Vector2 SetPos(int ix, float y){
        return new Vector2(RT[ix].anchoredPosition.x, y);
    }

}
