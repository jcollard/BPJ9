using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollower : MonoBehaviour
{
    public Transform Target;


    // Update is called once per frame
    void Update()
    {
        
        Vector3 newPosition = Target.position;
        newPosition.z = this.transform.position.z;
        this.transform.position = newPosition;
    }
}
