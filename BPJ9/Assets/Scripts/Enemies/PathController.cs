using System.Collections.Generic;
using UnityEngine;
using CaptainCoder.Unity.GameObjectExtensions;

public class PathController : MonoBehaviour
{

    public List<Transform> WayPoints;
    private Queue<Transform> Queue;
    public Vector2 TargetWayPoint, PreviousWayPoint;
    public float Speed;
    private float StartAt = -1;
    private float TravelDuration;
    public bool PathOnStart = false;
    public bool Loop = false;
    public bool Finished = false;

    public System.Action<PathController> OnFinishPathing;
    public System.Action<PathController> OnNextWayPoint;

    public void Start()
    {
        if (PathOnStart) StartPathing();
    }


    public void Update()
    {
        if (StartAt < 0 || Finished) return;
        if (!UpdatePosition())
        {
            if (!GetNextWayPoint() && !Finished)
            {
                this.FinishPathing();
            }
        }

    }

    public void StartPathing()
    {
        Queue = new Queue<Transform>(WayPoints);
        TargetWayPoint = Queue.Dequeue().position;
        Vector2 startPosition = TargetWayPoint;
        this.gameObject.SetPosition2D(startPosition);
        GetNextWayPoint();
    }

    private void FinishPathing()
    {
        if (this.OnFinishPathing != null)
        {
            this.OnFinishPathing(this);
            Finished = true;
        }
        else
        {
            Finished = true;
            this.gameObject.SetActive(false);
            UnityEngine.Object.Destroy(this.gameObject);
        }
    }

    /// <summary>
    /// Updates the position of this game object returning true if it has finished moving and false otherwise.
    /// </summary>
    /// <returns></returns>
    private bool UpdatePosition()
    {
        float percent = (Time.time - StartAt) / TravelDuration;
        Vector2 newPosition = Vector2.Lerp(PreviousWayPoint, TargetWayPoint, percent);
        this.gameObject.SetPosition2D(newPosition);
        if (percent >= 1) return false;
        return true;
    }

    private bool GetNextWayPoint()
    {
        if (Queue.Count == 0)
        {
            if (Loop == false) return false;
            foreach(Transform t in WayPoints) Queue.Enqueue(t);
        } 
        Vector2 NextWayPoint = Queue.Dequeue().position;
        TravelDuration = Vector2.Distance(NextWayPoint, TargetWayPoint) / Speed;
        StartAt = Time.time;
        this.PreviousWayPoint = TargetWayPoint;
        TargetWayPoint = NextWayPoint;
        if (this.OnNextWayPoint != null) this.OnNextWayPoint(this);
        return true;
    }

}