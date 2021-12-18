using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlock : InteractableController
{
    public Puddle PuddleTemplate;
    public override void Interact(PlayerController player)
    {
        if (player.CurrentPower == "Fire")
        {
            Puddle newPuddle = UnityEngine.Object.Instantiate<Puddle>(PuddleTemplate);
            newPuddle.transform.SetParent(this.transform.parent);
            newPuddle.transform.position = this.transform.position;
            newPuddle.gameObject.SetActive(true);
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }
}