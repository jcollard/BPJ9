using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlock : InteractableController
{
    public Puddle PuddleTemplate;
    public override void Interact(PlayerController player)
    {
        

        
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);

        FireEffect fe = other.gameObject.GetComponent<FireEffect>();
        if (fe != null)
        {
            Spawner.SpawnObject(PuddleTemplate.gameObject)
                   .Position(this.transform.position)
                   .Parent(this.transform.parent)
                   .Spawn();
            UnityEngine.Object.Destroy(this.gameObject);
            return;
        }

        FireWall wall = other.gameObject.GetComponent<FireWall>();
        if (wall != null)
        {
            SoundController.PlaySFX("Melt");
            UnityEngine.Object.Destroy(this.gameObject);
            return;
        }

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
