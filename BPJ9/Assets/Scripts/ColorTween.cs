using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTween : MonoBehaviour
{

    public SpriteRenderer Target;
    public float Speed;
    public Color FirstColor;
    public Color SecondColor;

    void Update()
    {
        float t = Mathf.Abs((Mathf.Sin(Time.time*Speed)+1)/2);
        Target.color = Color.Lerp(FirstColor, SecondColor, t);
    }
}
