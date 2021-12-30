using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicFadeIn : MonoBehaviour
{
    
    public float FadeDuration = 1;
    public float StartAt = -1;
    public bool FadeIn, FadeOut;
    public float MaxVolume = 1;

    public void Start()
    {
        StartFadeIn();
    }

    public void StartFadeIn()
    {
        if (FadeIn) return;
        StartAt = Time.time;
        FadeIn = true;
        FadeOut = false;
    }

    public void StartFadeOut()
    {
        if (FadeOut) return;
        StartAt = Time.time;
        FadeOut = true;
        FadeIn = false;

    }

    void Update()
    {
        if (StartAt < 0) return;
        float percent = (Time.time - StartAt) / FadeDuration;
        if (FadeIn)
        {
            GetComponent<AudioSource>().volume = Mathf.Lerp(0, MaxVolume, percent);
        }
        else
        {
            GetComponent<AudioSource>().volume = Mathf.Lerp(MaxVolume, 0, percent);
        }

        if (percent > 1)
        {
            StartAt = -1;
        }
    }
}
