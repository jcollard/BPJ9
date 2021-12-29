using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    public float StartCollection = -1;
    public float CollectionTime = 1.0f;
    public float EndDelay = 0.5f;
    private Vector3 StartPos, EndPos;
    private PlayerController player;

    public void Update()
    {
        if (StartCollection < 0) return;
        float percent = (Time.time - StartCollection) / CollectionTime;
        this.transform.position = Vector3.Lerp(StartPos, EndPos, percent);
        if (Time.time > CollectionTime + EndDelay + StartCollection)
        {
            this.EndCollect(player);
            this.gameObject.SetActive(false);
            UnityEngine.Object.Destroy(this);
        }
        
    }

    public virtual void StartCollect(PlayerController player)
    {
        this.player = player;
        this.StartCollection = Time.time;
        player.StartCollection();
        StartPos = player.transform.position;
        StartPos.z -= 1;
        EndPos = player.transform.position;
        EndPos.y += 1.5f;
        EndPos.z -= 1;
        this.GetComponent<Collider2D>().enabled = false;
        SoundController.PlaySFX("Collect Gem");
    }

    public virtual void EndCollect(PlayerController player)
    {
        player.EndCollection();
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        PlayerCollider player = other.GetComponent<PlayerCollider>();
        if (player != null)
        {
            this.StartCollect(player.Player);
        }
    }

}
