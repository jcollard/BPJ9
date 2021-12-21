#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[RequireComponent(typeof(MapTile))]
public class MapTileManager : MonoBehaviour
{
    public MapTile Tile => this.GetComponent<MapTile>();
}

[CustomEditor(typeof(MapTileManager))]
public class MapTileManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        MapTileManager manager = (MapTileManager)target;
        EditorGUILayout.ObjectField("Map Tile", manager.Tile, typeof(MapTile), false);


        if (EditorGUI.EndChangeCheck())
        {
            // This code will unsave the current scene if there's any change in the editor GUI.
            // Hence user would forcefully need to save the scene before changing scene
            EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
        }
    }
}

#endif