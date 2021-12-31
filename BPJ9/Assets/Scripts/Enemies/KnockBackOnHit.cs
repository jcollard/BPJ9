using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnockBackOnHit : MonoBehaviour
{    
    public float Knockback = 6;
    public bool IsEnabled = true;

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        WeaponController wc = other.GetComponent<WeaponController>();
        if (!IsEnabled || wc == null) return;
        wc.Player.Knockback(this.gameObject, this.Knockback);
    }

}
