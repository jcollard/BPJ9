using System.Collections;
using System.Collections.Generic;
using CaptainCoder.Unity.GameObjectExtensions;
using UnityEngine;

public class YetiController : MonoBehaviour
{

    public GameObject Idle, Smash, Jump, Land, Fall, Hurt;

    public Transform[] SnowballPositions;

    public SnowballController SnowballTemplate;

    public float HurtAt;
    public float HurtDuration = 3f;

    public bool IsHurt, IsJumping, IsLanding;
    public bool IsIdle = true;
    public bool IsSmashing = false;

    public float SnowBallAt = -1;
    public float SnowBallDelay = .2f;
    public int SnowBalls = 4;
    public float SnowBallSpacing = 2f;

    public float IdleAt = 0;
    public float IdleDuration = 1f;

    public float JumpAt = -1;
    public float JumpDuration = 1f;
    public float LandAt = -1;
    public float LandDuration = 1f;
    public Transform[] LandLocations;

    public Transform JumpLocation;
    public Vector2 Target;
    public Vector2 LastTarget;
    public int Jumps;

    public EnemyController _EnemyController;
    public KnockBackOnHit _KnockBackOnHit;

    public bool Started = false;
    public bool IsDead = false;

    public GameObject DeathEffect;

    public float DeathAt = -1;
    public float DeathDuration = 3f;
    public float DeathFlashSpeed = 10f;

    public void Start()
    {
        _EnemyController.IsInvulnerable = true;
        _KnockBackOnHit.IsEnabled = true;
        IdleAt = Time.time;
        GetComponent<EnemyController>().OnDeath.Add(OnDeath);
    }

    public void Update()
    {
        if (!Started) return;
        if (IsSmashing) DoSmash();
        if (IsIdle) HandleIdle();
        if (IsHurt) HandleHurt();
        if (IsJumping) HandleJump();
        if (IsLanding) HandleLand();
        if (IsDead) HandleDeath();
    }

    public void HandleDeath()
    {
        if (Time.time > DeathAt + DeathDuration)
        {
            UnityEngine.Object.Destroy(this.gameObject);
            DialogController.Instance.WriteText("Whew... That was close!");
        }
    }

    public void OnDeath(EnemyController ec)
    {
        this.IsDead = true;
        this.IsHurt = false;
        this.IsIdle = false;
        this.IsJumping = false;
        this.IsLanding = false;
        this.IsSmashing = false;
        this.DeathEffect.SetActive(true);
        this.DeathAt = Time.time;
    }

    public void StartFight()
    {
        Started = true;
        IdleAt = Time.time;
    }

    public void HandleJump()
    {
        float percent = (Time.time - JumpAt) / JumpDuration;
        this.gameObject.SetPosition2D(Vector2.Lerp(LastTarget, Target, percent));
        if (percent >= 1)
        {
            StartLand();
        }

    }

    public void HandleLand()
    {
        float percent = (Time.time - LandAt) / LandDuration;
        this.gameObject.SetPosition2D(Vector2.Lerp(LastTarget, Target, percent));
        if (percent >= 1)
        {
            StartJump();
        }
    }

    public void HandleIdle()
    {
        if (IdleAt < 0) return;
        if (Time.time > IdleAt + IdleDuration)
        {
            IdleAt = -1;
            if (Random.Range(0f, 1f) < 0.1f)
            {
                StartJump();
            }
            else
            {
                ThrowSnowBalls();
            }
            
        }
    }

    public void HandleHurt()
    {
        if (HurtAt < 0) return;
        if (Time.time > HurtAt + HurtDuration)
        {
            HurtAt = -1;
            _EnemyController.IsInvulnerable = true;
            _KnockBackOnHit.IsEnabled = true;
            DoJump();
        }
    }

    public void DoJump()
    {
        Jumps = 5;
        IsHurt = false;
        StartJump();
    }

    public void StartJump()
    {
        if (Jumps <= 0)
        {
            DoIdle();
            return;
        }

        JumpAt = Time.time;
        Jumps--;
        LastTarget = this.transform.position;
        Target = JumpLocation.position;
        IsJumping = true;
        IsLanding = false;
        IsIdle = false;
        IsSmashing = false;
        IsHurt = false;
        UpdateSprite();
    }

    public void StartLand()
    {
        IsIdle = false;
        IsSmashing = false;
        IsHurt = false;
        IsJumping = false;
        IsLanding = true;
        LandAt = Time.time;
        LastTarget = JumpLocation.position;
        LandAt = Time.time;
        Target = LandLocations[Random.Range(0, LandLocations.Length)].position;
        UpdateSprite();
    }

    public void DoSmash()
    {
        if (SnowBalls < 0)
        {
            IsIdle = true;
            IsSmashing = false;
            DoIdle();
            return;
        }

        if (Time.time > SnowBallAt + SnowBallDelay)
        {
            SpawnSnowBall(SnowballPositions[Random.Range(0, SnowballPositions.Length)].position);
            SnowBallAt = Time.time;
            SnowBalls--;
        }
    }

    public void SpawnSnowBall(Vector2 pos)
    {
        SnowballController snowball = UnityEngine.Object.Instantiate<SnowballController>(SnowballTemplate);
        snowball.transform.position = pos;
        snowball.gameObject.SetActive(true);
    }

    public void ThrowSnowBalls()
    {
        SnowBallAt = Time.time;
        this.IsIdle = false;
        this.IsHurt = false;
        this.IsSmashing = true;
        SnowBalls = 8;
        UpdateSprite();
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        IceBlock ib = other.gameObject.GetComponent<IceBlock>();
        PlayerCollider pc = other.gameObject.GetComponent<PlayerCollider>();
        if (ib != null && (IsIdle || IsSmashing))
        {
            DoHurt();
        } else if (ib != null && (IsJumping || IsLanding))
        {
            UnityEngine.Object.Destroy(ib.gameObject);
        } else if (pc != null && !IsHurt)
        {
            if (pc.Player.DamageBoostStartAt < 0)
                pc.Player.TakeHit(this.gameObject, 1, 6);
        }

        
    }

    public void DoIdle()
    {
        IsIdle = true;
        IsSmashing = false;
        IsHurt = false;
        IsJumping = false;
        IsLanding = false;
        IdleAt = Time.time;
        UpdateSprite();
    }

    public void DoHurt()
    {

        IsIdle = false;
        IsSmashing = false;
        IsHurt = true;

        IsJumping = false;
        IsLanding = false;
        HurtAt = Time.time;
        _KnockBackOnHit.IsEnabled = false;
        _EnemyController.IsInvulnerable = false;
        UpdateSprite();
    }

    public void UpdateSprite()
    {
        Idle.SetActive(IsIdle);
        Hurt.SetActive(IsHurt);
        Smash.SetActive(IsSmashing);
        Jump.SetActive(IsJumping);
        Fall.SetActive(IsLanding);
    }

}
