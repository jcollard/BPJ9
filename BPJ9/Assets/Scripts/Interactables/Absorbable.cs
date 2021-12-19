using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Absorbable : MonoBehaviour
{
    public PowerType Power;
    public void Absorb(PlayerController player)
    {
        player.CurrentPower = Power;
    }
}
