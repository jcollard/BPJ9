using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameWonFadeIn : MonoBehaviour
{
    
    public float FadeAt = -1; 
    public float FadeDuration = 3;

    public CanvasGroup CG;

    // Update is called once per frame
    void Update()
    {
        if (FadeAt < 0) return;
        float percent = (Time.time - FadeAt) / FadeDuration;
        float alpha = Mathf.Lerp(0, 1, percent);
        CG.alpha = alpha;
        if (percent >= 1) FadeAt = -1;
    }

    void OnEnable()
    {
        this.FadeAt = Time.time;
    }
}
