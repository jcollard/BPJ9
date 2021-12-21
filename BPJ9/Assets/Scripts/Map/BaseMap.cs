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
            (int, int)[] hack = {TL, TR, BL, BR};
            foreach (var ix in hack)
            {
                if(!positions.ContainsKey(ix))
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
            bool[] corners = GetCorners(r, c, Grid);
            (TilePosition position, bool useWall) = GetTilePosition(corners);
            MapTileDefinition def = DefinitionLookup[ch];
            TileSet template = useWall ? def.WallTemplate : def.FloorTemplate;
            GameObject toClone = template.GetTileTemplate(position, RNG);
            Spawner.SpawnObject(toClone)
                   .Parent(MapContainer)
                   .LocalPosition(new Vector2(c, r))
                   .Name($"Map: ({r}, {c})")
                   .Spawn();
        }

    }

    private (TilePosition, bool) GetTilePosition(bool[] corners)
    {
        int type = 0;
        // TL
        type += corners[0] ? 0b0001 : 0;
        // TR
        type += corners[1] ? 0b0010 : 0;
        // BL
        type += corners[2] ? 0b0100 : 0;
        // BR
        type += corners[3] ? 0b1000 : 0;

        // TilePostion, UseWallType
        (TilePosition, bool) position = type switch
        {
            // None
            0b0000 => (TilePosition.Middle, false),
            // __ __ __ TL
            0b0001 => (TilePosition.BottomRight, true),
            // __ __ TR __
            0b0010 => (TilePosition.BottomLeft, true),
            // __ __ TR TL (TOP)
            0b0011 => (TilePosition.Top, false),
            // __ BL __ __
            0b0100 => (TilePosition.TopRight, true),
            // __ BL __ TL (Left)
            0b0101 => (TilePosition.Left, false),
            // __ BL TR __ (Invalid)
            0b0110 => throw new System.Exception("Invalid tile placement: 0b0110"),
            // __ BL TR TL (Left and Top)
            0b0111 => (TilePosition.TopLeft, false),
            // BR __ __ __
            0b1000 => (TilePosition.TopLeft, true),
            // BR __ __ TL (Invalid)
            0b1001 => throw new System.Exception("Invalid tile placement: 0b1001"),
            // BR __ TR __ (Right)
            0b1010 => (TilePosition.Right, false),
            // BR __ TR TL (TOP & RIGHT)
            0b1011 => (TilePosition.TopRight, false),
            // BR BL __ __ (Bottom)
            0b1100 => (TilePosition.Bottom, false),
            // BR BL __ TL (BOTTOM & LEFT)
            0b1101 => (TilePosition.BottomLeft, false),
            // BR BL TR __  (BOTTOM & RIGHT)
            0b1110 => (TilePosition.BottomRight, false),
            // BR BL TR LT (ALL)
            0b1111 => (TilePosition.Middle, true),
            _ => throw new System.Exception($"Invalid tile placement {type}")
        };
        return position;
    }

    private bool[] GetCorners(int row, int col, Dictionary<(int, int), char> Grid)
    {
        bool[] corners = new bool[4];
        // Top Left
        var ix = (row + 1, col);
        corners[0] = Grid.ContainsKey(ix) ? IsWall.Contains(Grid[ix]) : true;
        // Top Right
        ix = (row + 1, col + 1);
        corners[1] = Grid.ContainsKey(ix) ? IsWall.Contains(Grid[ix]) : true;
        // Bottom Left
        ix = (row, col);
        corners[2] = Grid.ContainsKey(ix) ? IsWall.Contains(Grid[ix]) : true;
        // Bottom Right
        ix = (row, col + 1);
        corners[3] = Grid.ContainsKey(ix) ? IsWall.Contains(Grid[ix]) : true;

        return corners;
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