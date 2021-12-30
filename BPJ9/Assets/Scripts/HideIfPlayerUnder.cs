using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideIfPlayerUnder : MonoBehaviour
{

    public List<UnityEngine.UI.Image> Renderers;
    public float Fade = .2f;
    public float StartFade;
    public bool FadeIn;
    public bool FadeOut;
    public float FadeDuration = 1f;
    public float FadeStartAt = -1;

    public void Update()
    {
        if (FadeStartAt < 0) return;
        float percent = (Time.time - FadeStartAt) / FadeDuration;
        if (FadeIn)
        {
            SetFade(Mathf.Lerp(Fade, 1, percent));
        }
        else if (FadeOut)
        {
            SetFade(Mathf.Lerp(1, Fade, percent));
        }

        if (percent > 1)
        {
            FadeStartAt = -1;   
        }

    }

    private void SetFade(float fade)
    {
        foreach(UnityEngine.UI.Image renderer in Renderers)
        {
            Color c = renderer.color;
            c.a = fade;
            renderer.color = c;
        }
    }

    public void StartFadeOut()
    {
        if (FadeOut) return;
        FadeIn = false;
        FadeOut = true;
        FadeStartAt = Time.time;
    }

    public void StartFadeIn()
    {
        if (FadeIn) return;
        FadeIn = true;
        FadeOut = false;
        FadeStartAt = Time.time;
    }
}
