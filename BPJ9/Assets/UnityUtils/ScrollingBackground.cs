namespace CaptainCoder.Unity
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class ScrollingBackground : MonoBehaviour
    {
        public float scrollSpeed;
        public float BobbingSpeed;
        public float BobbingMagnitude;
        private Renderer Renderer;

        void Start()
        {
            this.Renderer = GetComponent<Renderer>();
        }

        void Update()
        {
            float x = Mathf.Repeat(Time.time * scrollSpeed, 1);
            float y = Mathf.Sin(Time.time * BobbingSpeed) * BobbingMagnitude;
            Vector2 offset = new Vector2(x, y);
            this.Renderer.sharedMaterial.SetTextureOffset("_MainTex", offset);
        }
    }
}