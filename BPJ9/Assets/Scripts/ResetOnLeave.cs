using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetOnLeave : MonoBehaviour
{
    public Resetable ToReset;

    public virtual void OnTriggerExit2D(Collider2D other)
    {
        PlayerCollider pt = other.GetComponent<PlayerCollider>();
        if (pt == null) return;
        ToReset.Reset();
    }
}
