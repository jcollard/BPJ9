using System.Collections;
using System.Collections.Generic;
using CaptainCoder.Unity;
using UnityEngine;
using System.Linq;

public class BaseMap : MonoBehaviour
{
    public Transform FloorContainer, WallContainer, TransitionContainer, EnemyContainer, ItemContainer, Player;
    public MapTileDefinition[] Definitions;
    public EnemyDefinition[] Enemies;
    public ItemDefinition[] Items;
    public MapChunker Chunker;
    public CameraFollower MainCamera;
    public string MapData, RoomData, TransitionData, EnemyData, ItemData;

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
                                                     .ItemContainer(this.ItemContainer)
                                                     .MapData(this.MapData)
                                                     .RoomData(this.RoomData)
                                                     .TransitionData(this.TransitionData)
                                                     .EnemyData(this.EnemyData)
                                                     .ItemData(this.ItemData);
                                                     
        foreach (MapTileDefinition def in Definitions)
            builder.AddTileSet(def.FloorCharacter, def.TileSet)
                   .AddTileSet(def.WallCharacter, def.TileSet)
                   .AddWallChar(def.WallCharacter);

        foreach (EnemyDefinition def in Enemies)
            builder.AddEnemy(def.EnemyCharacter, def.Template);

        foreach (ItemDefinition def in Items)
            builder.AddItem(def.ItemCharacter, def.Template);

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

[System.Serializable]
public class ItemDefinition
{
    public string name;
    public char ItemCharacter;
    public GameObject Template;
}