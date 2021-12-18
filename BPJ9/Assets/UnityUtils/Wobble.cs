namespace CaptainCoder.Unity
{

    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    
    [RequireComponent(typeof(RectTransform))]
    public class Wobble : MonoBehaviour
    {

        private RectTransform rect;
        public float Magnitude = 10;
        public float Speed = 5;

        void Start()
        {
            if (rect == null)
            {
                rect = this.gameObject.GetComponent<RectTransform>();
            }
        }
        // Update is called once per frame
        void Update()
        {
            rect.localPosition = new Vector2(0, Mathf.Sin(Time.time * Speed) * Magnitude);
        }
    }
}