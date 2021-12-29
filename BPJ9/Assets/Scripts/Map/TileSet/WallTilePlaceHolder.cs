using CaptainCoder.Unity;
using UnityEngine;

public class WallTilePlaceHolder : MonoBehaviour
{
    public GridTileSet TileSet;

    public void Start()
    {
        this.ReplaceWithWallTile();
    }

    public void UpdateWallTile()
    {
        UnityEngineUtils.Instance.DestroyChildren(this.transform);
        int criteria = NeighborSpaceUtil.DiscoverEncodedCriteria(this.transform);
        WallTile tile = UnityEngine.Object.Instantiate<WallTile>(TileSet.TileLookup[criteria]);
        tile.transform.parent = this.transform;
        tile.transform.localPosition = new Vector2();
    }

    public GameObject ReplaceWithWallTile()
    {
        int criteria = NeighborSpaceUtil.DiscoverEncodedCriteria(this.transform);
        WallTile tile = UnityEngine.Object.Instantiate<WallTile>(TileSet.TileLookup[criteria]);
        tile.transform.parent = this.transform.parent;
        tile.transform.position = this.transform.position;
        tile.name = this.name;
        tile.gameObject.SetActive(true);
        System.Func<GameObject, bool> f = (x) => {UnityEngine.Object.Destroy(x); return true;};
        #if UNITY_EDITOR
        f = (x) => {UnityEngine.Object.DestroyImmediate(this.gameObject); return true;};
        #endif
        f.Invoke(this.gameObject);
        return tile.gameObject;
    }

}