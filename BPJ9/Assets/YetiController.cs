using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class YetiController : MonoBehaviour
{

    public GameObject Idle, Smash, Jump, Land, Fall, Hurt;
    

    public float HurtAt;
    public float HurtDuration = 3f;

    public bool IsHurt;
    public bool IsIdle = true;

    public EnemyController _EnemyController;
    public KnockBackOnHit _KnockBackOnHit;

    public void Start()
    {
        _EnemyController.IsInvulnerable = true;
        _KnockBackOnHit.IsEnabled = true;
    }

    public void Update()
    {

    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        IceBlock ib = other.gameObject.GetComponent<IceBlock>();
        if (ib != null && IsIdle)
        {
            DoHurt();
        }
    }

    public void DoHurt()
    {

        IsIdle = false;
        IsHurt = true;
        _KnockBackOnHit.IsEnabled = false;
        _EnemyController.IsInvulnerable = false;
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        Idle.SetActive(IsIdle);
        Hurt.SetActive(IsHurt);
    }

}
