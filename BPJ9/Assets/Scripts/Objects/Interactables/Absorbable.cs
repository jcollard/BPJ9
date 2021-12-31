using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(InteractableController))]
public class Absorbable : MonoBehaviour
{
    public PowerType Power;
    public Color AbsorbColor;
    public void Absorb(PlayerController player)
    {
        if (player.CanAbsorb)
        {
            player.CurrentPower = Power;
            player.AbsorbEffectReference.StartColor = AbsorbColor;
            if (Power == PowerType.Ice)
            {
                SoundController.PlaySFX("Absorb Ice");
            }
            else if (Power == PowerType.Fire)
            {
                SoundController.PlaySFX("Absorb Fire");
            }
        }
    }
}
