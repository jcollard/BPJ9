using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaptainCoder.Unity;
using UnityEditor;
using UnityEditor.SceneManagement;

#if UNITY_EDITOR

[RequireComponent(typeof(ShapeBuilder))]
public class ShapeBuilderManager : MonoBehaviour
{
    public ShapeBuilder builder => this.GetComponent<ShapeBuilder>(); 
}

[CustomEditor(typeof(ShapeBuilderManager))]
public class ShapeBuilderManagerEditor : Editor 
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck(); 
        ShapeBuilderManager manager = (ShapeBuilderManager)target;
        EditorGUILayout.ObjectField("Shape Builder", manager.builder, typeof(ShapeBuilder), false);

        EditorGUILayout.LabelField("Shape Editor");
        manager.builder.Shape = EditorGUILayout.TextArea(manager.builder.Shape, UnityEditorUtils.MonoSpacedTextArea);

        if(GUILayout.Button("Rebuild Grid"))
        {
            manager.builder.BuildShape();
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