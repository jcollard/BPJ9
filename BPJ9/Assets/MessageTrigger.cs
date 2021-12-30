using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageTrigger : MonoBehaviour
{

    public string Message;
    public bool Displayed;
    public bool OneTime = true;
    
    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        PlayerCollider player = other.gameObject.GetComponent<PlayerCollider>();
        if (player != null)
        {
            if (DialogController.Instance.IsVisible) return;
            DialogController.Instance.WriteText(Message);
            Displayed = true;
        }
    }

    public virtual void OnTriggerExit2D(Collider2D other)
    {
        PlayerCollider player = other.gameObject.GetComponent<PlayerCollider>();
        if (player != null && Displayed && OneTime)
        {
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }
}
