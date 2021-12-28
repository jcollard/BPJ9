using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WakeUp : MonoBehaviour
{
    public BatController Bat;

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        PlayerCollider pc = other.GetComponent<PlayerCollider>();
        if (pc == null) return;
        this.OnTrigger();
    }

    public virtual void OnTriggerStay2D(Collider2D other)
    {
        PlayerCollider pc = other.GetComponent<PlayerCollider>();
        if (pc == null) return;
        this.OnTrigger();
    }

    public virtual void OnTriggerExit2D(Collider2D other)
    {
        PlayerCollider pc = other.GetComponent<PlayerCollider>();
        if (pc == null) return;
        Bat.FindPlayer = true;
        Bat.Speed = Bat.StartSpeed;
    }

    private void OnTrigger() => Bat.WakeUp();
}
