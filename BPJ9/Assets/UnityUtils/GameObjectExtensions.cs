namespace CaptainCoder.Unity.GameObjectExtensions
{
    using UnityEngine;

    public static class GameObjectExtensions
    {

        public static void SetPosition2D(this GameObject input, Vector2 newPosition)
        {
            Vector3 v3 = newPosition;
            v3.z = input.transform.position.z;
            input.transform.position = v3;
        }

    }
}