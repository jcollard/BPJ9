using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HurtOnTouch : MonoBehaviour
{
    public float Damage;
    public float Knockback = 2;

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        PlayerCollider pc = other.GetComponent<PlayerCollider>();
        if (pc == null) return;
        if (pc.Player.DamageBoostStartAt > 0) return;
        pc.Player.TakeHit(this.gameObject, this.Damage, this.Knockback);
    }

}
