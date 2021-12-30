using UnityEngine;


public class OnTriggerChangeSprite : MonoBehaviour
{

    public PressurePlate Trigger;
    public Sprite Original;
    public Sprite Triggered;

    public void Start()
    {
        Original = this.GetComponent<SpriteRenderer>().sprite;
        Trigger.OnTrigger.Add(ChangeSprite);
        Trigger.OnUntrigger.Add(UnChangeSprite);
    }

    private void ChangeSprite(PressurePlate pt)
    {
        this.GetComponent<SpriteRenderer>().sprite = Triggered;
    }

    private void UnChangeSprite(PressurePlate pt)
    {
        this.GetComponent<SpriteRenderer>().sprite = Original;
    }

}