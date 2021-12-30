using UnityEngine;

public class OnTriggerDisable : MonoBehaviour
{
    public PressurePlate Trigger;

    public void Start()
    {
        Trigger.OnTrigger.Add((_) => this.gameObject.SetActive(false));
        Trigger.OnUntrigger.Add((_) => this.gameObject.SetActive(true));
    }
}