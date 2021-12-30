using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentPowerController : MonoBehaviour
{
    public static CurrentPowerController Instance;
    
    public Sprite[] ShardSprites;
    public Sprite IceSprite;
    public Sprite FireSprite;

    public void Start()
    {
        Instance = this;
    }

    public void SetPower(PowerType pt)
    {

        if (pt == PowerType.Ice)
        {
            this.GetComponent<Image>().sprite = IceSprite;    
        }
        else if (pt == PowerType.Fire)
        {
            this.GetComponent<Image>().sprite = FireSprite;
        }
    }

    public void SetCrystalShard(int ix)
    {
        this.GetComponent<Image>().sprite = ShardSprites[ix];
    }

}
