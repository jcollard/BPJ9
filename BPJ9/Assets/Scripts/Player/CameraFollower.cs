using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollower : MonoBehaviour
{
    public HideIfPlayerUnder ToHide;
    public PlayerController Target;
    public MapChunker Chunker;
    private char CurrentRoom = (char)0;

    [SerializeField]
    private Transform _MapContainer;
    public Transform MapContainer
    {
        get => _MapContainer;

        set => _MapContainer = value;
    }

    public Camera Camera => this.GetComponent<Camera>();
    public Vector2 Max, Min;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Bounds bounds = OrthographicBounds();
        if (this.Chunker != null)
        {
            this.Chunker.SetSize(bounds);
            this.SetBounds();
            // if (this.Chunker.CheckAndBuildChunk() || !PlayerInCamera(bounds))
            // {
            //     this.SetBounds(this.DiscoverBounds(_MapContainer));
            // }
        }


            Bounds tcBounds = TopCorner(bounds);
            Debug.Log(tcBounds);
            Debug.Log(Target.transform.position);
            Vector2 ignoringZ = Target.transform.position;
            if (tcBounds.Contains(ignoringZ))
            {
                Debug.Log("In top corner");
                ToHide.StartFadeOut();
            }
            else
            {
                ToHide.StartFadeIn();
            }


        Vector3 newPosition = Target.transform.position;
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

    private bool PlayerInCamera(Bounds bounds)
    {
        return bounds.Contains(Target.transform.position);
    }

    private void SetBounds()
    {
        if (CurrentRoom != Target.CurrentRoom && MapChunker.Instance != null)
        {
            CurrentRoom = Target.CurrentRoom;
            (this.Min, this.Max) = MapChunker.Instance.GetRoomBounds(CurrentRoom);
            
            
            Bounds cBounds = OrthographicBounds();
            
            float roomWidth = this.Max.x - this.Min.x;
            float roomHeight = this.Max.y - this.Min.y;
            Vector2 roomCenter = new Vector2(this.Max.x - roomWidth/2, this.Max.y - roomHeight/2);
            float cameraWidth = cBounds.extents.x*2;
            float cameraHeight = cBounds.extents.y*2;

            // If the room is too small, center it
            if (roomWidth < cameraWidth)
            {
                this.Min.x = roomCenter.x - cameraWidth/2;
                this.Max.x = roomCenter.x + cameraWidth/2;
            }

            if (roomHeight < cameraHeight)
            {
                this.Min.y = roomCenter.y - cameraHeight/2;
                this.Max.y = roomCenter.y + cameraHeight/2;
            }

        }
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

    public Bounds TopCorner(Bounds b)
    {
        Vector2 center = new Vector2();
        center.x += (b.center.x + b.extents.x);
        center.y += (b.center.y + b.extents.y);
        Bounds bounds = new Bounds(center, new Vector2(5, 5));
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
