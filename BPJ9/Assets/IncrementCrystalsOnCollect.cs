using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IncrementCrystalsOnCollect : MonoBehaviour
{
    
    public void Start()
    {
        this.GetComponent<Collectable>().OnCollect.Add((_) => PlayerController.Instance.CrystalsFound++);
    }

}
