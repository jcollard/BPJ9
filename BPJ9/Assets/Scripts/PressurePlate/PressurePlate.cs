using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public PressureTrigger Triggering;
    public List<System.Action<PressurePlate>> OnTrigger = new List<System.Action<PressurePlate>>();
    public List<System.Action<PressurePlate>> OnUntrigger = new List<System.Action<PressurePlate>>();
    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        PressureTrigger pt = other.GetComponent<PressureTrigger>();
        if (pt == null) return;
        Triggering = pt;
        foreach (System.Action<PressurePlate> action in OnTrigger)
            action(this);
        SoundController.PlaySFX("Click 0");
    }

    public virtual void OnTriggerExit2D(Collider2D other)
    {
        PressureTrigger pt = other.GetComponent<PressureTrigger>();
        if (pt == null) return;
        if (pt != Triggering) return;
        Triggering = null;
        foreach (System.Action<PressurePlate> action in OnUntrigger)
            action(this);
        SoundController.PlaySFX("Click 1");
    }
}
