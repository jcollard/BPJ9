#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaptainCoder.Unity;
using UnityEditor;
using UnityEditor.SceneManagement;



[RequireComponent(typeof(GridBuilder))]
public class GridBuilderManager : MonoBehaviour
{
    public GridBuilder builder => this.GetComponent<GridBuilder>();
}

[CustomEditor(typeof(GridBuilderManager))]
public class GridBuilderManagerEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        GridBuilderManager manager = (GridBuilderManager)target;
        EditorGUILayout.ObjectField("Grid Builder", manager.builder, typeof(GridBuilder), false);

        if(GUILayout.Button("Rebuild Shape"))
        {
            manager.builder.BuildGrid();
        }

        if (EditorGUI.EndChangeCheck())
        {
            // This code will unsave the current scene if there's any change in the editor GUI.
            // Hence user would forcefully need to save the scene before changing scene
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}

#endif