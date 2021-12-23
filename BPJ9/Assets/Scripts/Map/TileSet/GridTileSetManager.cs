#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
using CaptainCoder.Unity;
using System.Linq;

[RequireComponent(typeof(GridTileSet))]
public class GridTileSetManager : MonoBehaviour
{
    public GridTileSet Model => this.GetComponent<GridTileSet>();
    public Transform FloorTilesDemo;
    public Transform WallTilesContainer;
    public Transform DemoConfigContainer;

    public void BuildAllWalls()
    {
        for (int row = 0; row < 16; row++)
        {
            for (int col = 0; col < 16; col++)
            {
                int encoding = (row * 16) + col;
                if (!this.Model.TileLookup.ContainsKey(encoding))
                {
                    WallTile tile = UnityEngine.Object.Instantiate<WallTile>(this.Model.DefaultWall);
                    tile.name = $"Wall ({encoding})";
                    HashSet<NeighborSpace> neighbors = NeighborSpaceUtil.DecodeSet(encoding);
                    tile.CriteriaSet = neighbors;
                    tile.transform.parent = WallTilesContainer;
                    tile.transform.localPosition = new Vector2(col, row);
                    this.Model.TileLookup[encoding] = tile;
                }
            }
        }
        this.Model.Tiles = this.Model.TileLookup.Values.ToList();
    }

    public void BuildConfigDemo()
    {
        UnityEngineUtils.Instance.DestroyChildren(DemoConfigContainer);
        int Width = 4;
        int Height = 4;
        for (int row = 0; row < 16; row++)
        {
            for (int col = 0; col < 16; col++)
            {
                int encoding = (row * 16) + col;
                Transform configDemo = new GameObject($"Config ({encoding})").transform;
                configDemo.localPosition = new Vector2(col * Width, row * Height);
                configDemo.parent = DemoConfigContainer;

                WallTile tile = UnityEngine.Object.Instantiate<WallTile>(this.Model.TileLookup[encoding]);
                tile.name = $"WallTile ({encoding})";                
                tile.transform.parent = configDemo;
                tile.transform.localPosition = new Vector2();

                List<WallTilePlaceHolder> placeHolders = new List<WallTilePlaceHolder>();
                foreach (NeighborSpace neighbor in NeighborSpaceUtil.DecodeSet(encoding))
                {
                    GameObject obj = new GameObject("PlaceHolder");
                    obj.transform.parent = configDemo;
                    (int offX, int offY) = NeighborSpaceUtil.ReverseSpaceLookup[neighbor];
                    obj.transform.localPosition = new Vector2(offX, offY);

                    WallTilePlaceHolder placeHolder = obj.AddComponent<WallTilePlaceHolder>();
                    placeHolder.TileSet = this.Model;
                    placeHolders.Add(placeHolder);
                }

                foreach (WallTilePlaceHolder placeHolder in placeHolders)
                {
                    placeHolder.UpdateWallTile();
                }

            }
        }
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

        manager.WallTilesContainer = (Transform)EditorGUILayout.ObjectField("Wall Tiles", manager.WallTilesContainer, typeof(Transform), true);
        manager.DemoConfigContainer = (Transform)EditorGUILayout.ObjectField("Config Demo", manager.DemoConfigContainer, typeof(Transform), true);
        manager.FloorTilesDemo = (Transform)EditorGUILayout.ObjectField("Floor Tiles Demo", manager.FloorTilesDemo, typeof(Transform), true);

        if (GUILayout.Button("Build All Walls"))
        {
            manager.BuildAllWalls();
        }

        if (GUILayout.Button("Build Config Demo"))
        {
            manager.BuildConfigDemo();
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