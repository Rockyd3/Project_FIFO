using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class SpriteSelect : MonoBehaviour
{
    [SerializeField]
    SpriteRenderer SR;
    public List<Sprite> SpriteOptions = new List<Sprite>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SR = gameObject.GetComponent<SpriteRenderer>();

        if (SpriteOptions != null && SpriteOptions.Count > 0) {
            int r = (int)Random.Range(0f, (float)SpriteOptions.Count-0.01f);
            SR.sprite = SpriteOptions[r];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
