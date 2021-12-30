
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(EnemyController))]
public class OnDeathSound : MonoBehaviour
{

    public string[] sfx;


    public void Start()
    {
        this.GetComponent<EnemyController>().OnDeath.Add(this.PlaySound);
    }

    private void PlaySound(EnemyController ec)
    {
        SoundController.PlayRandomSFX(sfx);
    }

    

}