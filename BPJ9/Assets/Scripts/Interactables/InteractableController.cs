using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableController : MonoBehaviour
{

    public virtual void Interact(PlayerController player)
    {

    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        PlayerCollider player = other.GetComponent<PlayerCollider>();
        if (player != null)
        {
            player.Player.CurrentInteractable = this;
        }
    }

    public virtual void OnTriggerStay2D(Collider2D other)
    {
        PlayerCollider player = other.GetComponent<PlayerCollider>();
        if (player != null)
        {
            player.Player.CurrentInteractable = this;
        }
    }

    public virtual void OnTriggerExit2D(Collider2D other)
    {
        PlayerCollider player = other.GetComponent<PlayerCollider>();
        if (player != null)
        {
            player.Player.CurrentInteractable = null;
        }
    }

    public virtual void OnCollisionEnter2D(Collision2D other)
    {
        PlayerCollider player = other.gameObject.GetComponent<PlayerCollider>();
        if (player != null)
        {
            player.Player.CurrentInteractable = this;
        }
    }

    public virtual void OnCollisionStay2D(Collision2D other)
    {
        PlayerCollider player = other.gameObject.GetComponent<PlayerCollider>();
        if (player != null)
        {
            player.Player.CurrentInteractable = this;

        }
    }

    public virtual void OnCollisionExit2D(Collision2D other)
    {
        PlayerCollider player = other.gameObject.GetComponent<PlayerCollider>();
        if (player != null)
        {
            player.Player.CurrentInteractable = null;

        }
    }
}
