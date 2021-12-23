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
            System.Func<Transform, Transform> ShowTile = toScan =>
            {
                if (toScan.GetComponent<GridTile>() == null)
                {
                    toScan.gameObject.SetActive(this._ShowNonGridTiles);
                }
                return toScan;
            };
            ToggleTiles(this.Model.transform, ShowTile);
        }
    }

    private bool _ShowMirrors = true;
    public bool ShowMirrors
    {
        get => _ShowMirrors;
        set
        {
            _ShowMirrors = value;
            System.Func<Transform, Transform> ShowTile = toScan =>
            {
                GridTile tile = toScan.GetComponent<GridTile>();
                if (tile != null && tile.IsMirror)
                {
                    toScan.gameObject.SetActive(this._ShowMirrors);
                }
                return toScan;
            };
            ToggleTiles(this.Model.transform, ShowTile);
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

    private void ToggleTiles(Transform toScan, System.Func<Transform, Transform> action)
    {
        if (toScan.childCount > 0)
        {
            for (int ix = 0; ix < toScan.childCount; ix++)
                ToggleTiles(toScan.GetChild(ix), action);
        }
        else
        {
            action(toScan);
        }
    }

    // private void ToggleNonGridTiles(Transform toScan) => ToggleTiles(toScan, (toScan => toScan.GetComponent<GridTile>() == null && this.ShowNonGridTiles));


    // private void ToggleNonGridTiles(Transform toScan)
    // {
    //     if (toScan.childCount > 0)
    //     {
    //         for (int ix = 0; ix < toScan.childCount; ix++)
    //             ToggleNonGridTiles(toScan.GetChild(ix));
    //     }
    //     else if (toScan.GetComponent<GridTile>() == null)
    //     {
    //         toScan.gameObject.SetActive(this.ShowNonGridTiles);
    //     }
    // }

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
        this.UpdatePlaceHolderSprites(this.Model.transform);
    }

    public void DiscoverMirrors()
    {
        // Create a lookup dictionary of all of the sprites to a tile that should be the "Mirror"
        Dictionary<Sprite, GridTile> spriteLookup = new Dictionary<Sprite, GridTile>();

        int mainTiles = 0;
        // First pass we mark all of the "main" tiles
        foreach (GridTile tile in this.Model.Tiles)
        {
            Sprite asRenderer = tile.GetComponent<SpriteRenderer>().sprite;
            // If this is the first time we are seeing this sprite 
            // on a non-mirror, this GridTile becomes the main GridTile for this sprite
            if (!spriteLookup.ContainsKey(asRenderer) && !tile.IsMirror)
            {
                spriteLookup[asRenderer] = tile;
                mainTiles++;
                continue;
            }
        }

        int mirrorTiles = 0;
        // Second pass we make all of the duplicates Mirrors
        foreach (GridTile tile in this.Model.Tiles)
        {
            Sprite asRenderer = tile.GetComponent<SpriteRenderer>().sprite;
            if (!spriteLookup.ContainsKey(asRenderer))
            {
                Debug.Log($"Discovered an unused sprite {asRenderer.name}", tile);
                throw new System.Exception($"Discovered an unused sprite {asRenderer.name}");
            }

            GridTile mainTile = spriteLookup[asRenderer];
            // Main tile doesn't need updated.
            if (mainTile == tile) continue;
            // Otherwise, we set the Mirror to the main tile
            tile.MirrorTile = mainTile;
            mirrorTiles++;
        }

        List<Transform> mirrors = this.DiscoverMirrors(this.transform, spriteLookup, new List<Transform>());
        Debug.Log($"Found {mainTiles} main tiles.");
        Debug.Log($"Found {mirrorTiles} mirror tiles.");
        Debug.Log($"Created {mirrors.Count} new mirror tiles.");
    }

    public List<Transform> DiscoverMirrors(Transform toScan, Dictionary<Sprite, GridTile> spriteLookup, List<Transform> transforms)
    {
        for (int ix = 0; ix < toScan.childCount; ix++)
            DiscoverMirrors(toScan.GetChild(ix), spriteLookup, transforms);

        // We ignore GridTiles as they should already be set up as mirrors
        if (toScan.GetComponent<GridTile>() != null) return transforms;

        SpriteRenderer asRenderer = toScan.GetComponent<SpriteRenderer>();
        // If we don't have a SpriteRenderer there is nothing to do.
        if (asRenderer == null) return transforms;

        Sprite sprite = asRenderer.sprite;
        // Throw an exception if we discover an unused sprite
        if (!spriteLookup.ContainsKey(sprite))
        {
            Debug.Log($"Discovered sprite that is not attached to a GridTile: {sprite.name}!", asRenderer);
            throw new System.Exception($"Discovered sprite that is not attached to a GridTile: {sprite.name}!");
        }

        // Update the tile to be a mirror
        GridTile mainTile = spriteLookup[sprite];
        GridTile asTile = asRenderer.gameObject.AddComponent<GridTile>();
        asRenderer.gameObject.AddComponent<GridTileManager>();
        asTile.MirrorTile = mainTile;
        transforms.Add(toScan);

        return transforms;
    }

    private void UpdatePlaceHolderSprites(Transform toScan)
    {
        for (int ix = 0; ix < toScan.childCount; ix++)
            UpdatePlaceHolderSprites(toScan.GetChild(ix));

        // Ignore elements without a SpriteRenderer
        SpriteRenderer renderer = toScan.GetComponent<SpriteRenderer>();
        if (renderer == null) return;


        GridTile asTile = toScan.GetComponent<GridTile>();
        if (asTile != null)
        {
            //Ignore regular GridTiles
            if (!asTile.IsMirror) return;

            // Update Mirror Grid Tile sprites to match their mirror
            renderer.sprite = asTile.MirrorTile.GetComponent<SpriteRenderer>().sprite;
            // TODO: Copy over box collider.
            return;
        }

        // Otherwise, get the encoding
        int encoding = NeighborSpaceUtil.DiscoverEncodedCriteria(renderer.transform);
        try
        {
            // And try to update the sprite
            GridTile tile = this.Model.GetGridTile(encoding);
            renderer.sprite = tile.GetComponent<SpriteRenderer>().sprite;
        }
        catch (System.Exception e)
        {
            HashSet<NeighborSpace> set = NeighborSpaceUtil.DiscoverCriteria(renderer.transform);
            string c = "Criteria was:";
            foreach (NeighborSpace s in set) c += $" {s} ";
            Debug.Log($"Failed to update Sprite on {renderer.gameObject.name} at {renderer.transform.position}", renderer);
            Debug.Log(c);
            throw e;
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
        manager.ShowMirrors = EditorGUILayout.Toggle("Show Mirror Tiles", manager.ShowMirrors);


        if (GUILayout.Button("Discover Tiles"))
        {
            manager.DiscoverTiles();
        }

        if (GUILayout.Button("Discover Mirrors"))
        {
            Debug.Log("Discovering Mirrors...");
            manager.DiscoverMirrors();
        }

        if (GUILayout.Button("Toggle Floor Tiles"))
        {
            manager.ToggleFloorTiles();
        }

        if (GUILayout.Button("Update Place Holder Sprites"))
        {
            // manager.UpdatePlaceHolderSprites();
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