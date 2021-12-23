using CaptainCoder.Unity;
using UnityEngine;

public class WallTilePlaceHolder : MonoBehaviour
{
    public GridTileSet TileSet;

    public void UpdateWallTile()
    {
        UnityEngineUtils.Instance.DestroyChildren(this.transform);
        int criteria = NeighborSpaceUtil.DiscoverEncodedCriteria(this.transform);
        WallTile tile = UnityEngine.Object.Instantiate<WallTile>(TileSet.TileLookup[criteria]);
        tile.transform.parent = this.transform;
        tile.transform.localPosition = new Vector2();
    }

}