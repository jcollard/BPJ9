using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puddle : InteractableController
{
    public IceBlock[] IceBlockTemplates;
    public override void Interact(PlayerController player)
    {
        if (player.CurrentPower == PowerType.Ice)
        {
            Freeze();
        }
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        IceEffect iceEffect = other.GetComponent<IceEffect>();
        if (iceEffect != null)
        {
            this.Freeze();
        }
    }

    public void Freeze()
    {
        Spawner.SpawnObject(IceBlockTemplates[Random.Range(0, IceBlockTemplates.Length)].gameObject)
                           .Position(this.transform.position)
                           .Parent(this.transform.parent)
                           .Spawn();
        UnityEngine.Object.Destroy(this.gameObject);
    }
}
