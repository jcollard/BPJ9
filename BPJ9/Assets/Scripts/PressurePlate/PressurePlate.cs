using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressurePlate : MonoBehaviour
{
    public PressureTrigger Triggering;
    private HashSet<PressureTrigger> Triggers = new HashSet<PressureTrigger>();
    public List<System.Action<PressurePlate>> OnTrigger = new List<System.Action<PressurePlate>>();
    public List<System.Action<PressurePlate>> OnUntrigger = new List<System.Action<PressurePlate>>();
    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        PressureTrigger pt = other.GetComponent<PressureTrigger>();
        if (pt == null) return;
        Triggering = pt;
        if (Triggers.Count == 0)
        {
            foreach (System.Action<PressurePlate> action in OnTrigger)
                action(this);
            SoundController.PlaySFX("Click 0");
        }
        Triggers.Add(pt);
    }

    public virtual void OnTriggerExit2D(Collider2D other)
    {
        PressureTrigger pt = other.GetComponent<PressureTrigger>();
        if (pt == null) return;
        if (pt != Triggering) return;
        Triggers.Remove(pt);
        if (Triggers.Count == 0)
        {
            foreach (System.Action<PressurePlate> action in OnUntrigger)
                action(this);

            Triggering = null;
            SoundController.PlaySFX("Click 1");
        }
    }
}
