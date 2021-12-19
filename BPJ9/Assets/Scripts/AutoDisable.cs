using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisable : MonoBehaviour
{
    public float Duration;
    public float StartAt = -1;

    // Update is called once per frame
    void Update()
    {
       float EndAt = StartAt + Duration;
       if (EndAt > Time.time) return;
       this.gameObject.SetActive(false);
    }

    public void OnEnable()
    {
        this.StartAt = Time.time;
    }
}
