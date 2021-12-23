#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
using System.Linq;

[RequireComponent(typeof(SpriteLoader))]
public class SpriteLoaderManager : MonoBehaviour
{
    public SpriteLoader Model => this.GetComponent<SpriteLoader>();
}

[CustomEditor(typeof(SpriteLoaderManager))]
public class BaseSpriteLoaderManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        SpriteLoaderManager manager = (SpriteLoaderManager)target;
        EditorGUILayout.ObjectField("Sprite Loader", manager.Model, typeof(SpriteLoader), false);

        if (GUILayout.Button("Build Sets"))
        {
            manager.Model.BuildSets();
        }

        if (EditorGUI.EndChangeCheck())
        {
            // This code will unsave the current scene if there's any change in the editor GUI.
            // Hence user would forcefully need to save the scene before changing scene
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }

        var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
        if (prefabStage != null)
        {
            EditorSceneManager.MarkSceneDirty(prefabStage.scene);
        }
    }


}

#endif