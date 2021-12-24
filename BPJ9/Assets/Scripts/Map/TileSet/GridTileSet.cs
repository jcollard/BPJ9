using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CaptainCoder.Unity;

public class GridTileSet : MonoBehaviour
{
    public SpriteLoader Sprites;

    [SerializeField]
    private List<WallTile> _Tiles;
    public List<WallTile> Tiles 
    {
        get => _Tiles;
        set
        {
            this._Tiles = value;
            this.BuildDictionary();
        }
    }

    public WallTile DefaultWall;
    

    public List<FloorTile> Floors;

    /// <summary>
    /// A Dictionary from encoded tile bits to a list of GridTiles meeting that criteria
    /// </summary>
    private Dictionary<int, WallTile> _TileLookup;
    public Dictionary<int, WallTile> TileLookup 
    {
        get {
            if (this._TileLookup == null)
            {
                this.BuildDictionary();
            }
            return this._TileLookup;
        }
    }

    public void BuildDictionary()
    {
        this._TileLookup = new Dictionary<int, WallTile>();
        foreach (WallTile t in Tiles)
        {
            if (this._TileLookup.ContainsKey(t.Criteria)) UnityEngineUtils.Instance.FailFast($"Duplicate criteria found: {t.Criteria}.", this);
            this._TileLookup[t.Criteria] = t;
        }

        // If we don't have a 0 tile, set it to the default wall
        if (!this._TileLookup.ContainsKey(0)) this._TileLookup[0] = DefaultWall;
    }
    
}
