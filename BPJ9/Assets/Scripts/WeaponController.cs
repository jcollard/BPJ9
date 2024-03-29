using System.Collections;
using System.Collections.Generic;
using CaptainCoder.Unity.GameObjectExtensions;
using UnityEngine;

public class WeaponController : MonoBehaviour
{
    public PlayerController Player;    
    private float StartAt = -1;
    private float Duration = .15f;
    private float _Damage = 1f;
    public float Damage => _Damage;
    public Vector2 StartPosition;
    public Vector2 EndPosition;
    private bool IsLoaded = false;

    public IceEffect IceEffectTemplate;
    public FireEffect FireEffectTemplate;

    private void Init()
    {
        if(IsLoaded) return;
        this.StartPosition = this.transform.localPosition;
        this.EndPosition = this.gameObject.transform.up * .5f;
        this.EndPosition.x += StartPosition.x;
        this.EndPosition.y += StartPosition.y;
        IsLoaded = true;
    }

    public System.Action<WeaponController> OnEndAnimation;

    public void Update()
    {
        if (StartAt < 0) return;
        float HalfDuration = Duration/2;
        float lungePercent = (Time.time - StartAt) / HalfDuration;
        if (lungePercent < 1)
        {
            this.gameObject.SetLocalPosition2D(Vector2.Lerp(StartPosition, EndPosition, lungePercent));
        }
        else
        {
            
            float retractPercent = (Time.time - (StartAt + HalfDuration)) / HalfDuration;
            this.gameObject.SetLocalPosition2D(Vector2.Lerp(EndPosition, StartPosition, retractPercent));
        }
        
        
        
        if (Time.time > (StartAt + Duration))
        {
            StartAt = -1;
            EndAnimation();
        }
    }

    public void StartAnimation()
    {
        Init();
        SoundController.PlayRandomSFX("Attack 1", "Attack 2");
        this.StartAt = Time.time;
        this.gameObject.SetLocalPosition2D(StartPosition);
        this.gameObject.SetActive(true);
        if (PlayerController.Instance.CanAbsorb)
        {
            CreateEffect(PlayerController.Instance.CurrentPower);
        }
    }

    public void EndAnimation()
    {
        if (OnEndAnimation != null) OnEndAnimation(this);
        this.gameObject.SetActive(false);
    }

    public void CreateEffect(PowerType pt)
    {
        if (pt == PowerType.Ice)
        {
            IceEffect ie = UnityEngine.Object.Instantiate<IceEffect>(IceEffectTemplate);
            ie.transform.position = IceEffectTemplate.transform.position;
            ie.dir = PlayerController.Instance.Direction;
            ie.direction = PlayerController.Instance.CurrentFacing;
            ie.gameObject.SetActive(true);
        }
        if (pt == PowerType.Fire)
        {
            FireEffect ie = UnityEngine.Object.Instantiate<FireEffect>(FireEffectTemplate);
            ie.transform.position = FireEffectTemplate.transform.position;
            ie.dir = PlayerController.Instance.Direction;
            ie.direction = PlayerController.Instance.CurrentFacing;
            ie.gameObject.SetActive(true);
        }
    }
}
