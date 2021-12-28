using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MapChunker
{
    public static MapChunker Instance;
    public CameraFollower Camera;
    private int MinWidth, MinHeight, PreLoadSize = 2;
    private int PreLoadMaxWork = 3, MaxUnloadWork = 30;
    public GridBounds MapBounds;
    private GridBounds _CurrentBounds;
    private GridBounds CurrentBounds
    {
        get => _CurrentBounds;
        set
        {
            _CurrentBounds = value;
            RebuildBounds = new GridBounds(_CurrentBounds.Center, 0, 0);
            CalcPreBuild();
        }
    }
    private GridBounds PreloadBounds, LastPreLoadBounds;
    private IEnumerable<(int, int)> PreloadLocations;
    private HashSet<(int, int)> UnloadLocations = new HashSet<(int, int)>();
    private Queue<(int, int)> UnloadLocationOrder = new Queue<(int, int)>();
    private bool PreLoadComplete = false;
    private GridBounds RebuildBounds;
    public readonly Transform WallContainer;
    public readonly Transform FloorContainer;
    private Dictionary<(int, int), GameObject> Loaded;
    private int MaxGameObjects = 2000;
    private Dictionary<(int, int), char> MapData;
    private Dictionary<(int, int), char> RoomData;
    private Dictionary<(int, int), char> TransitionData;
    private readonly Dictionary<char, GridTileSet> TileSets;
    private readonly HashSet<char> IsWall;
    private bool FirstLoad = true;

    internal MapChunker(CameraFollower camera,
                      Transform wallContainer,
                      Transform floorContainer,
                      Dictionary<char, GridTileSet> tileSets,
                      HashSet<char> isWall,
                      string mapData,
                      string roomData,
                      string transitionData)
    {
        Instance = this;
        this.Camera = camera;
        this.Camera.Chunker = this;
        this.SetSize(this.Camera.OrthographicBounds());
        (int, int) center = ((int)Camera.transform.position.y, (int)Camera.transform.position.x);
        this.CurrentBounds = new GridBounds(center, this.MinWidth, this.MinHeight);
        this.WallContainer = wallContainer;
        this.FloorContainer = floorContainer;
        this.TileSets = tileSets;
        this.IsWall = isWall;
        this.LoadMap(mapData);
        this.LoadRooms(roomData);
        this.LoadTransitions(roomData);
        this.BuildNextChunk();
    }

    public void SetSize(Bounds bounds)
    {
        (this.MinWidth, this.MinHeight) = ((int)(bounds.extents.x) + 2, (int)(bounds.extents.y) + 2);
    }

    public void CalcPreBuild()
    {
        this.LastPreLoadBounds = this.PreloadBounds == null ? null : this.PreloadBounds;
        this.PreloadBounds = new GridBounds(_CurrentBounds, PreLoadSize);
        this.PreloadLocations = PreloadBounds.Difference(_CurrentBounds);
        this.PreLoadComplete = false;
        if (this.LastPreLoadBounds != null)
        {
            TimerUtil.StartTrial("Build UnloadSet");
            TimerUtil.StartTimer("Build UnloadSet");
            foreach ((int, int) pos in PreloadBounds.Difference(LastPreLoadBounds))
            {
                if (this.UnloadLocations.Contains(pos)) continue;
                this.UnloadLocations.Add(pos);
                this.UnloadLocationOrder.Enqueue(pos);
            }
            TimerUtil.StopTimer("Build UnloadSet");
        }
    }

    public bool CheckAndBuildChunk()
    {
        // (int, int) pos = ((int)Target.position.y, (int)Target.position.x);
        (int, int) pos = ((int)Camera.transform.position.y, (int)Camera.transform.position.x);
        // If we are outside the rebuild bounds, we build the next chunk.
        if (!RebuildBounds.Contains(pos)) return BuildNextChunk();
        // If we are in the bounds, try preloading
        if (this.PreloadTick()) return true;
        // If we are done preloading unload unused objects.
        return UnloadTick();
    }

    private bool PreloadTick()
    {
        if (this.PreLoadComplete) return false;
        TimerUtil.StartTrial("PreloadTick");
        TimerUtil.StartTimer("PreloadTick");
        int work = 0;
        foreach ((int, int) pos in this.PreloadLocations)
        {
            if (this.MapData.TryGetValue(pos, out char ch) && !this.Loaded.ContainsKey(pos))
            {
                if (!this.LoadTile(ch, pos)) continue;
                if (work++ >= PreLoadMaxWork)
                {
                    TimerUtil.StopTimer("PreloadTick");
                    return true;
                }
            }
        }
        this.PreLoadComplete = true;
        TimerUtil.StopTimer("PreloadTick");
        return false;
    }

    private bool UnloadTick()
    {
        if (UnloadLocations.Count < MaxGameObjects) return false;
        TimerUtil.StartTrial("UnloadTick");
        TimerUtil.StartTimer("UnloadTick");
        List<(int, int)> toRemoved = new List<(int, int)>();
        int work = 0;
        while (work++ < MaxUnloadWork && this.UnloadLocationOrder.Count > 0)
        {
            (int, int) pos = this.UnloadLocationOrder.Dequeue();

            // Don't count work on elements that needed to stay / were not unloaded
            if (this.PreloadBounds.Contains(pos) || !this.TryUnload(pos))
            {
                this.UnloadLocationOrder.Enqueue(pos);
                continue;
            }

            this.UnloadLocations.Remove(pos);

        }

        TimerUtil.StopTimer("UnloadTick");
        return work > 0;
    }

    public bool BuildNextChunk(GridBounds _nextBounds = null)
    {
        (int, int) center = ((int)Camera.transform.position.y, (int)Camera.transform.position.x);
        GridBounds NextBounds = new GridBounds(center, MinWidth, MinHeight);
        if (_nextBounds != null) NextBounds = _nextBounds;

        // If the preload is completed, we don't need to do any work.
        if (this.PreLoadComplete)
        {
            this.CurrentBounds = NextBounds;
            return false;
        }
        TimerUtil.StartTrial("LoadTile", "BuildNextChunk");
        TimerUtil.StartTimer("BuildNextChunk");

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

            // Check if it is part of the map, if it is not we don't need to load it
            if (!MapData.TryGetValue(pos, out char ch)) continue;
            // If it is a blank space, we skip it
            if (ch == ' ') continue;
            // Check if it is already loaded, it it is we don't need to load it
            if (Loaded.ContainsKey(pos)) continue;
            TimerUtil.StartTimer("LoadTile");
            this.LoadTile(ch, pos);
            TimerUtil.StopTimer("LoadTile");
        }

        this.CurrentBounds = NextBounds;
        TimerUtil.StopTimer("BuildNextChunk");
        return true;
    }

    private bool LoadTile(char ch, (int, int) pos)
    {
        if (ch == ' ') return false;
        GameObject obj = this.IsWall.Contains(ch) ? this.CreateWall(ch, pos) : this.CreateFloor(ch, pos);
        Loaded[pos] = obj;
        return true;
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
        int ix = Random.Range(0, toClone.Templates.Count);
        Sprite s = toClone.Templates[ix].GetSprite();
        GameObject newObj =
        Spawner.SpawnObject(toClone.gameObject)
               .Parent(WallContainer)
               .LocalPosition(new Vector2(pos.col, pos.row))
               .Name($"Wall[{ch}] @ ({pos.row}, {pos.col})")
               .Spawn();

        newObj.GetComponent<SpriteRenderer>().sprite = s;
        return newObj;
    }

    private GameObject CreateFloor(char ch, (int row, int col) pos)
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

            if (c != ' ')
                Grid[(row, col)] = c;

            col++;
            cols = Mathf.Max(cols, col);
        }
        bounds = new GridBounds(rows, cols, 0, 0);
        return Grid;
    }

    public bool TryGetRoom((int, int) pos, out char ch) => RoomData.TryGetValue(pos, out ch);

    public void LoadRooms(string roomData)
    {
        RoomData = new Dictionary<(int, int), char>();
        int rows = roomData.Split('\n').Length;
        int row = rows - 1;
        int cols = 0;
        int col = 0;
        foreach (char c in roomData)
        {
            if (c == '\n')
            {
                row--;
                col = 0;
                continue;
            }

            if (c != ' ')
                RoomData[(row, col)] = c;

            col++;
            cols = Mathf.Max(cols, col);
        }
    }

    public void LoadTransitions(string transitionData)
    {
        TransitionData = new Dictionary<(int, int), char>();
        int rows = transitionData.Split('\n').Length;
        int row = rows - 1;
        int cols = 0;
        int col = 0;
        foreach (char c in transitionData)
        {
            if (c == '\n')
            {
                row--;
                col = 0;
                continue;
            }

            if (c != ' ')
                RoomData[(row, col)] = c;

            col++;
            cols = Mathf.Max(cols, col);
        }
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
    private string _MapData, _RoomData, _TransitionData;

    public MapChunkerBuilder Camera(CameraFollower camera) => SetField(ref _Camera, camera);
    public MapChunkerBuilder WallContainer(Transform wallContainer) => SetField(ref _WallContainer, wallContainer);
    public MapChunkerBuilder FloorContainer(Transform floorContainer) => SetField(ref _FloorContainer, floorContainer);
    public MapChunkerBuilder MapData(string mapData) => SetField(ref _MapData, mapData);
    public MapChunkerBuilder TransitionData(string transitionData) => SetField(ref _TransitionData, transitionData);
    public MapChunkerBuilder RoomData(string roomData) => SetField(ref _RoomData, roomData);
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
                              this._MapData,
                              this._RoomData,
                              this._TransitionData);
    }

}