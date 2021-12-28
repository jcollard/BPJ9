using System.Collections;
using System.Collections.Generic;
using CaptainCoder.Unity;
using UnityEngine;
using System.Linq;

public class BaseMap : MonoBehaviour
{
    public Transform FloorContainer, WallContainer, TransitionContainer, EnemyContainer, Player;
    public MapTileDefinition[] Definitions;
    public EnemyDefinition[] Enemies;
    public MapChunker Chunker;
    public CameraFollower MainCamera;
    public string MapData, RoomData, TransitionData, EnemyData;

    public void Awake()
    {
        // TODO: Consider not calling this on start as it will rebuild when
        // the scene changes.
        Init();
        // Chunker.BuildNextChunk();
    }

    public void Init()
    {
        
        MapChunkerBuilder builder = MapChunkerBuilder.Instantiate(this.MainCamera)
                                                     .WallContainer(this.WallContainer)
                                                     .FloorContainer(this.FloorContainer)
                                                     .TransitionContainer(this.TransitionContainer)
                                                     .EnemyContainer(this.EnemyContainer)
                                                     .MapData(this.MapData)
                                                     .RoomData(this.RoomData)
                                                     .TransitionData(this.TransitionData)
                                                     .EnemyData(this.EnemyData);
        foreach (MapTileDefinition def in Definitions)
            builder.AddTileSet(def.FloorCharacter, def.TileSet)
                   .AddTileSet(def.WallCharacter, def.TileSet)
                   .AddWallChar(def.WallCharacter);

        foreach (EnemyDefinition def in Enemies)
            builder.AddEnemy(def.EnemyCharacter, def.Template);

        this.Chunker = builder.Build();
    }

    public void DestroyMap()
    {
        UnityEngineUtils.Instance.DestroyChildren(WallContainer);
        UnityEngineUtils.Instance.DestroyChildren(FloorContainer);
    }
}

[System.Serializable]
public class MapTileDefinition
{
    public char FloorCharacter;
    public char WallCharacter;

    public GridTileSet TileSet;
}

[System.Serializable]
public class EnemyDefinition
{
    public string name;
    public char EnemyCharacter;
    public GameObject Template;
}