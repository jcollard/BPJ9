using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntroPreventLeave : MonoBehaviour
{

    
    public virtual void OnTriggerStay2D(Collider2D other)
    {
        PlayerCollider player = other.gameObject.GetComponent<PlayerCollider>();
        if (player != null)
        {
            PlayerController pc = player.Player;
            if (pc.CurrentFacing != Facing.South) return;
            
            DialogController.Instance.WriteText("I cannot leave yet... I must complete my trial.");
            pc.SetDirectionalSprite(Facing.North);
        }
    }
}