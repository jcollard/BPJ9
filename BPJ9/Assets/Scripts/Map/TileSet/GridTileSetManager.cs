#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;

[RequireComponent(typeof(GridTileSet))]
public class GridTileSetManager : MonoBehaviour
{
    public GridTileSet Model => this.GetComponent<GridTileSet>();

    public void DiscoverTiles()
    {
        List<GridTile> tiles = this.DiscoverTiles(new List<GridTile>(), this.transform);
        Model.Tiles = tiles;
    }
    private List<GridTile> DiscoverTiles(List<GridTile> acc, Transform toScan)
    {
        for (int ix = 0; ix < toScan.childCount; ix++)
            DiscoverTiles(acc, toScan.GetChild(ix));
        GridTile tile = toScan.GetComponent<GridTile>();
        if (tile != null) {
            acc.Add(tile);
            tile.GetComponent<GridTileManager>().DiscoverCriteria();
        }
        return acc;
    }
}

[CustomEditor(typeof(GridTileSetManager))]
public class GridTileSetManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        EditorGUI.BeginChangeCheck();
        GridTileSetManager manager = (GridTileSetManager)target;
        EditorGUILayout.ObjectField("Tile Set", manager.Model, typeof(GridTileSet), false);

        if (GUILayout.Button("Discover Tiles"))
        {
            manager.DiscoverTiles();
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