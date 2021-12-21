#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[RequireComponent(typeof(BaseMap))]
public class BaseMapManager : MonoBehaviour
{
    public BaseMap Map => this.GetComponent<BaseMap>();
    public TextAsset MapFile;
}

[CustomEditor(typeof(BaseMapManager))]
public class BaseMapManagerEditor : Editor
{

    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        BaseMapManager manager = (BaseMapManager)target;
        EditorGUILayout.ObjectField("Base Map", manager.Map, typeof(BaseMap), false);

        manager.MapFile = (TextAsset)EditorGUILayout.ObjectField("Map File", manager.MapFile, typeof(TextAsset), false);
        
        if (manager.MapFile != null)
        {
            manager.Map.MapData = manager.MapFile.text;
        }

        if(GUILayout.Button("Build Map"))
        {
            manager.Map.BuildMap();
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