using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceTile : InteractableController
{
    
    public override void Interact(PlayerController player)
    {
        if (player.CurrentPower == "Fire")
        {
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }
}
