using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collectable))]
public class HealOnCollect : MonoBehaviour
{

    public float HealAmount = 2f;

    public void Start()
    {
        this.GetComponent<Collectable>().OnCollect.Add((_) => PlayerController.Instance.HP += HealAmount);
    }
    
}
