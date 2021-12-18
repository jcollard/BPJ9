namespace CaptainCoder.Unity
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class AutoDelete : MonoBehaviour
    {
        public float Duration = 2;
        public float StartAt;


        // Update is called once per frame
        void Update()
        {
            if (Time.time < StartAt) return;

            float EndAt = StartAt + Duration;
            float percent = (EndAt - Time.time) / Duration;

            if (percent <= 0)
            {
                this.gameObject.SetActive(false);
                UnityEngine.GameObject.Destroy(this.gameObject);
            }
        }

        public void OnEnable()
        {
            this.StartAt = (Time.time + Duration);
            this.gameObject.SetActive(true);
        }
    }
}
