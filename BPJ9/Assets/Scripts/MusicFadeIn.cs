using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicFadeIn : MonoBehaviour
{
    public static MusicFadeIn Instance;
    public float FadeDuration = 1;
    public float StartAt = -1;
    public bool FadeIn, FadeOut;
    public float MaxVolume = 1;
    public AudioClip Wandering;
    public AudioClip BossMusic;
    public AudioClip WinMusic;
    public AudioClip GameOverMusic;

    public AudioClip Next;

    public void Start()
    {
        Instance = this;
        StartFadeIn();
    }

    public void SelectTrack(AudioClip next)
    {
        this.Next = next;
        FadeIn = false;
        FadeOut = false;
    }

    public void StartFadeIn()
    {
        if (FadeIn) return;
        if (this.Next != null) this.GetComponent<AudioSource>().clip = this.Next;
        this.GetComponent<AudioSource>().Play();
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
