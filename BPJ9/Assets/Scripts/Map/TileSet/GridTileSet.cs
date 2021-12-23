using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

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

    public GridTile GetGridTile(int encoding, System.Random RNG = null)
    {
        if (this.TileLookup.TryGetValue(encoding, out List<GridTile> options))
        {
            int ix = RNG == null ? Random.Range(0, options.Count) : RNG.Next(0, options.Count);
            return options[ix];
        }
        throw new System.Exception($"No such GridTile with encoding {encoding} in {this.gameObject.name}.");
    }

    public bool TryGetGridTile(int encoding, out GridTile result, System.Random RNG = null)
    {
        if (this.TileLookup.TryGetValue(encoding, out List<GridTile> options))
        {
            int ix = RNG == null ? Random.Range(0, options.Count) : RNG.Next(0, options.Count);
            result = options[ix];
            return true;
        }
        List<GridTile> tiles = this.TileLookup[0].Where(t => !t.IsFloor).ToList();
        // Get the first wall with no neighbors
        result = tiles[0];
        return false;
    }

    public void BuildDictionary()
    {
        this._TileLookup = new Dictionary<int, List<GridTile>>();
        this._Floors = new List<GridTile>();
        foreach (GridTile t in Tiles)
        {
            if (t.IsMirror && t.MirrorTile.IsMirror) {
                Debug.Log($"GridTile Mirror must not be a Mirror.", t);
                throw new System.Exception("GridTileMirror must not be a Mirror.");
            }
            if (!this._TileLookup.ContainsKey(t.Criteria)) this._TileLookup[t.Criteria] = new List<GridTile>();
            this._TileLookup[t.Criteria].Add(t);
            if (t.IsFloor) this._Floors.Add(t);
        }
    }
    
}
