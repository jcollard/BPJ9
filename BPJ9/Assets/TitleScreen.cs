using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreen : MonoBehaviour
{
    public GameObject CreditScreen;
    public float FadeAt = -1;
    public float FadeDuration = 3;

    public void Update()
    {
        if (FadeAt < 0) return;
        float percent = (Time.time - FadeAt)/FadeDuration;
        GetComponent<CanvasGroup>().alpha = Mathf.Lerp(1, 0, percent);
    }
    public void StartGame()
    {
        if (FadeAt < 0) FadeAt = Time.time;
    }

    public void Credits()
    {
        CreditScreen.SetActive(!CreditScreen.activeInHierarchy);
    }
}
