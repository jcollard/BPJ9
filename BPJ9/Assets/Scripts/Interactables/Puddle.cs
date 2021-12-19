using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puddle : InteractableController
{
    public IceBlock IceBlockTemplate;
    public override void Interact(PlayerController player)
    {
        if (player.CurrentPower == PowerType.Ice)
        {
            Spawner.SpawnObject(IceBlockTemplate.gameObject)
                   .Position(this.transform.position)
                   .Parent(this.transform.parent)
                   .Spawn();
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }
}
