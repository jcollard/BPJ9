using System;
using System.Collections;
using System.Collections.Generic;
using CaptainCoder.Unity;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(Collider2D))]
public class PlayerController : MonoBehaviour
{
    public bool IsMoving => !(DirectionX == 0 && DirectionY == 0);
    public bool IsMovingLeft => DirectionX < 0;
    public bool IsMovingRight => DirectionX > 0;
    public bool IsMovingDown => DirectionY < 0;
    public bool IsMovingUp => DirectionY > 0;

    [SerializeField]
    private float _HP = 6;

    [SerializeField]
    private int _MaxHP = 6;

    public int MaxHP
    {
        get => _MaxHP;
        set => SetAndNotify(ref _MaxHP, value);
    }

    public float HP 
    {
        get => _HP;
        set => SetAndNotify(ref _HP, Mathf.Min(_MaxHP, value));
    }

    
    
    public Vector2 Velocity => new Vector2(DirectionX, DirectionY) * Speed;
    public PowerType CurrentPower = PowerType.None;
    public InteractableController CurrentInteractable;
    public Pushable Pushing;
    public float Speed, DirectionX, DirectionY, LastDirectionX, LastDirectionY;
    public AbsorbEffect AbsorbEffectReference;
    private bool IsAnimating = false;
    public bool CanMove => !IsAnimating;
    public bool IsAbsorbing => AbsorbEffectReference.gameObject.activeInHierarchy;

    private Dictionary<string, System.Action> _MovementControls;
    public Dictionary<string, System.Action> MovementControls
    {
        get
        {
            if (_MovementControls == null) _MovementControls = GetMovementControls();
            return _MovementControls;
        }
    }

    private Dictionary<string, System.Action> _ActionControls;
    public Dictionary<string, System.Action> ActionControls
    {
        get
        {
            if (_ActionControls == null) _ActionControls = GetActionControls();
            return _ActionControls;
        }
    }

    private Collider2D Collider;

    public List<GameObject> DirectionalSprites;

    public void Start()
    {
        Collider = this.GetComponent<Collider2D>();
        if (Collider == null)
        {
            throw new System.Exception("Could not locate Collider2D");
        }
    }

    private bool TryRaycast(Vector2 direction, out RaycastHit2D result, Type type)
    {
        RaycastHit2D[] results = Physics2D.RaycastAll(this.transform.position, direction);
        RaycastHit2D[] filtered = results.Where(r => r.transform != null && r.transform.GetComponent(type)).ToArray();
        if (filtered.Length == 0)
        {
            result = results[0];
            return false;
        }
        result = filtered[0];
        return true;
    }

    /// <summary>
    /// Attempts to set the object as being pushed. An object can only be pushed
    /// if it is in the raycast of the players movement direction.
    /// </summary>
    /// <param name="pushable">The object to be pushed</param>
    /// <returns>true if the object specified is being pushed</returns>
    public bool TrySetPushing(Pushable pushable)
    {
        RaycastHit2D result;
        if (!this.TryRaycast(this.Velocity, out result, typeof(Pushable))) return false;
        if (result.transform == pushable.transform)
        {
            this.Pushing = pushable;
            return true;
        }
        return false;
    }

    /// <summary>
    /// If the object was being pushed, it is cleared. Otherwise, does nothing.
    /// </summary>
    /// <param name="pushable"></param>
    /// <returns>true if the object was cleared</returns>
    public bool TryClearPushing(Pushable pushable)
    {
        if (this.Pushing == pushable)
        {
            this.Pushing = null;
            return true;
        }
        return false;
    }

    private Dictionary<string, System.Action> GetMovementControls()
    {
        Dictionary<string, System.Action> MovementControls = new Dictionary<string, System.Action>();
        MovementControls["Left"] = () => this.CalcMove(-1, 0);
        MovementControls["Right"] = () => this.CalcMove(1, 0);
        MovementControls["Up"] = () => this.CalcMove(0, 1);
        MovementControls["Down"] = () => this.CalcMove(0, -1);
        return MovementControls;
    }

    private Dictionary<string, System.Action> GetActionControls()
    {
        Dictionary<string, System.Action> ActionControls = new Dictionary<string, System.Action>();
        ActionControls["Interact"] = () => this.DoInteract();
        ActionControls["Absorb"] = () => this.DoAbsorb();
        ActionControls["Diagnostics"] = () => Debug.Log(TimerUtil.ReportAllTimers());
        ActionControls["ResetTimers"] = () => TimerUtil.ResetTimers();
        return ActionControls;
    }


    // Update is called once per frame
    void Update()
    {
        DirectionX = DirectionY = 0;

        HandleInput();

        HandleDirection();

        if (Velocity.magnitude == 0)
            this.Pushing = null;

        UpdateScreen();
    }

    void FixedUpdate()
    {
        DoMove();
    }

    private void HandleDirection()
    {
        // Check to see if we have switched direction
        if (LastDirectionX == DirectionX && LastDirectionY == DirectionY) return;
        // Are we moving?
        if (Velocity.magnitude == 0) return;
        Facing f = Facing.South;
        // West
        if (Velocity.x < 0) f = Facing.West;
        // East
        else if (Velocity.x > 0) f = Facing.East;
        // South
        else if (Velocity.y < 0) f = Facing.South;
        // North
        else if (Velocity.y > 0) f = Facing.North;
        this.SetDirectionalSprite(f);
    }

    private void UpdateScreen()
    {
        
    }

    private void HandleInput()
    {
        foreach (string control in MovementControls.Keys)
        {
            if (Input.GetButton(control))
            {
                MovementControls[control]();
            }
        }

        foreach (string control in ActionControls.Keys)
        {
            if (Input.GetButtonDown(control))
            {
                ActionControls[control]();
            }
        }
    }

    private void CalcMove(float x, float y)
    {
        if (!CanMove) return;
        DirectionX += x;
        DirectionY += y;
    }

    private void DoMove()
    {
        if (DirectionX == 0 && DirectionY == 0) return;
        Vector2 dir = new Vector2(DirectionX, DirectionY);
        this.transform.Translate(dir * Speed * Time.fixedDeltaTime);
    }

    private void DoInteract()
    {
        if (this.CurrentInteractable == null) return;
        this.CurrentInteractable.Interact(this);
    }

    private void DoAbsorb()
    {
        if (this.CurrentInteractable == null) return;
        Absorbable a = this.CurrentInteractable.GetComponent<Absorbable>();
        if (a == null) return;
        a.Absorb(this);
        this.AbsorbEffectReference.gameObject.SetActive(true);
    }

    private void SetDirectionalSprite(Facing facing)
    {
        int ix = (int)facing;
        for (int i = 0; i < 4; i++)
            DirectionalSprites[i].SetActive(i == ix);
    }

    public void StartCollection()
    {

        this.IsAnimating = true;
        this.SetDirectionalSprite(Facing.South);

    }

    public void EndCollection()
    {
        this.IsAnimating = false;
    }

    private void SetAndNotify<T>(ref T toUpdate, T value)
    {
        toUpdate = value;
        OverlayController.Instance.UpdatePlayerInfo(this);
    }


}

public enum Facing
{
    North,
    East,
    South,
    West
}
