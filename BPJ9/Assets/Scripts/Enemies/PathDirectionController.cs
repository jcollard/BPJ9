using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(PathController))]
public class PathDirectionController : MonoBehaviour
{
    private PathController PathController => this.GetComponent<PathController>();

    public GameObject North, East, South, West;

    private GameObject[] _Directions;
    private GameObject[] Directions => CaptainCoder.Utils.MemoizeField(ref _Directions, new GameObject[]{North, East, South, West});

    public void Start()
    {
        PathController.OnNextWayPoint = this.CalcNewDirection;
    }   

    private void CalcNewDirection(PathController pc)
    {
        Vector2 direction = pc.TargetWayPoint - pc.PreviousWayPoint;
        if (direction.x < 0) SetDirection(West);
        else if (direction.x > 0) SetDirection(East);
        else if (direction.y < 0) SetDirection(South);
        else if (direction.y > 0) SetDirection(North);
    }

    private void SetDirection(GameObject dir)
    {
        foreach(GameObject obj in Directions)
            obj.SetActive(obj == dir);
    }
    
}