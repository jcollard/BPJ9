using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollower : MonoBehaviour
{
    public Transform Target;
    public MapChunker Chunker;

    [SerializeField]
    private Transform _MapContainer;
    public Transform MapContainer
    {
        get => _MapContainer;

        set
        {
            if (_MapContainer == value) return;
            this.SetBounds(this.DiscoverBounds(value));
            _MapContainer = value;

        }
    }

    public Camera Camera => this.GetComponent<Camera>();
    public Vector2 Max, Min;

    void Start()
    {
        this.SetBounds(this.DiscoverBounds(_MapContainer));
    }

    // Update is called once per frame
    void Update()
    {
        Bounds bounds = OrthographicBounds();
        if (this.Chunker != null)
        {
            this.Chunker.SetSize(bounds);
            if (this.Chunker.CheckAndBuildChunk())
            {
                this.SetBounds(this.DiscoverBounds(_MapContainer));
            }
        }
        Vector3 newPosition = Target.position;
        // TODO: When a new chunk is built rediscover bounds
        float MaxY = Max.y - bounds.extents.y;
        float MinY = Min.y + bounds.extents.y;
        float MaxX = Max.x - bounds.extents.x;
        float MinX = Min.x + bounds.extents.x;
        newPosition.y = Mathf.Clamp(newPosition.y, MinY, MaxY);
        newPosition.x = Mathf.Clamp(newPosition.x, MinX, MaxX);
        newPosition.z = this.transform.position.z;
        this.transform.position = newPosition;

    }

    private void SetBounds((Vector2 Min, Vector2 Max) bounds)
    {
        this.Min = bounds.Min;
        this.Max = bounds.Max;
    }

    public Bounds OrthographicBounds()
    {
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float cameraHeight = this.Camera.orthographicSize * 2;
        Bounds bounds = new Bounds(
            this.Camera.transform.position,
            new Vector3(cameraHeight * screenAspect, cameraHeight, 0));
        return bounds;
    }

    private (Vector2, Vector2) DiscoverBounds(Transform MapContainer)
    {
        Vector2 Min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
        Vector2 Max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
        DiscoverBounds(MapContainer, ref Min, ref Max);
        return (Min, Max);
    }

    private void DiscoverBounds(Transform MapContainer, ref Vector2 Min, ref Vector2 Max)
    {
        for (int ix = 0; ix < MapContainer.childCount; ix++)
        {
            Transform child = MapContainer.GetChild(ix);
            if (child.childCount > 0)
            {
                DiscoverBounds(child, ref Min, ref Max);
            }
            else
            {
                Vector2 childPosition = child.position;
                Min.x = Mathf.Min(Min.x, childPosition.x);
                Min.y = Mathf.Min(Min.y, childPosition.y);
                Max.x = Mathf.Max(Max.x, childPosition.x);
                Max.y = Mathf.Max(Max.y, childPosition.y);
            }
        }
    }


}
