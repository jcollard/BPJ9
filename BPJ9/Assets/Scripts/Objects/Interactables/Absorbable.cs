using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Absorbable : MonoBehaviour
{
    public PowerType Power;
    public Color AbsorbColor;
    public void Absorb(PlayerController player)
    {
        player.CurrentPower = Power;
        player.AbsorbEffectReference.StartColor = AbsorbColor;
    }
}
