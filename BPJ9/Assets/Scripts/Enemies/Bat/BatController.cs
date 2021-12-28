using System.Collections;
using System.Collections.Generic;
using CaptainCoder.Unity;
using CaptainCoder.Unity.GameObjectExtensions;
using UnityEngine;

public class BatController : MonoBehaviour
{ 
    public GameObject Idle, Awake;
    public bool IsAwake;

    public float StartSpeed = 2;
    public float Acceleration = 2f;
    public float Speed = 2;
    public float MaxSpeed = 6;
    public Vector2 StartPosition;
    public Vector2 Direction;
    private char Room;
    public bool FindPlayer = false;

    void Start()
    {
        this.StartPosition = this.transform.position;
        if(!MapChunker.Instance.TryGetRoom(this.gameObject, out this.Room)) UnityEngineUtils.Instance.FailFast($"Bat started outside of a room!", this);
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsAwake) return;

        if (PlayerController.Instance.CurrentRoom != this.Room)
        {
            Reset();
            return;
        }

        if (FindPlayer && Speed > StartSpeed)
        {
            Speed = Mathf.Max(StartSpeed, Speed - Acceleration * Time.deltaTime);
        } 
        else if (FindPlayer)
        {
            ChangeDirection();    
        }
        else
        {
            Speed = Mathf.Min(MaxSpeed, Speed + Acceleration * Time.deltaTime);    
        }
        
    }

    void FixedUpdate()
    {
        if (!IsAwake) return;
        this.transform.Translate(Direction * Speed * Time.fixedDeltaTime);
    }

    private void Reset()
    {
        Debug.Log("Reset!");
        this.IsAwake = false;
        Awake.SetActive(false);
        Idle.SetActive(true);
        this.gameObject.SetPosition2D(StartPosition);
    }

    public void WakeUp()
    {
        FindPlayer = false;
        if (IsAwake) return;
        Awake.SetActive(true);
        Idle.SetActive(false);
        IsAwake = true;
        Speed = StartSpeed;
        ChangeDirection();
        
    }

    public void ChangeDirection()
    {
        Direction = (PlayerController.Instance.transform.position - this.transform.position).normalized;
    }

}
