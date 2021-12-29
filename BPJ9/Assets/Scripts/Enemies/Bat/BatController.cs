using System.Collections;
using System.Collections.Generic;
using CaptainCoder.Unity;
using CaptainCoder.Unity.GameObjectExtensions;
using UnityEngine;

[RequireComponent(typeof(EnemyController))]
public class BatController : MonoBehaviour
{ 
    public GameObject Idle, Awake;
    public bool IsAwake;

    public float StartSpeed = 2;
    public float Acceleration = 2f;
    public float Deceleration = 6f;
    public float Speed = 2;
    public float MaxSpeed = 6;
    public Vector2 StartPosition;
    public Vector2 Direction;
    private char Room;
    public bool IsOutOfBounds = false;
    public bool FindPlayer = false;
    public bool IsSlowing = false;

    void Start()
    {
        this.StartPosition = this.transform.position;
        if(!MapChunker.Instance.TryGetRoom(this.gameObject, out this.Room)) UnityEngineUtils.Instance.FailFast($"Bat started outside of a room!", this);
        this.GetComponent<EnemyController>().OnDeath.Add(this.OnDeath);        
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


        if (!IsOutOfBounds)
        {
            IsSlowing = false;
            FindPlayer = false;
            Speed = Mathf.Min(MaxSpeed, Speed + Acceleration * Time.deltaTime);
            return;
        }

        if (FindPlayer)
        {
            ChangeDirection();    
            Speed = Mathf.Min(MaxSpeed, Speed + Acceleration * Time.deltaTime);    
            return;
        }


        if (IsOutOfBounds && !IsSlowing)
        {
            IsSlowing = true;
         }
         else if (IsOutOfBounds && IsSlowing && Speed > StartSpeed)
        {
            Speed = Mathf.Max(StartSpeed, Speed - Deceleration * Time.deltaTime);
        } 
        else 
        {
            FindPlayer = true;
        }

    }

    void FixedUpdate()
    {
        if (!IsAwake) return;
        this.transform.Translate(Direction * Speed * Time.fixedDeltaTime);
    }

    private void Reset()
    {
        this.IsAwake = false;
        Awake.SetActive(false);
        Idle.SetActive(true);
        this.gameObject.SetPosition2D(StartPosition);
    }

    public void WakeUp()
    {
        IsOutOfBounds = false;
        if (IsAwake) return;
        SoundController.PlayRandomSFX("Bat 1", "Bat 2");
        Awake.SetActive(true);
        Idle.SetActive(false);
        IsAwake = true;
        Speed = StartSpeed;
        ChangeDirection();
        
    }

    public void ChangeDirection()
    {
        Direction = (PlayerController.Instance.transform.position - this.transform.position).normalized;
        bool flipX = Direction.x > 0;
        this.Idle.GetComponent<SpriteRenderer>().flipX = flipX;
        this.Awake.GetComponent<SpriteRenderer>().flipX = flipX;
    }

    private void OnDeath(EnemyController ec)
    {
        SoundController.PlaySFX("Bat Destroyed");
    }

}
