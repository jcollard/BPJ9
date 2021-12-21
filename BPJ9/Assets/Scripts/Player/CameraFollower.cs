using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFollower : MonoBehaviour
{
    public Transform Target;
    public Camera Camera => this.GetComponent<Camera>();
    public Transform TopLeft, TopRight, BottomLeft, BottomRight;

    // Update is called once per frame
    void Update()
    {
        Bounds bounds = OrthographicBounds();
        Vector3 newPosition = Target.position;
        float MaxY = TopLeft.position.y - bounds.extents.y;
        float MinY = BottomLeft.position.y + bounds.extents.y;
        float MaxX = TopRight.position.x - bounds.extents.x;
        float MinX = TopLeft.position.x + bounds.extents.x;
        newPosition.y = Mathf.Clamp(newPosition.y, MinY, MaxY);
        newPosition.x = Mathf.Clamp(newPosition.x, MinX, MaxX);
        newPosition.z = this.transform.position.z;
        this.transform.position = newPosition;
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

}
