using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreventLeave : MonoBehaviour
{
    
    public virtual void OnCollisionEnter2D(Collision2D other)
    {
        PlayerCollider player = other.gameObject.GetComponent<PlayerCollider>();
        if (player != null)
        {
            if (DialogController.Instance.IsVisible) return;
            DialogController.Instance.WriteText("I can't leave yet. I must complete the trial...");
        }
    }

}
