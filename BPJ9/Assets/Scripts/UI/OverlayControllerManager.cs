#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using UnityEditor.SceneManagement;

[RequireComponent(typeof(OverlayController))]
public class OverlayControllerManager : MonoBehaviour
{
    public OverlayController Model => this.GetComponent<OverlayController>();
    public PlayerController Player;
}

[CustomEditor(typeof(OverlayControllerManager))]
public class OverlayControllerManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        OverlayControllerManager manager = (OverlayControllerManager)target;
        EditorGUILayout.ObjectField("Overlay Controller", manager.Model, typeof(OverlayControllerManager), false);

        manager.Player = (PlayerController)EditorGUILayout.ObjectField("Player",  manager.Player, typeof(PlayerController), true);

        if (manager.Player)
        {
            // EditorGUILayout.BeginHorizontal();
            manager.Player.HP = EditorGUILayout.FloatField("HP", manager.Player.HP);
            manager.Player.MaxHP = EditorGUILayout.IntField("Max HP", manager.Player.MaxHP);
            // EditorGUILayout.EndHorizontal();
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