using System.Collections;
using System.Collections.Generic;
using CaptainCoder.Unity.GameObjectExtensions;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class TransitionController : MonoBehaviour
{

    public TransitionController TeleportTo;

    public virtual void OnTriggerExit2D(Collider2D other)
    {
        PlayerCollider player = other.GetComponent<PlayerCollider>();
        if (player != null)
        {
            player.Player.TransitionTo(TeleportTo);
        }
    }



}
