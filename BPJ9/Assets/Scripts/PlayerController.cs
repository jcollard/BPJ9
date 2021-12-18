using System.Collections;
using System.Collections.Generic;
using CaptainCoder.Unity;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public bool IsMoving => !(DirectionX == 0 && DirectionY == 0);
    public TextGroup CurrentPowerText;
    public string CurrentPower = "None";
    public InteractableController CurrentInteractable;
    public float Speed;

    public float DirectionX, DirectionY;

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
        return ActionControls;
    }


    // Update is called once per frame
    void Update()
    {
        DirectionX = DirectionY = 0;
        HandleInput();
        UpdateScreen();
    }

    void FixedUpdate()
    {
        DoMove();
    }

    private void UpdateScreen()
    {
        CurrentPowerText.SetText($"Current Power: {CurrentPower}");
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


}
