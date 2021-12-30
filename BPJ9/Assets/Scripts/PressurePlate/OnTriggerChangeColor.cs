using UnityEngine;


public class OnTriggerChangeColor : MonoBehaviour
{

    public PressurePlate Trigger;
    public Color NewColor;
    public Color OriginalColor;

    public void Start()
    {
        Trigger.OnTrigger.Add(ChangeColor);
        Trigger.OnUntrigger.Add(UnChangeColor);
    }

    private void ChangeColor(PressurePlate pt)
    {
        this.GetComponent<SpriteRenderer>().color = NewColor;
    }

    private void UnChangeColor(PressurePlate pt)
    {
        this.GetComponent<SpriteRenderer>().color = OriginalColor;
    }

}