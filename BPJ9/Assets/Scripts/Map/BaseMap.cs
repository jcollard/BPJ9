using System.Collections;
using System.Collections.Generic;
using CaptainCoder.Unity;
using UnityEngine;
using System.Linq;

public class BaseMap : MonoBehaviour
{
    public Transform MapContainer;
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

    private Dictionary<char, MapTileDefinition> _DefinitionLookup;
    private Dictionary<char, MapTileDefinition> DefinitionLookup => BuildDefinitions();
    private HashSet<char> IsFloor;
    private HashSet<char> IsWall;

    public void Start()
    {
        // TODO: Consider not calling this on start as it will rebuild when
        // the scene changes.
        BuildMap();
    }

    public void BuildMap()
    {
        BuildDefinitions();
        System.Random RNG = new System.Random(Seed);
        UnityEngineUtils.Instance.DestroyChildren(MapContainer);
        Dictionary<(int, int), char> Grid = BuildGrid(out int rows, out int cols);
        Dictionary<(int, int), List<char>> positions = new Dictionary<(int, int), List<char>>();

        foreach ((int r, int c) pos in Grid.Keys)
        {
            (int, int) TL = (pos.r, pos.c - 1);
            (int, int) TR = (pos.r, pos.c);
            (int, int) BL = (pos.r - 1, pos.c - 1);
            (int, int) BR = (pos.r - 1, pos.c);
            (int, int)[] hack = { TL, TR, BL, BR };
            foreach (var ix in hack)
            {
                if (!positions.ContainsKey(ix))
                {
                    positions[ix] = new List<char>();
                }
            }
            positions[TL].Add(Grid[pos]);
            positions[TR].Add(Grid[pos]);
            positions[BL].Add(Grid[pos]);
            positions[BR].Add(Grid[pos]);
        }


        foreach ((int r, int c) pos in positions.Keys)
        {
            // TODO: need to do something smart to select the correct value because
            // sometimes you'll have two competing chars. Some kinds of "transition"
            // lookup.
            List<char> nearbyChars = positions[pos];
            char ch = nearbyChars[0];
            char[] wallChars = nearbyChars.Where(ch => IsWall.Contains(ch)).ToArray();
            if (wallChars.Length > 0)
            {
                ch = wallChars[0];
            }
            int r = pos.r;
            int c = pos.c;
            int type = GetCorners(r, c, Grid);
            MapTileDefinition def = DefinitionLookup[ch];
            (TilePosition position, bool useWall) = def.Strategy.GetTilePosition(type);
            TileSet template = useWall ? def.WallTemplate : def.FloorTemplate;
            GameObject toClone = template.GetTileTemplate(position, RNG);
            Spawner.SpawnObject(toClone)
                   .Parent(MapContainer)
                   .LocalPosition(new Vector2(c, r))
                   .Name($"Map: ({r}, {c})")
                   .Spawn();
        }

    }

    private int GetCorners(int row, int col, Dictionary<(int, int), char> Grid)
    {
        int type = 0;
        // Top Left
        var ix = (row + 1, col);
        type += CheckWall(ix, Grid) ? 0b0001 : 0b0000;
        // Top Right
        ix = (row + 1, col + 1);
        type += CheckWall(ix, Grid) ? 0b0010 : 0b0000;
        // Bottom Left
        ix = (row, col);
        type += CheckWall(ix, Grid) ? 0b0100 : 0b0000;
        // Bottom Right
        ix = (row, col + 1);
        type += CheckWall(ix, Grid) ? 0b1000 : 0b0000;
        return type;
    }

    private bool CheckWall((int, int) ix, Dictionary<(int, int), char> Grid)
    {
        if (!Grid.ContainsKey(ix)) return true;
        return this.IsWall.Contains(Grid[ix]);
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

    private Dictionary<char, MapTileDefinition> BuildDefinitions()
    {
        if (_DefinitionLookup == null || IsDirty)
        {
            _DefinitionLookup = new Dictionary<char, MapTileDefinition>();
            IsFloor = new HashSet<char>();
            IsWall = new HashSet<char>();
            foreach (MapTileDefinition def in Definitions)
            {
                if (_DefinitionLookup.ContainsKey(def.FloorCharacter))
                    throw new System.Exception($"Duplicate definition found for character {def.FloorCharacter}.");
                if (_DefinitionLookup.ContainsKey(def.WallCharacter))
                    throw new System.Exception($"Duplicate definition found for character {def.WallCharacter}.");
                _DefinitionLookup[def.FloorCharacter] = def;
                _DefinitionLookup[def.WallCharacter] = def;
                IsFloor.Add(def.FloorCharacter);
                IsWall.Add(def.WallCharacter);
            }
            IsDirty = false;
        }
        return _DefinitionLookup;
    }

}

[System.Serializable]
public class MapTileDefinition
{
    public char FloorCharacter;
    public char WallCharacter;

    public TileSet FloorTemplate;
    public TileSet WallTemplate;

    public TileSelector Strategy;
    // TODO Transition tiles?
}