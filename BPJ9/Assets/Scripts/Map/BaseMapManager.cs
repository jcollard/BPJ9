#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.Linq;

[RequireComponent(typeof(BaseMap))]
public class BaseMapManager : MonoBehaviour
{
    public (int min, int max) Rows;
    public (int min, int max) Cols;
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

        EditorGUILayout.Separator();
        EditorGUILayout.BeginHorizontal();
        manager.Rows.min = EditorGUILayout.IntField("Min Row", manager.Rows.min);
        manager.Cols.min = EditorGUILayout.IntField("Min Col", manager.Cols.min);
        EditorGUILayout.EndHorizontal();
        EditorGUILayout.BeginHorizontal();
        manager.Rows.max = EditorGUILayout.IntField("Max Row", manager.Rows.max);
        manager.Cols.max = EditorGUILayout.IntField("Max Col", manager.Cols.max);
        EditorGUILayout.EndHorizontal();
        
        if (GUILayout.Button("Test Diff From Center"))
        {
            GridBounds gb = new GridBounds((2, 2), 0, 0);
            GridBounds surrounding = new GridBounds(gb, 4);
            Debug.Log($"Inner: {gb}");
            Debug.Log($"Outer: {surrounding}"); 
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            (int, int)[] ps = gb.DifferenceFromCenter(surrounding, gb.Center).ToArray();
            Debug.Log($"Diff: {ps.Length}");
            foreach((int, int) pos in ps)
            {
                sb.Append($"{pos} ");
            }
            Debug.Log(sb.ToString());
        }
        
        if(GUILayout.Button("Build Chunk"))
        {
            manager.Map.DestroyMap();
            manager.Map.Init();
            GridBounds bounds = new GridBounds(manager.Rows.max, manager.Cols.max, manager.Rows.min, manager.Cols.min);
            manager.Map.Chunker.BuildNextChunk(bounds);
        }

        EditorGUILayout.Separator();

        if(GUILayout.Button("Build Current Chunk"))
        {
            manager.Map.DestroyMap();
            manager.Map.Init();
            GridBounds bounds = new GridBounds(manager.Rows.max, manager.Cols.max, manager.Rows.min, manager.Cols.min);
            manager.Map.Chunker.BuildNextChunk();
        }

        EditorGUILayout.Separator();

        if(GUILayout.Button("Build Entire Map"))
        {
            manager.Map.DestroyMap();
            manager.Map.Init();
            manager.Map.Chunker.BuildNextChunk(manager.Map.Chunker.MapBounds);
        }

        if (GUILayout.Button("Destroy Map"))
        {
            manager.Map.DestroyMap();
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