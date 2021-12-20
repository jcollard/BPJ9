using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerController : MonoBehaviour
{

    public Layer TargetLayer;
    private int TargetLayerInt => -(int)TargetLayer;

    public void SetLayer()
    {
        if (this.transform.position.z == TargetLayerInt)
            return;
        Vector3 newPosition = this.transform.position;
        newPosition.z = TargetLayerInt;
        this.transform.position = newPosition;
    }

    void OnAwake()
    {
        SetLayer();
    }

    void OnStart()
    {
        SetLayer();
    }

    void OnEnable()
    {
        SetLayer();
    }
}

public enum Layer
{
    Ground,
    OnGround,
    Blocks,
    Walls,
    Player,
}