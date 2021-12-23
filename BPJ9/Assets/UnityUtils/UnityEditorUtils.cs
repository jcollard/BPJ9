#if UNITY_EDITOR
namespace CaptainCoder.Unity
{
    using UnityEngine;
    using UnityEditor;
    using System.Collections.Generic;
    using System.Linq;
    using System;

    public class UnityEditorUtils
    {
        public static readonly UnityEditorUtils Instance = new UnityEditorUtils();

        private UnityEditorUtils() { }

        private static readonly Dictionary<Color, GUIStyle> ColorLabels = new Dictionary<Color, GUIStyle>();
        private static GUIStyle _MonoSpacedTextArea;

        /// <summary>
        /// Builds a GUIStyle for a Label setting the color of the label to the specified color.
        /// </summary>
        /// <param name="color">The color of the text in the GUIStyle.</param>
        /// <returns></returns>
        public static GUIStyle GetColorLabel(Color color)
        {
            if (!ColorLabels.TryGetValue(color, out GUIStyle style))
            {
                style = new GUIStyle(EditorStyles.label);
                style.normal.textColor = color;
            }

            return style;
        }

        /// <summary>
        /// A GUIStyle for a MonoSpacedTextArea. This assumes there is a font file
        /// named "Fonts/MonoSpaced" within your Resources folder.
        /// </summary>
        public static GUIStyle MonoSpacedTextArea
        {
            get
            {
                if (_MonoSpacedTextArea == null)
                {
                    _MonoSpacedTextArea = new GUIStyle(EditorStyles.textArea);
                    Font f = Resources.Load<Font>("Fonts/MonoSpaced");
                    if (f == null)
                    {
                        Debug.LogWarning("Attempted to load font from 'Resources/Fonts/MonoSpaced' but no file was found. Defaulting to system font.");
                    }
                    else
                    {
                        _MonoSpacedTextArea.font = f;
                        _MonoSpacedTextArea.fontSize = 18;
                    }
                }
                return _MonoSpacedTextArea;
            }
        }

        public void CenterSceneCameraOn(GameObject target, float zoom = -1)
        {
            // EditorGUIUtility.PingObject(target);
            var currentlActive = Selection.activeGameObject;
            Selection.activeGameObject = target;
            SceneView.lastActiveSceneView.FrameSelected();
            if (zoom > 0)
            {
                SceneView.lastActiveSceneView.size = zoom;
            }
            Selection.activeGameObject = currentlActive;
        }
    }

}
#endif