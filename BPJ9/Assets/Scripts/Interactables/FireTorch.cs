using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireTorch : InteractableController
{
    
    public override void Interact(PlayerController player)
    {
        Debug.Log("Updating Fire Power");
        player.CurrentPower = "Fire";
    }
}
