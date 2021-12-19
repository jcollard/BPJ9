using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaTile : InteractableController
{
    public HardenedLava HardenedLavaTemplate;
    public void Harden()
    {
        HardenedLava hard = UnityEngine.Object.Instantiate<HardenedLava>(HardenedLavaTemplate);
        hard.transform.SetParent(this.transform.parent);
        hard.transform.position = this.transform.position;
        hard.gameObject.SetActive(true);
        UnityEngine.Object.Destroy(this.gameObject);
    }
}
