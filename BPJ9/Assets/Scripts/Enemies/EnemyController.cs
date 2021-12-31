using UnityEngine;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    public bool IsInvulnerable = false;
    public bool Flicker = false;
    [SerializeField]
    private float _HP = 1;
    public float HP { get => _HP; set => _HP = value; }
    public List<System.Action<EnemyController>> OnDeath = new List<System.Action<EnemyController>>();
    public string[] OnHitSound;

    public float FlickerAt;
    public float FlickerDuration = 0.05f;
    public SpriteRenderer Renderer;

    public void Update()
    {
        if (!Flicker || FlickerAt < 0) return;
        if (Time.time > FlickerAt + FlickerDuration)
        {
            FlickerAt = -1;
            Color c = Renderer.color;
            c.a = 1;
            Renderer.color = c;
        }
        else
        {
            Color c = Renderer.color;
            c.a = 0;
            Renderer.color = c;
        }
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        WeaponController wc = other.GetComponent<WeaponController>();
        if (IsInvulnerable || wc == null) return;
        HP -= wc.Damage;
        if (HP <= 0) this.DoDestroy();
        else if (OnHitSound.Length > 0)
        {
            if (FlickerAt < 0)
                FlickerAt = Time.time;
            SoundController.PlayRandomSFX(OnHitSound);
        } 
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