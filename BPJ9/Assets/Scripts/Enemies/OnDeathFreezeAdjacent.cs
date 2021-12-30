
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(Collider2D))]
public class OnDeathFreezeAdjacent : MonoBehaviour
{

    public EnemyController Enemy;
    public HashSet<Puddle> ToFreeze = new HashSet<Puddle>();

    public void Start()
    {
        this.Enemy.OnDeath.Add(this.FreezeAdjacent);
    }

    private void FreezeAdjacent(EnemyController ec)
    {
        Puddle[] puddles = ToFreeze.Where(p => p != null & p.gameObject != null).ToArray();
        ToFreeze.Clear();
        foreach (Puddle p in puddles)
        {
            if (p != null && p.gameObject != null)
            {
                p.Freeze();
            }
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        Puddle p = other.GetComponent<Puddle>();
        if (p != null)
        {
            ToFreeze.Add(p);
        }
    }

    public virtual void OnTriggerExit2D(Collider2D other)
    {
        Puddle p = other.GetComponent<Puddle>();
        if (p != null)
        {
            ToFreeze.Remove(p);
        }
    }

}