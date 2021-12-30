using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class DisableWhenNoEnemies : MonoBehaviour
{
    
    public HashSet<EnemyController> enemies = new HashSet<EnemyController>();
    public List<GameObject> ToDisable;

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        EnemyController ec = other.GetComponent<EnemyController>();
        if (ec == null) return;
        enemies.Add(ec);
        DoEnable();
    }

    public virtual void OnTriggerExit2D(Collider2D other)
    {
        EnemyController ec = other.GetComponent<EnemyController>();
        if (ec == null) return;
        enemies.Remove(ec);
        if (enemies.Count == 0) DoDisable();
    }
    
    public void DoEnable()
    {
        foreach (GameObject obj in ToDisable)
            obj.SetActive(true);
    }

    public void DoDisable()
    {
        foreach (GameObject obj in ToDisable)
            obj.SetActive(false);
        SoundController.PlaySFX("Doors Close");
    }

    public void OnEnable()
    {
        enemies = new HashSet<EnemyController>();
    }

}
