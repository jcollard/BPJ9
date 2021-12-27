using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    private float _HP = 1;
    public float HP { get => _HP; set => _HP = value; }
    public System.Action<EnemyController> OnDestroy;

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        WeaponController wc = other.GetComponent<WeaponController>();
        if (wc == null) return;
        HP -= wc.Damage;
        if (HP <= 0) this.DoDestroy();
    }

    public virtual void DoDestroy()
    {
        if (this.OnDestroy == null)
        {
            this.gameObject.SetActive(false);
            UnityEngine.Object.Destroy(this.gameObject);
        } 
        else
        {
            this.OnDestroy(this);
        }
    }


}