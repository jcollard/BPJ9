using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndGame : MonoBehaviour
{
    public CanvasGroup EndScreen;

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<PlayerCollider>() != null)
        {
            EndScreen.gameObject.SetActive(true);
        }
    }
}
