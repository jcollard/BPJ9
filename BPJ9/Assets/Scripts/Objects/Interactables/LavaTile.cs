using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaTile : InteractableController
{
    public HardenedLava HardenedLavaTemplate;
    public void Harden()
    {
        Spawner.SpawnObject(HardenedLavaTemplate.gameObject)
               .Position(this.transform.position)
               .Parent(this.transform.parent)
               .Spawn();
        UnityEngine.Object.Destroy(this.gameObject);
    }
}
