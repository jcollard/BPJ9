using UnityEngine;

[RequireComponent(typeof(PressurePlate))]
public class OnTriggerSound : MonoBehaviour
{
    public string sound;

    public void Start()
    {
        GetComponent<PressurePlate>().OnTrigger.Add((_) => SoundController.PlaySFX(sound));
    }
}