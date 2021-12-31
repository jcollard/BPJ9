using System.Collections;
using System.Collections.Generic;
using CaptainCoder.Unity;
using UnityEngine;

public class SoundController : MonoBehaviour
{
    public static SoundController Instance;
    public UnityEngine.Audio.AudioMixerGroup Group;
    public SFX[] Sounds;
    private Dictionary<string, SFX> lookup;
    public Dictionary<string, AudioSource> channels;

    public void Awake()
    {
        Init();
    }

    public static void PlaySFX(string name)
    {
        if (Instance == null) return;
        Instance.Play(name);
    }

    public static void PlayRandomSFX(params string[] name)
    {
        if (Instance == null) return;
        Instance.Play(name[Random.Range(0, name.Length)]);
    }

    public void Play(string name)
    {
        if (!this.channels.TryGetValue(name, out AudioSource channel))
        {
            if (!this.lookup.TryGetValue(name, out SFX sound)) UnityEngineUtils.Instance.FailFast($"No such sound {sound.Name}.", this.gameObject);
            GameObject container = new GameObject($"SFX: {sound.Name}");
            container.transform.parent = this.transform;
            channel = container.AddComponent<AudioSource>();
            channel.playOnAwake = false;
            channel.volume = sound.Volume;
            channel.outputAudioMixerGroup = this.Group;
            channel.clip = sound.AudioClip;
            this.channels[name] = channel;
        }
        channel.Play();
    }


    public void Init()
    {
        Instance = this;
        lookup = new Dictionary<string, SFX>();
        channels = new Dictionary<string, AudioSource>();
        foreach (SFX sfx in Sounds)
        {
            if (lookup.ContainsKey(sfx.Name)) UnityEngineUtils.Instance.FailFast($"Duplicate SFX name detected, {sfx.Name}.", this.gameObject);
            lookup[sfx.Name] = sfx;
        }
    }

}

[System.Serializable]
public class SFX
{
    public string Name;
    public AudioClip AudioClip;
    public float Volume = 1f;
}
