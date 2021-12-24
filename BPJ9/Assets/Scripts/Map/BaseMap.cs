using System.Collections;
using System.Collections.Generic;
using CaptainCoder.Unity;
using UnityEngine;
using System.Linq;

public class BaseMap : MonoBehaviour
{
    public Transform FloorContainer;
    public Transform WallContainer;
    public MapTileDefinition[] Definitions;
    public bool IsDirty = true;
    public int Seed = 42;

    [SerializeField]
    private string _MapData;
    public string MapData
    {
        get => _MapData;
        set
        {
            _MapData = value;
            IsDirty = true;
        }
    }

    private Dictionary<char, GridTileSet> _DefinitionLookup;
    private Dictionary<char, GridTileSet> DefinitionLookup 
    {
        get 
        {
            if(_DefinitionLookup == null)
            {
                _DefinitionLookup = BuildDefinitions();
            }
            return _DefinitionLookup;
        }
    }
    private HashSet<char> IsWall;

    public void Start()
    {
        // TODO: Consider not calling this on start as it will rebuild when
        // the scene changes.
        BuildMap();
    }

    public void BuildMap()
    {
        System.Random RNG = new System.Random(Seed);
        UnityEngineUtils.Instance.DestroyChildren(WallContainer);
        UnityEngineUtils.Instance.DestroyChildren(FloorContainer);
        BuildDefinitions();
        Dictionary<(int, int), char> Grid = BuildGrid(out int rows, out int cols);
        
        List<WallTilePlaceHolder> walls = new List<WallTilePlaceHolder>();
        foreach ((int row, int col) pos in Grid.Keys)
        {
            char ch = Grid[pos];
            // TODO: Add black tile??
            if (ch == ' ') continue;
            if (this.IsWall.Contains(ch))
            {
                GameObject placeHolder = new GameObject($"Wall[{ch}] @ ({pos.row}, {pos.col})");
                placeHolder.transform.parent = WallContainer;
                placeHolder.transform.localPosition = new Vector2(pos.col, pos.row);
                WallTilePlaceHolder asHolder = placeHolder.AddComponent<WallTilePlaceHolder>();
                asHolder.TileSet = this.DefinitionLookup[ch];
                walls.Add(asHolder);
            }
            else
            {
                List<FloorTile> options = this.DefinitionLookup[ch].Floors;
                int ix = RNG.Next(0, options.Count);
                FloorTile toCopy = options[ix];
                ix = RNG.Next(0, toCopy.Templates.Count);
                Sprite s = toCopy.Templates[ix].GetSprite();
                GameObject newFloor = 
                Spawner.SpawnObject(toCopy.gameObject)
                       .Name($"Floor[{ch}] @ ({pos.row}, {pos.col})")
                       .Parent(FloorContainer)
                       .LocalPosition(new Vector2(pos.col, pos.row))
                       .Spawn();
                newFloor.GetComponent<SpriteRenderer>().sprite = s;
            }
        }

        foreach (WallTilePlaceHolder wall in walls)
        {
            wall.ReplaceWithWallTile();
        }

    }

    public Dictionary<char, GridTileSet> BuildDefinitions()
    {
        this._DefinitionLookup = new Dictionary<char, GridTileSet>();
        this.IsWall = new HashSet<char>();
        foreach(MapTileDefinition def in Definitions)
        {
            this.IsWall.Add(def.WallCharacter);
            if(this._DefinitionLookup.ContainsKey(def.FloorCharacter)) 
                UnityEngineUtils.Instance.FailFast($"Duplicate tile character definition found: {def.FloorCharacter}.", this.gameObject);
            
            this._DefinitionLookup[def.FloorCharacter] = def.TileSet;

            if(this._DefinitionLookup.ContainsKey(def.WallCharacter)) 
                UnityEngineUtils.Instance.FailFast($"Duplicate tile character definition found: {def.WallCharacter}.", this.gameObject);
            
            this._DefinitionLookup[def.WallCharacter] = def.TileSet;
            
        }
        return this._DefinitionLookup;
    }

    private Dictionary<(int, int), char> BuildGrid(out int rows, out int cols)
    {
        Dictionary<(int, int), char> Grid = new Dictionary<(int, int), char>();
        rows = MapData.Split('\n').Length;
        int row = rows - 1;
        cols = 0;
        int col = 0;
        foreach (char c in MapData)
        {
            if (c == '\n')
            {
                row--;
                col = 0;
                continue;
            }

            Grid[(row, col)] = c;

            col++;
            cols = Mathf.Max(cols, col);
        }
        return Grid;
    }
}

[System.Serializable]
public class MapTileDefinition
{
    public char FloorCharacter;
    public char WallCharacter;

    public GridTileSet TileSet;
}