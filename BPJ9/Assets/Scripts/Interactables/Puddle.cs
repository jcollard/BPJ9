using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puddle : InteractableController
{
    public IceBlock IceBlockTemplate;
    public override void Interact(PlayerController player)
    {
        if (player.CurrentPower == "Ice")
        {
            IceBlock newBlock = UnityEngine.Object.Instantiate<IceBlock>(IceBlockTemplate);
            newBlock.transform.SetParent(this.transform.parent);
            newBlock.transform.position = this.transform.position;
            newBlock.gameObject.SetActive(true);
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }
}
