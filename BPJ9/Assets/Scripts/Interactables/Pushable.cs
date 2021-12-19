using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Pushable : MonoBehaviour
{
    public float PushTime;
    public float PushDelay = 0.5f;
    public Vector2 Velocity;
    public float SlideSpeed = 10;

    public PlayerController Player;

    public void Update()
    {
        CheckForPush();
    }

    public void FixedUpdate()
    {
        HandlePush();
    }

    private void HandlePush()
    {
        this.transform.Translate(this.Velocity * Time.fixedDeltaTime);
    }

    private void CheckForPush()
    {
        if (Player == null) return;
        if (Player.Pushing == this)
        {
            this.PushTime += Time.deltaTime;
            if (PushTime > PushDelay)
            {
                PushTime = 0;
                CollisionDirection d = new CollisionDirection(Player.gameObject, this.gameObject);
                // TODO: Figure out where the block will slide rather than using collision which is finicky
                if (d.IsLeft)
                {
                    this.Velocity = new Vector2(-SlideSpeed, 0);
                }
                else if (d.IsRight)
                {
                    this.Velocity = new Vector2(SlideSpeed, 0);
                }
                else if (d.IsDown)
                {
                    this.Velocity = new Vector2(0, -SlideSpeed);
                }
                else if (d.IsUp)
                {
                    this.Velocity = new Vector2(0, SlideSpeed);
                }
            }
        }
        else
        {
            this.PushTime = 0;
        }
    }

    public void OnCollisionEnter2D(Collision2D collision)
    {
        this.PushTime = 0;
        this.Player = collision.gameObject.GetComponent<PlayerController>();
        if (this.Player == null) return;
        this.Player.TrySetPushing(this);
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        this.Player = collision.gameObject.GetComponent<PlayerController>();
        if (this.Player == null) return;
        this.Player.TrySetPushing(this);
    }

    public void OnCollisionExit2D(Collision2D collision)
    {
        this.PushTime = 0;
        this.Player = collision.gameObject.GetComponent<PlayerController>();
        if (this.Player == null) return;
        this.Player.TryClearPushing(this);
    }

    public virtual void OnTriggerEnter2D(Collider2D other)
    {
        
        WallBlock wallBlock = other.gameObject.GetComponent<WallBlock>();
        if (wallBlock != null && this.Velocity.magnitude > 0)
        {
            CollisionDirection d = new CollisionDirection(other.gameObject, this.gameObject);
            Vector3 newPosition = other.transform.position;
            newPosition.x += d.IsLeft ? -1 : 0;
            newPosition.x += d.IsRight ? 1 : 0;
            newPosition.y += d.IsDown ? -1 : 0;
            newPosition.y += d.IsUp ? 1 : 0;
            this.transform.position = newPosition;
            this.Velocity = new Vector2();
            return;
        }

    }
}
