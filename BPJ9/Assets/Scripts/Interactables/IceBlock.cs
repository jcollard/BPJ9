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
            Spawner.SpawnObject(PuddleTemplate.gameObject)
                   .Position(this.transform.position)
                   .Parent(this.transform.parent)
                   .Spawn();
            UnityEngine.Object.Destroy(this.gameObject);
        }

        
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        LavaTile tile = other.gameObject.GetComponent<LavaTile>();
        if (tile != null)
        {
            tile.Harden();
            this.MeltAndDestroy();
        }
    }

    public void MeltAndDestroy()
    {
        UnityEngine.Object.Destroy(this.gameObject);
    }

}
