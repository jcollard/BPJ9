using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetTrigger : MonoBehaviour
{
    public Resetable ToReset;

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        PlayerCollider pt = other.GetComponent<PlayerCollider>();
        if (pt == null) return;
        ToReset.Reset();
    }
}
