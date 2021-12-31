using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroFade : MonoBehaviour
{
 public float FadeAt = -1; 
    public float FadeDuration = 3;

    public CanvasGroup CG;

    // Update is called once per frame
    void Update()
    {
        if (FadeAt < 0 && Input.GetButton("Up")) DoFadeOut();
        if (FadeAt < 0) return;
        float percent = (Time.time - FadeAt) / FadeDuration;
        float alpha = Mathf.Lerp(1, 0, percent);
        CG.alpha = alpha;
        if (percent >= 1){
            FadeAt = -1;
            this.gameObject.SetActive(false);
        } 
    }

    void DoFadeOut()
    {
        this.FadeAt = Time.time;
        PlayerController.Instance.IsPlaying = true;
    }
}
