using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnowballController : MonoBehaviour
{
    public float Speed = 8f;
    public float Duration = 5;

    public float StartAt;

    public void OnEnable()
    {
        this.StartAt = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > this.StartAt + Duration)
        {
            this.gameObject.SetActive(false);
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }

    public void FixedUpdate()
    {
        this.transform.Translate(new Vector2(0, -1) * Speed * Time.fixedDeltaTime);
    }
}
