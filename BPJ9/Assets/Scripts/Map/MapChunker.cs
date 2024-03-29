using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CaptainCoder.Unity;

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
            // CalcPreBuild();
        }
    }
    private GridBounds PreloadBounds, LastPreLoadBounds;
    private IEnumerable<(int, int)> PreloadLocations;
    private HashSet<(int, int)> UnloadLocations = new HashSet<(int, int)>();
    private Queue<(int, int)> UnloadLocationOrder = new Queue<(int, int)>();
    private bool PreLoadComplete = false;
    private GridBounds RebuildBounds;
    public readonly Transform WallContainer, FloorContainer, TransitionContainer, EnemyContainer, ItemContainer;
    public readonly TransitionController TemplateTransitionController;
    private Dictionary<(int, int), GameObject> Loaded;
    private int MaxGameObjects = 2000;
    private Dictionary<(int, int), char> MapData;
    private Dictionary<(int, int), char> RoomData;
    private Dictionary<(int, int), char> TransitionData;
    private Dictionary<(int, int), GameObject> EnemyData;
    private Dictionary<(int, int), GameObject> ItemData;
    private Dictionary<char, List<(int, int)>> TransitionLookup;
    private Dictionary<char, (Vector2, Vector2)> RoomBounds;
    private readonly Dictionary<char, GridTileSet> TileSets;
    private readonly HashSet<char> IsWall;
    private bool FirstLoad = true;
    public char CurrentRoom = (char)0;

    internal MapChunker(CameraFollower camera,
                      Transform wallContainer,
                      Transform floorContainer,
                      Transform transitionContainer,
                      Transform enemyContainer,
                      Transform itemContainer,
                      Dictionary<char, GridTileSet> tileSets,
                      HashSet<char> isWall,
                      string mapData,
                      string roomData,
                      string transitionData,
                      string enemyData,
                      Dictionary<char, GameObject> enemyLookup,
                      string itemData,
                      Dictionary<char, GameObject> itemLookup)
    {
        Instance = this;
        this.Camera = camera;
        this.Camera.Chunker = this;
        this.SetSize(this.Camera.OrthographicBounds());
        (int, int) center = ((int)Camera.transform.position.y, (int)Camera.transform.position.x);
        this.CurrentBounds = new GridBounds(center, this.MinWidth, this.MinHeight);
        this.WallContainer = wallContainer;
        this.FloorContainer = floorContainer;
        this.TransitionContainer = transitionContainer;
        this.EnemyContainer = enemyContainer;
        this.ItemContainer = itemContainer;
        this.TileSets = tileSets;
        this.IsWall = isWall;
        this.LoadMap(mapData);
        this.LoadRooms(roomData);
        this.LoadTransitions(transitionData);
        this.LoadEnemies(enemyData, enemyLookup);
        this.LoadItems(itemData, itemLookup);
        this.BuildNextChunk();
    }

    public (Vector2, Vector2) GetRoomBounds(char ch) => RoomBounds[ch];

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
        return false;
        // If we are in the bounds, try preloading
        // if (this.PreloadTick()) return true;
        // If we are done preloading unload unused objects.
        // return UnloadTick();
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

    public void Unload(bool immediate = false)
    {
        UnityEngineUtils.Instance.DestroyChildren(WallContainer, immediate);
        UnityEngineUtils.Instance.DestroyChildren(FloorContainer, immediate);
        UnityEngineUtils.Instance.DestroyChildren(EnemyContainer, immediate);
        Loaded.Clear();
    }

    public void LoadAll()
    {
        this.Unload(immediate: true);
        foreach ((int, int) pos in MapData.Keys)
        {
            this.LoadTile(MapData[pos], pos);
            this.LoadEnemy(pos);
            this.SpawnItem(pos);
        }
    }

    public void LoadChunk(GridBounds chunk, char CurrentRoom = (char)0)
    {
        this.Unload();
        if (CurrentRoom > 0)
        {
            this.CurrentRoom = CurrentRoom;
        }

        // Loop through elements that do not overlap with new bounds
        foreach ((int row, int col) pos in chunk)
        {
            // Only draw rooms
            if (!RoomData.TryGetValue(pos, out char roomCh)) continue;
            // Check if it is part of the map, if it is not we don't need to load it
            if (!MapData.TryGetValue(pos, out char ch)) continue;
            // If it is a blank space, we skip it
            if (ch == ' ') continue;
            this.LoadEnemy(pos);
            this.LoadTile(ch, pos);
        }
    }

    private void LoadEnemy((int, int) pos)
    {
        if (!EnemyData.TryGetValue(pos, out GameObject template)) return;
        (int row, int col) = pos;
        GameObject newEnemy = UnityEngine.Object.Instantiate(template);
        newEnemy.transform.parent = EnemyContainer;
        newEnemy.transform.localPosition = new Vector2(col, row);
        newEnemy.SetActive(true);
    }

    private void SpawnItem((int, int) pos)
    {
        if (!ItemData.TryGetValue(pos, out GameObject template)) return;
        (int row, int col) = pos;
        GameObject newItem = UnityEngine.Object.Instantiate(template);
        newItem.transform.parent = ItemContainer;
        newItem.transform.localPosition = new Vector2(col, row);
        newItem.SetActive(true);
    }

    public bool BuildNextChunk(GridBounds _nextBounds = null, char CurrentRoom = (char)0)
    {
        if (CurrentRoom > 0)
        {
            this.CurrentRoom = CurrentRoom;
        }
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
            // Only draw rooms
            if (!RoomData.TryGetValue(pos, out char roomCh)) continue;
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
        // If we know what room we are in, only draw this room.
        if (this.CurrentRoom > 0)
        {
            if (!RoomData.TryGetValue(pos, out char roomCh)) return false;
            if (roomCh != CurrentRoom) return false;
        }

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
        //TODO: This is pretty hacky.
        newObj.AddComponent<WallBlock>();

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
    public bool TryGetRoom(GameObject obj, out char ch)
    {
        int row = (int)Mathf.Round(obj.transform.position.y);
        int col = (int)Mathf.Round(obj.transform.position.x);
        return TryGetRoom((row, col), out ch);
    }

    public void LoadRooms(string roomData)
    {
        RoomBounds = new Dictionary<char, (Vector2, Vector2)>();
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
            {
                RoomData[(row, col)] = c;
                if (!RoomBounds.TryGetValue(c, out (Vector2 min, Vector2 max) bounds))
                {
                    bounds.min = new Vector2(float.PositiveInfinity, float.PositiveInfinity);
                    bounds.max = new Vector2(float.NegativeInfinity, float.NegativeInfinity);
                    RoomBounds[c] = bounds;
                }
                bounds.min.x = Mathf.Min(col, bounds.min.x);
                bounds.max.x = Mathf.Max(col, bounds.max.x);
                bounds.min.y = Mathf.Min(row, bounds.min.y);
                bounds.max.y = Mathf.Max(row, bounds.max.y);
                RoomBounds[c] = bounds;
            }

            col++;
            cols = Mathf.Max(cols, col);
        }
    }

    public void LoadTransitions(string transitionData)
    {
        TransitionData = new Dictionary<(int, int), char>();
        TransitionLookup = new Dictionary<char, List<(int, int)>>();
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
            {
                if (!RoomData.ContainsKey((row, col))) throw new System.Exception($"Transition key {c} is not in a room.");
                TransitionData[(row, col)] = c;
                if (!TransitionLookup.TryGetValue(c, out List<(int, int)> pair))
                {
                    pair = new List<(int, int)>();
                    TransitionLookup[c] = pair;
                }
                if (pair.Count == 2) throw new System.Exception($"Transition key {c} found too many times.");
                pair.Add((row, col));
            }

            col++;
            cols = Mathf.Max(cols, col);
        }

        UnityEngineUtils.Instance.DestroyChildren(this.TransitionContainer);
        foreach (char ch in TransitionLookup.Keys)
        {
            List<(int, int)> pair = TransitionLookup[ch];
            if (pair.Count != 2) throw new System.Exception($"Transition key {ch} found {pair.Count} times.");
            TransitionController tc0, tc1;
            {
                (int row, int col) t0 = pair[0];
                GameObject obj0 = new GameObject($"TransitionController: {ch} 0");
                obj0.AddComponent<BoxCollider2D>().isTrigger = true;
                tc0 = obj0.AddComponent<TransitionController>();
                obj0.transform.parent = TransitionContainer;
                obj0.transform.localPosition = new Vector2(t0.col, t0.row);
            }
            {
                (int row, int col) t1 = pair[1];
                GameObject obj1 = new GameObject($"TransitionController: {ch} 1");
                obj1.AddComponent<BoxCollider2D>().isTrigger = true;
                tc1 = obj1.AddComponent<TransitionController>();
                obj1.transform.parent = TransitionContainer;
                obj1.transform.localPosition = new Vector2(t1.col, t1.row);
            }
            tc0.TeleportTo = tc1;
            tc1.TeleportTo = tc0;

        }
    }

    private void LoadItems(string itemData, Dictionary<char, GameObject> itemLookup)
    {
        UnityEngineUtils.Instance.DestroyChildren(ItemContainer);
        this.ItemData = new Dictionary<(int, int), GameObject>();
        int rows = itemData.Split('\n').Length;
        int row = rows - 1;
        int cols = 0;
        int col = 0;
        foreach (char c in itemData)
        {
            if (c == '\n')
            {
                row--;
                col = 0;
                continue;
            }

            if (c != ' ')
            {
                if (!itemLookup.TryGetValue(c, out GameObject template)) throw new System.Exception($"Could not find template for item char {c}.");
                this.ItemData[(row, col)] = template;
                SpawnItem((row, col));
            }


            col++;
            cols = Mathf.Max(cols, col);
        }
    }

    private void LoadEnemies(string enemyData, Dictionary<char, GameObject> enemyLookup)
    {
        // TODO: Consider loading enemies when you enter a specific room.
        // Challenges: Will have to keep track of destroyed enemies so they 
        // don't respawn when you enter the room. Or... maybe that is a good thing
        // for puzzles?
        UnityEngineUtils.Instance.DestroyChildren(EnemyContainer);
        EnemyData = new Dictionary<(int, int), GameObject>();
        int rows = enemyData.Split('\n').Length;
        int row = rows - 1;
        int cols = 0;
        int col = 0;
        foreach (char c in enemyData)
        {
            if (c == '\n')
            {
                row--;
                col = 0;
                continue;
            }

            if (c != ' ')
            {
                if (!enemyLookup.TryGetValue(c, out GameObject template)) throw new System.Exception($"Could not find template for enemy char {c}.");
                this.EnemyData[(row, col)] = template;
            }


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

    private Transform _WallContainer, _FloorContainer, _TransitionContainer, _EnemyContainer, _ItemContainer;
    private CameraFollower _Camera;
    private Dictionary<char, GridTileSet> _TileSets = new Dictionary<char, GridTileSet>();
    private Dictionary<char, GameObject> _EnemyLookup = new Dictionary<char, GameObject>();
    private Dictionary<char, GameObject> _ItemLookup = new Dictionary<char, GameObject>();
    private HashSet<char> _IsWall = new HashSet<char>();
    private string _MapData, _RoomData, _TransitionData, _EnemyData, _ItemData;

    public MapChunkerBuilder Camera(CameraFollower camera) => SetField(ref _Camera, camera);
    public MapChunkerBuilder WallContainer(Transform wallContainer) => SetField(ref _WallContainer, wallContainer);
    public MapChunkerBuilder FloorContainer(Transform floorContainer) => SetField(ref _FloorContainer, floorContainer);
    public MapChunkerBuilder TransitionContainer(Transform transitionContainer) => SetField(ref _TransitionContainer, transitionContainer);
    public MapChunkerBuilder EnemyContainer(Transform enemyContainer) => SetField(ref _EnemyContainer, enemyContainer);
    public MapChunkerBuilder ItemContainer(Transform itemContainer) => SetField(ref _ItemContainer, itemContainer);
    public MapChunkerBuilder MapData(string mapData) => SetField(ref _MapData, mapData);
    public MapChunkerBuilder TransitionData(string transitionData) => SetField(ref _TransitionData, transitionData);
    public MapChunkerBuilder RoomData(string roomData) => SetField(ref _RoomData, roomData);
    public MapChunkerBuilder EnemyData(string enemyData) => SetField(ref _EnemyData, enemyData);
    public MapChunkerBuilder ItemData(string itemData) => SetField(ref _ItemData, itemData);
    public MapChunkerBuilder AddTileSet(char ch, GridTileSet tileSet)
    {
        if (this._TileSets.ContainsKey(ch)) throw new System.Exception($"Duplicate tile set character found: {ch}/");
        this._TileSets[ch] = tileSet;
        return this;
    }
    public MapChunkerBuilder AddEnemy(char ch, GameObject template)
    {
        if (this._EnemyLookup.ContainsKey(ch)) throw new System.Exception($"Duplicate enemy character found: {ch}/");
        this._EnemyLookup[ch] = template;
        return this;
    }

    public MapChunkerBuilder AddItem(char ch, GameObject template)
    {
        if (this._ItemLookup.ContainsKey(ch)) throw new System.Exception($"Duplicate item character found: {ch}/");
        this._ItemLookup[ch] = template;
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
                              this._TransitionContainer,
                              this._EnemyContainer,
                              this._ItemContainer,
                              this._TileSets,
                              this._IsWall,
                              this._MapData,
                              this._RoomData,
                              this._TransitionData,
                              this._EnemyData,
                              this._EnemyLookup,
                              this._ItemData,
                              this._ItemLookup);
    }

}