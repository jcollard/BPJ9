using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OverlayController : MonoBehaviour
{
    private static OverlayController _Instance;
    public static OverlayController Instance
    {
        get
        {
            if (_Instance == null)
                _Instance = GameObject.FindGameObjectWithTag("Overlay").GetComponent<OverlayController>();

            return _Instance;
        }
        private set
        {
            _Instance = value;
        }
    }

    public HeartController Hearts;

    public void Awake() => Instance = this;

    public void UpdatePlayerInfo(PlayerController player)
    {
        Hearts.UpdateHearts(player.HP, player.MaxHP);
    }

}
