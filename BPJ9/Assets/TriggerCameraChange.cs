using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerCameraChange : MonoBehaviour
{
    
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerCollider>() != null)
        {
            CameraFollower.Instance.PlayerAtBottom = true;
            MusicFadeIn.Instance.StartFadeOut();
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.GetComponent<PlayerCollider>() != null)
        {
            CameraFollower.Instance.PlayerAtBottom = false;
        }
    }

}
