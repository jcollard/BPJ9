using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeltOnFire : MonoBehaviour
{

    public GameObject PuddleTemplate;
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        FireEffect fireEffect = other.GetComponent<FireEffect>();
        if (fireEffect != null)
        {
            this.Melt();
        }
    }

    public void Melt()
    {
        Spawner.SpawnObject(PuddleTemplate.gameObject)
                           .Position(this.transform.position)
                           .Parent(this.transform.parent)
                           .Spawn();
        UnityEngine.Object.Destroy(this.gameObject);
    }

}
