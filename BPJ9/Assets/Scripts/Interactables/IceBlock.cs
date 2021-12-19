using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlock : InteractableController
{
    public Puddle PuddleTemplate;
    public float PushTime;
    public float PushDelay = 0.5f;
    public Vector2 Velocity;
    public float SlideSpeed = 10;

    public PlayerController Player;
    public override void Interact(PlayerController player)
    {
        if (player.CurrentPower == "Fire")
        {
            Puddle newPuddle = UnityEngine.Object.Instantiate<Puddle>(PuddleTemplate);
            newPuddle.transform.SetParent(this.transform.parent);
            newPuddle.transform.position = this.transform.position;
            newPuddle.gameObject.SetActive(true);
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }

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
        this.Player.TrySetPushing(this);
        if (Player.IsMoving && Player.Pushing == this)
        {
            this.PushTime += Time.deltaTime;
            if (PushTime > PushDelay)
            {
                CollisionDirection d = new CollisionDirection(Player.gameObject, this.gameObject);
                // TODO: Figure out where the block will slide rather than using collision which is finicky
                if (d.IsLeft && Player.IsMovingLeft)
                {
                    this.Velocity = new Vector2(-SlideSpeed, 0);
                }
                if (d.IsRight && Player.IsMovingRight)
                {
                    this.Velocity = new Vector2(SlideSpeed, 0);
                }

                if (d.IsDown && Player.IsMovingDown)
                {
                    this.Velocity = new Vector2(0, -SlideSpeed);
                }

                if (d.IsUp && Player.IsMovingUp)
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

    public override void HandlePlayerEnter(PlayerController player, Collision2D collision)
    {
        this.PushTime = 0;
        this.Player = player;
        this.Player.TrySetPushing(this);
    }

    public override void HandlePlayerExit(PlayerController player, Collision2D collision)
    {
        this.PushTime = 0;
        this.Player = player;
        this.Player.TryClearPushing(this);
    }

    public override void OnTriggerEnter2D(Collider2D other)
    {
        base.OnTriggerEnter2D(other);
        LavaTile tile = other.gameObject.GetComponent<LavaTile>();
        if (tile != null)
        {
            UnityEngine.Object.Destroy(tile.gameObject);
            UnityEngine.Object.Destroy(this.gameObject);
            return;
        }

        IceBlock iceblock = other.gameObject.GetComponent<IceBlock>();
        if (iceblock != null && this.Velocity.magnitude > 0)
        {
            CollisionDirection d = new CollisionDirection(iceblock.gameObject, this.gameObject);
            Vector3 newPosition = iceblock.transform.position;
            newPosition.x += d.IsLeft ? -1 : 0;
            newPosition.x += d.IsRight ? 1 : 0;
            newPosition.y += d.IsDown ? -1 : 0;
            newPosition.y += d.IsUp ? 1 : 0;
            this.transform.position = newPosition;

            this.Velocity = new Vector2();
            return;
        }
    }

    public override void OnTriggerExit2D(Collider2D other)
    {
        base.OnTriggerExit2D(other);
        Debug.Log("Ice Block Exit");
    }
}
