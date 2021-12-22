#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;
using CaptainCoder.Unity;

[RequireComponent(typeof(GridTileSet))]
public class GridTileSetManager : MonoBehaviour
{
    public GridTileSet Model => this.GetComponent<GridTileSet>();
    public Transform FloorTilesDemo;

    [SerializeField]
    private bool _ShowNonGridTiles = true;
    public bool ShowNonGridTiles
    {
        get => _ShowNonGridTiles;
        set
        {
            _ShowNonGridTiles = value;
            this.ToggleNonGridTiles(this.transform);
        }
    }

    public void DiscoverTiles()
    {
        UnityEngineUtils.Instance.DestroyChildren(FloorTilesDemo);
        List<GridTile> tiles = this.DiscoverTiles(new List<GridTile>(), this.transform);
        Model.Tiles = tiles;
    }
    private List<GridTile> DiscoverTiles(List<GridTile> acc, Transform toScan)
    {
        for (int ix = 0; ix < toScan.childCount; ix++)
            DiscoverTiles(acc, toScan.GetChild(ix));
        GridTile tile = toScan.GetComponent<GridTile>();
        if (tile != null)
        {
            acc.Add(tile);
            tile.GetComponent<GridTileManager>().DiscoverCriteria();
        }
        return acc;
    }

    private void ToggleNonGridTiles(Transform toScan)
    {
        if (toScan.childCount > 0)
        {
            for (int ix = 0; ix < toScan.childCount; ix++)
                ToggleNonGridTiles(toScan.GetChild(ix));
        }
        else if (toScan.GetComponent<GridTile>() == null)
        {
            toScan.gameObject.SetActive(this.ShowNonGridTiles);
        }
    }

    public void ToggleFloorTiles()
    {
        if (FloorTilesDemo.childCount > 0)
        {
            UnityEngineUtils.Instance.DestroyChildren(FloorTilesDemo);
        }
        else
        {
            Vector2 Min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
            Vector2 Max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
            this.DiscoverWidthAndHeight(this.transform, ref Min, ref Max);
            Min.x -= 1;
            Min.y -= 1;
            Max.x += 1;
            Max.y += 1;
            for (int x = (int)Min.x; x <= Max.x; x++)
            {
                for (int y = (int)Min.y; y <= Max.y; y++)
                {
                    GridTile tile = UnityEngine.Object.Instantiate<GridTile>(this.Model.Floors[Random.Range(0, this.Model.Floors.Count)]);
                    tile.transform.parent = FloorTilesDemo;
                    tile.transform.position = new Vector3(x, y, FloorTilesDemo.position.z);
                }
            }
        }
    }

    private void DiscoverWidthAndHeight(Transform toScan, ref Vector2 Min, ref Vector2 Max)
    {
        Min.x = Mathf.Min(toScan.position.x, Min.x);
        Min.y = Mathf.Min(toScan.position.y, Min.y);
        Max.x = Mathf.Max(toScan.position.x, Max.x);
        Max.y = Mathf.Max(toScan.position.y, Max.y);
        for (int ix = 0; ix < toScan.childCount; ix++)
            DiscoverWidthAndHeight(toScan.GetChild(ix), ref Min, ref Max);
    }

    public void UpdatePlaceHolderSprites()
    {

    }

    private void UpdatePlaceHolderSprites(Transform toScan)
    {
        for (int ix = 0; ix < toScan.childCount; ix++)
            UpdatePlaceHolderSprites(toScan.GetChild(ix));

        // Ignore non GridTiles
        if (toScan.GetComponent<GridTile>() != null) return;

        // Ignore elements without a SpriteRenderer
        SpriteRenderer renderer = toScan.GetComponent<SpriteRenderer>();
        if (renderer == null) return;

        Transform parent = renderer.transform.parent;
        for (int ix = 0; ix < parent.childCount; ix++)
        {
            Transform siblingTransform = parent.GetChild(ix);
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
        manager.FloorTilesDemo = (Transform)EditorGUILayout.ObjectField("Floor Tiles Demo", manager.FloorTilesDemo, typeof(Transform), true);
        manager.ShowNonGridTiles = EditorGUILayout.Toggle("Show Non Grid Tiles", manager.ShowNonGridTiles);

        if (GUILayout.Button("Discover Tiles"))
        {
            manager.DiscoverTiles();
        }

        if (GUILayout.Button("Toggle Floor Tiles"))
        {
            manager.ToggleFloorTiles();
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