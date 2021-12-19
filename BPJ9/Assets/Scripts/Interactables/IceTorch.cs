using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceTorch : InteractableController
{
    
    public override void Interact(PlayerController player)
    {
        player.CurrentPower = "Ice";
    }
}
