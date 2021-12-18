namespace CaptainCoder.Unity
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEditor;

    [RequireComponent(typeof(CanvasGroup))]
    public class CanvasFader : MonoBehaviour
    {

        private CanvasGroup _group => this.GetComponent<CanvasGroup>();
        public float FadeDuration = 2;
        public float FadeDelay = 1;
        public float StartAt;


        // Update is called once per frame
        void Update()
        {
            if (Time.time < StartAt) return;

            float EndAt = StartAt + FadeDuration;
            float percent = (EndAt - Time.time) / FadeDuration;
            _group.alpha = percent;

            if (percent <= 0)
            {
                this.gameObject.SetActive(false);
            }
        }

        public void StartFade()
        {
            _group.alpha = 1;
            this.StartAt = (Time.time + FadeDelay);
            this.gameObject.SetActive(true);
        }
    }
}
