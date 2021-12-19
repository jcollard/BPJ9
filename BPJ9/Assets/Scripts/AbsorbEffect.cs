using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbsorbEffect : MonoBehaviour
{
    public Color StartColor;

    public void OnEnable()
    {
        ParticleSystem.MainModule main = this.GetComponent<ParticleSystem>().main;
        main.startColor = StartColor;
    }

}
