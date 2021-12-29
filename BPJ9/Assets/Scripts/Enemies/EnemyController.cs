using UnityEngine;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private float _HP = 1;
    public float HP { get => _HP; set => _HP = value; }
    public List<System.Action<EnemyController>> OnDeath = new List<System.Action<EnemyController>>();

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        WeaponController wc = other.GetComponent<WeaponController>();
        if (wc == null) return;
        HP -= wc.Damage;
        if (HP <= 0) this.DoDestroy();
    }

    public virtual void DoDestroy()
    {
        if (this.OnDeath.Count == 0)
        {
            this.gameObject.SetActive(false);
            UnityEngine.Object.Destroy(this.gameObject);
        } 
        else
        {
            foreach (System.Action<EnemyController> action in OnDeath)
                action(this);
        }
    }


}