using UnityEngine;

[RequireComponent(typeof(MapTileManager))]
public class MapTile : MonoBehaviour
{
    public BaseMap Parent;
    public char TileType;
    public int Version;
}

