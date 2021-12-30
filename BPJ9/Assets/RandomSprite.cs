using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class RandomSprite : MonoBehaviour
{
    public Sprite[] Options;
    // Start is called before the first frame update
    void Start()
    {
        if (Options.Length > 0)
            this.GetComponent<SpriteRenderer>().sprite = Options[Random.Range(0, Options.Length)];
    }
}
