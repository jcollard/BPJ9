using UnityEngine;

public class OnTriggerEnable : MonoBehaviour
{
    public PressurePlate Trigger;

    public void Start()
    {
        Trigger.OnUntrigger.Add((_) => this.gameObject.SetActive(false));
        Trigger.OnTrigger.Add((_) => this.gameObject.SetActive(true));
        this.gameObject.SetActive(false);
    }
}