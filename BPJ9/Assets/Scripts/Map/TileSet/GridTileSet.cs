using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridTileSet : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private List<GridTile> _Tiles;
    public List<GridTile> Tiles 
    {
        get => _Tiles;
        set
        {
            this._Tiles = value;
            this.BuildDictionary();
        }
    }

    [SerializeField, ReadOnly]
    private List<GridTile> _Floors;
    public List<GridTile> Floors
    {
        get
        {
            if (this._Floors == null)
            {
                this.BuildDictionary();
            }
            return _Floors;
        }
    }
    /// <summary>
    /// A Dictionary from encoded tile bits to a list of GridTiles meeting that criteria
    /// </summary>
    private Dictionary<int, List<GridTile>> _TileLookup;
    public Dictionary<int, List<GridTile>> TileLookup 
    {
        get {
            if (this._TileLookup == null)
            {
                this.BuildDictionary();
            }
            return this._TileLookup;
        }
    }

    private void BuildDictionary()
    {
        this._TileLookup = new Dictionary<int, List<GridTile>>();
        this._Floors = new List<GridTile>();
        foreach (GridTile t in Tiles)
        {
            if (!this._TileLookup.ContainsKey(t.Criteria)) this._TileLookup[t.Criteria] = new List<GridTile>();
            this._TileLookup[t.Criteria].Add(t);
            if (t.IsFloor) this._Floors.Add(t);
        }
    }
    
}
