using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapChunker
{
    public CameraFollower Camera;
    private int Width, Height;
    public GridBounds MapBounds;
    private GridBounds _CurrentBounds;
    private GridBounds CurrentBounds
    {
        get => _CurrentBounds;
        set
        {
            _CurrentBounds = value;
            RebuildBounds = new GridBounds(_CurrentBounds.Center, 0, 0);
        }
    }
    private GridBounds RebuildBounds;
    public readonly Transform WallContainer;
    public readonly Transform FloorContainer;


    private Dictionary<(int, int), GameObject> Loaded;
    private Dictionary<(int, int), char> MapData;
    private readonly Dictionary<char, GridTileSet> TileSets;
    private readonly HashSet<char> IsWall;
    private bool FirstLoad = true;

    internal MapChunker(CameraFollower camera,
                      Transform wallContainer,
                      Transform floorContainer,
                      Dictionary<char, GridTileSet> tileSets,
                      HashSet<char> isWall,
                      string mapData)
    {
        this.Camera = camera;
        this.Camera.Chunker = this;
        this.SetSize(this.Camera.OrthographicBounds());
        this.WallContainer = wallContainer;
        this.FloorContainer = floorContainer;
        this.TileSets = tileSets;
        this.IsWall = isWall;
        this.LoadMap(mapData);
    }

    public void SetSize(Bounds bounds)
    {
        (this.Width, this.Height) = ((int)(bounds.extents.x) + 2, (int)(bounds.extents.y) + 2);
    }

    public bool CheckAndBuildChunk()
    {
        // (int, int) pos = ((int)Target.position.y, (int)Target.position.x);
        (int, int) pos = ((int)Camera.transform.position.y, (int)Camera.transform.position.x);
        // If we are within the RebuildBounds we do nothing
        if (RebuildBounds.Contains(pos)) return false;
        // Otherwise, we build a chunk
        BuildNextChunk();
        return true;
    }

    public void BuildNextChunk(GridBounds _nextBounds = null)
    {
        TimerUtil.StartTrial("TryUnload", "BuildNextChunk","CreateWall","CreateFloor");
        TimerUtil.StartTimer("BuildNextChunk");
        (int, int) center = ((int)Camera.transform.position.y, (int)Camera.transform.position.x);
        GridBounds NextBounds = new GridBounds(center, Width, Height);
        if (_nextBounds != null) NextBounds = _nextBounds;

        //TODO: Need to have a better RNG solutions
        System.Random RNG = new System.Random();
        IEnumerable<(int, int)> toCheck;
        if (this.FirstLoad)
        {
            toCheck = NextBounds;
            this.FirstLoad = false;
        }
        else
        {
            toCheck = CurrentBounds.Difference(NextBounds).ToList();
        }

        // Loop through elements that do not overlap with new bounds
        foreach ((int row, int col) pos in toCheck)
        {
            // If the tile is not in the new bounds, we unload it.
            if (!NextBounds.Contains(pos))
            {
                TimerUtil.StartTimer("TryUnload");
                this.TryUnload(pos);
                TimerUtil.StopTimer("TryUnload");
                continue;
            }

            // Otherwise, we load it.
            if (!MapData.TryGetValue(pos, out char ch)) continue;
            // TODO: Add black tile??
            if (ch == ' ') continue;

            GameObject obj = this.IsWall.Contains(ch) ? this.CreateWall(ch, pos) : this.CreateFloor(ch, pos);
            Loaded[pos] = obj;
        }

        this.CurrentBounds = NextBounds;
        TimerUtil.StopTimer("BuildNextChunk");
    }

    private bool TryUnload((int, int) pos)
    {
        if (this.Loaded.TryGetValue(pos, out GameObject toUnload))
        {
            this.Loaded.Remove(pos);
            UnityEngine.Object.Destroy(toUnload);
            return true;
        }
        return false;
    }

    private GameObject CreateWall(char ch, (int row, int col) pos)
    {
        TimerUtil.StartTimer("CreateWall");
        //TODO: Calculate criteria then spawn wall
        NeighborSpaceUtil.Spaces.Select(n => NeighborSpaceUtil.ReverseSpaceLookup[n]);
        int criteria = 0;
        foreach (NeighborSpace n in NeighborSpaceUtil.Spaces)
        {
            (int col, int row) off = NeighborSpaceUtil.ReverseSpaceLookup[n];
            (int, int) nPos = (pos.row + off.row, pos.col + off.col);
            // If no neighbor, continue;
            if (!MapData.TryGetValue(nPos, out char nCh) || !this.IsWall.Contains(nCh)) continue;
            criteria += NeighborSpaceUtil.ToCriteriaBit(n);
        }
        GridTileSet tileSet = TileSets[ch];
        WallTile toClone = tileSet.TileLookup[criteria];
        GameObject newObj =
        Spawner.SpawnObject(toClone.gameObject)
               .Parent(WallContainer)
               .LocalPosition(new Vector2(pos.col, pos.row))
               .Name($"Wall[{ch}] @ ({pos.row}, {pos.col})")
               .Spawn();
        TimerUtil.StopTimer("CreateWall");
        return newObj;
    }

    private GameObject CreateFloor(char ch, (int row, int col) pos)
    {
        TimerUtil.StartTimer("CreateFloor");
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
        TimerUtil.StopTimer("CreateFloor");
        return newFloor;
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

    public static MapChunkerBuilder Instantiate(CameraFollower camera)
    {
        return new MapChunkerBuilder().Camera(camera);
    }

    private Transform _WallContainer, _FloorContainer;
    private CameraFollower _Camera;
    private Dictionary<char, GridTileSet> _TileSets = new Dictionary<char, GridTileSet>();
    private HashSet<char> _IsWall = new HashSet<char>();
    private string _MapData;

    public MapChunkerBuilder Camera(CameraFollower camera) => SetField(ref _Camera, camera);
    public MapChunkerBuilder WallContainer(Transform wallContainer) => SetField(ref _WallContainer, wallContainer);
    public MapChunkerBuilder FloorContainer(Transform floorContainer) => SetField(ref _FloorContainer, floorContainer);
    public MapChunkerBuilder MapData(string mapData) => SetField(ref _MapData, mapData);
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
        return new MapChunker(this._Camera,
                              this._WallContainer,
                              this._FloorContainer,
                              this._TileSets,
                              this._IsWall,
                              this._MapData);
    }

}