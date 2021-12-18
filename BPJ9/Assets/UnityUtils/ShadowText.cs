namespace CaptainCoder.Unity
{
    using UnityEngine;
    using System.Collections.Generic;
    using System.Linq;
    using System;
    using UnityEditor;

    [RequireComponent(typeof(UnityEngine.UI.Text))]
    public class ShadowText : MonoBehaviour
    {
        public Color ShadowColor = new Color(0, 0, 0, 255);
        public Color TextColor = new Color(255, 255, 255, 255);
        public Vector3 ShadowOffset = new Vector3(2, 5, 0);

        public void UpdateShadow()
        {
            Transform shadow = this.transform.Find("Shadow");
            if (shadow != null)
            {
                UnityEngine.Object.DestroyImmediate(shadow.gameObject);
            }
            GameObject obj = GameObject.Instantiate(this.gameObject);
            obj.name = "Shadow";
            UnityEngine.Object.DestroyImmediate(obj.GetComponent<ShadowText>());
            UnityEngine.UI.Text text = obj.GetComponent<UnityEngine.UI.Text>();
            obj.transform.SetParent(this.transform, false);
            Vector3 newPos = this.transform.position;
            
            RectTransform rect = obj.GetComponent<RectTransform>();
            rect.anchorMin = new Vector2(0f, 0f);
            rect.anchorMax = new Vector2(1f, 1f);
            rect.localPosition = ShadowOffset;
            text.color = TextColor;
            this.GetComponent<UnityEngine.UI.Text>().color = ShadowColor;

            

        }

    }

#if UNITY_EDITOR

    [CustomEditor(typeof(ShadowText))]
    public class ShadowTextEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();

            ShadowText shadowText = (ShadowText)target;

            shadowText.TextColor = EditorGUILayout.ColorField("Text Color", shadowText.TextColor);
            shadowText.ShadowColor = EditorGUILayout.ColorField("Shadow Color", shadowText.ShadowColor);
            shadowText.ShadowOffset = EditorGUILayout.Vector2Field("Offset", shadowText.ShadowOffset);

            // Clicking invokes a change so the UpdateShadow will be called.
            GUILayout.Button("Update Shadow");

            if (EditorGUI.EndChangeCheck())
            {
                shadowText.UpdateShadow();
            }
        }
    }

#endif
}