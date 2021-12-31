using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireEffect : MonoBehaviour
{
    public float Duration = .5f;
    public float StartAt = -1;
    public float Speed = 6f;
    public Vector2 dir;
    public Facing direction;


    // Update is called once per frame
    void Update()
    {
        if (StartAt < 0) return;
        float percent = (Time.time - StartAt) / Duration;
        if (percent >= 1)
        {
            StartAt = -1;
            this.gameObject.SetActive(false);
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }

    void FixedUpdate()
    {
        if (StartAt < 0) return;
        this.transform.Translate(dir * Speed * Time.fixedDeltaTime);
    }

    void OnEnable()
    {
        this.StartAt = Time.time;
        if (dir == null || dir == new Vector2())
        {
            float x = direction == Facing.East ? 1 : direction == Facing.West ? -1 : 0;
            float y = direction == Facing.North ? 1 : direction == Facing.South ? -1 : 0;
            dir = new Vector2(x, y);
        }
    }
}
