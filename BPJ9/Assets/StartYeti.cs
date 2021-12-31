using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MessageTrigger))]
public class StartYeti : MonoBehaviour
{
    public YetiController Yeti;
    // Start is called before the first frame update
    void Start()
    {
        this.GetComponent<MessageTrigger>().OnFinish = DoYeti;
    }

    // Update is called once per frame
    public void DoYeti(MessageTrigger t)
    {
        MusicFadeIn.Instance.Next = MusicFadeIn.Instance.BossMusic;
        MusicFadeIn.Instance.FadeDuration = 3;
        MusicFadeIn.Instance.StartFadeIn();
        SoundController.PlaySFX("Yeti Cry");
        Yeti.Started = true;
    }
}
