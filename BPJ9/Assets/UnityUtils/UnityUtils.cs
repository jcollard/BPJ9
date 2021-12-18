namespace CaptainCoder.Unity
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    public class UnityEngineUtils
    {
        public static readonly UnityEngineUtils Instance = new UnityEngineUtils();

        private UnityEngineUtils() {}

        /// <summary>
        /// Given a Transform, calls DestroyImmediate on each of the children
        /// contained in that transform.
        /// </summary>
        /// <param name="t">The container to empty.</param>
        public void DestroyChildren(Transform t)
        {
            List<Transform> children = t.Cast<Transform>().ToList();
            foreach (Transform child in children)
            {
                #if UNITY_EDITOR
                    UnityEngine.Object.DestroyImmediate(child.gameObject);
                #else
                    UnityEngine.Object.Destroy(child.gameObject);
                #endif
            }
        }

    }
}