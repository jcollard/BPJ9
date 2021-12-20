using System;
using UnityEngine;

public class CollisionDirection
{
    public float X, Y;

    public bool IsRight => X < 0 && Mathf.Abs(X) > Mathf.Abs(Y);
    public bool IsLeft => X > 0 && Mathf.Abs(X) > Mathf.Abs(Y);
    public bool IsDown => Y > 0 && Mathf.Abs(Y) > Mathf.Abs(X);
    public bool IsUp => Y < 0 && Mathf.Abs(Y) > Mathf.Abs(X);

    public float Magnitude => Mathf.Abs(X) + Mathf.Abs(Y);

    public CollisionDirection(GameObject pushing, GameObject checking)
    {
        Vector2 direction = pushing.transform.position - checking.transform.position;
        this.X = direction.x;
        this.Y = direction.y;
    }

}