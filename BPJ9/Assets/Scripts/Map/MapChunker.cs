using System.Collections.Generic;
using UnityEngine;

public class MapChunker
{
    private Transform _Target;
    public Transform Target
    {
        get => _Target;
        set
        {
            _Target = value;
            (int, int) center = ((int)_Target.position.y, (int)_Target.position.x);
            CurrentBounds = new GridBounds(center, Width, Height);
        }
    }
    public readonly int Width, Height;
    public GridBounds MapBounds;
    public readonly Transform WallContainer;
    public readonly Transform FloorContainer;

    private GridBounds _CurrentBounds;
    public GridBounds CurrentBounds
    {
        get => _CurrentBounds;
        private set
        {
            _CurrentBounds = value;
            RebuildBounds = new GridBounds(_CurrentBounds.Center, Width / 2, Height / 2);
        }
    }
    public GridBounds RebuildBounds;
    private Dictionary<(int, int), GameObject> Loaded;
    private Dictionary<(int, int), char> MapData;
    private readonly Dictionary<char, GridTileSet> TileSets;
    private readonly HashSet<char> IsWall;
    private bool FirstLoad = true;

    internal MapChunker(Transform target,
                      Transform wallContainer,
                      Transform floorContainer,
                      Dictionary<char, GridTileSet> tileSets,
                      HashSet<char> isWall,
                      string mapData,
                      int width,
                      int height)
    {
        this.Target = target;
        this.WallContainer = wallContainer;
        this.FloorContainer = floorContainer;
        this.TileSets = tileSets;
        this.IsWall = isWall;
        this.Width = width;
        this.Height = height;
        this.LoadMap(mapData);
    }

    public void CheckAndBuildChunk()
    {
        (int, int) pos = ((int)Target.position.y, (int)Target.position.x);
        // If we are within the RebuildBounds we do nothing
        if (RebuildBounds.Contains(pos)) return;
        // Otherwise, we build a chunk
        BuildChunk();
    }

    public void BuildChunk(GridBounds _nextBounds = null)
    {
        long start = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
        (int, int) center = ((int)Target.position.y, (int)Target.position.x);
        GridBounds NextBounds = new GridBounds(center, Width, Height);
        if (_nextBounds != null) NextBounds = _nextBounds;

        //TODO: Need to have a better RNG solutions
        System.Random RNG = new System.Random();
        //TODO: This doesn't actually work for walls on the edge of the chunk
        List<(int, int, WallTilePlaceHolder)> walls = new List<(int, int, WallTilePlaceHolder)>();

        IEnumerable<(int,int)> toCheck;
        if (this.FirstLoad)
        {
            toCheck = NextBounds;
            this.FirstLoad = false;
        }
        else
        {
            toCheck = CurrentBounds.Difference(NextBounds);
        }

        // Loop through elements that do not overlap with new bounds
        foreach ((int row, int col) pos in toCheck)
        {
            // If tile is loaded, we should unload it
            if (Loaded.ContainsKey(pos))
            {
                this.Unload(pos);
                continue;
            }

            // Otherwise, we load it.
            if (!MapData.TryGetValue(pos, out char ch)) continue;
            // TODO: Add black tile??
            if (ch == ' ') continue;

            if (this.IsWall.Contains(ch)) walls.Add((pos.row, pos.col, this.CreateWall(ch, pos)));
            else Loaded[pos] = CreateFloor(ch, pos).gameObject;
        }

        foreach ((int row, int col, WallTilePlaceHolder wall) w in walls)
        {
            GameObject obj = w.wall.ReplaceWithWallTile();
            Loaded[(w.row, w.col)] = obj;
        }
        this.CurrentBounds = NextBounds;
        long end = System.DateTime.Now.Ticks / System.TimeSpan.TicksPerMillisecond;
        Debug.Log($"Chunk built in {end - start} milliseconds.");
    }

    private void Unload((int, int) pos)
    {
        GameObject toUnload = this.Loaded[pos];
        this.Loaded.Remove(pos);
        UnityEngine.Object.Destroy(toUnload);
    }

    private WallTilePlaceHolder CreateWall(char ch, (int row, int col) pos)
    {
        GameObject placeHolder = new GameObject($"Wall[{ch}] @ ({pos.row}, {pos.col})");
        placeHolder.transform.parent = WallContainer;
        placeHolder.transform.localPosition = new Vector2(pos.col, pos.row);
        WallTilePlaceHolder asHolder = placeHolder.AddComponent<WallTilePlaceHolder>();
        asHolder.TileSet = this.TileSets[ch];
        return asHolder;
    }

    private FloorTile CreateFloor(char ch, (int row, int col) pos)
    {
        List<FloorTile> options = this.TileSets[ch].Floors;
        //TODO: Need Better RNG
        // int ix = RNG.Next(0, options.Count);
        int ix = Random.Range(0, options.Count);
        FloorTile toCopy = options[ix];
        // ix = RNG.Next(0, toCopy.Templates.Count);
        ix = Random.Range(0, toCopy.Templates.Count);
        Sprite s = toCopy.Templates[ix].GetSprite();
        GameObject newFloor =
        Spawner.SpawnObject(toCopy.gameObject)
               .Name($"Floor[{ch}] @ ({pos.row}, {pos.col})")
               .Parent(FloorContainer)
               .LocalPosition(new Vector2(pos.col, pos.row))
               .Spawn();
        newFloor.GetComponent<SpriteRenderer>().sprite = s;
        return newFloor.GetComponent<FloorTile>();
    }

    public void LoadMap(string mapData)
    {
        this.Loaded = new Dictionary<(int, int), GameObject>();
        this.MapData = new Dictionary<(int, int), char>();
        this.MapData = this.LoadMap(mapData, out MapBounds);
    }

    private Dictionary<(int, int), char> LoadMap(string mapData, out GridBounds bounds)
    {
        Dictionary<(int, int), char> Grid = new Dictionary<(int, int), char>();
        int rows = mapData.Split('\n').Length;
        int row = rows - 1;
        int cols = 0;
        int col = 0;
        foreach (char c in mapData)
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
        bounds = new GridBounds(rows, cols, 0, 0);
        return Grid;
    }
}

public class MapChunkerBuilder
{

    public static MapChunkerBuilder Instantiate(Transform target)
    {
        return new MapChunkerBuilder().Target(target);
    }

    private Transform _Target, _WallContainer, _FloorContainer;
    private Dictionary<char, GridTileSet> _TileSets = new Dictionary<char, GridTileSet>();
    private HashSet<char> _IsWall = new HashSet<char>();
    private string _MapData;
    private int _Width, _Height;

    public MapChunkerBuilder Target(Transform target) => SetField(ref _Target, target);
    public MapChunkerBuilder WallContainer(Transform wallContainer) => SetField(ref _WallContainer, wallContainer);
    public MapChunkerBuilder FloorContainer(Transform floorContainer) => SetField(ref _FloorContainer, floorContainer);
    public MapChunkerBuilder MapData(string mapData) => SetField(ref _MapData, mapData);
    public MapChunkerBuilder Width(int width) => SetField(ref _Width, width);
    public MapChunkerBuilder Height(int height) => SetField(ref _Height, height);
    public MapChunkerBuilder AddTileSet(char ch, GridTileSet tileSet)
    {
        if (this._TileSets.ContainsKey(ch)) throw new System.Exception($"Duplicate tile set characater found: {ch}/");
        this._TileSets[ch] = tileSet;
        return this;
    }

    public MapChunkerBuilder AddWallChar(char ch)
    {
        if (this._IsWall.Contains(ch)) throw new System.Exception($"Duplicate wall character found: {ch}.");
        this._IsWall.Add(ch);
        return this;
    }

    private MapChunkerBuilder SetField<T>(ref T refObj, T newObj)
    {
        refObj = newObj;
        return this;
    }

    public MapChunker Build()
    {
        return new MapChunker(this._Target,
                              this._WallContainer,
                              this._FloorContainer,
                              this._TileSets,
                              this._IsWall,
                              this._MapData,
                              this._Width,
                              this._Height);
    }

}