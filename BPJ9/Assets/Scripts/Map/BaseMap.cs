using System.Collections;
using System.Collections.Generic;
using CaptainCoder.Unity;
using UnityEngine;
using System.Linq;

public class BaseMap : MonoBehaviour
{
    public Transform FloorContainer, WallContainer, Player;
    public MapTileDefinition[] Definitions;
    public MapChunker Chunker;
    public string MapData;

    public void Start()
    {
        // TODO: Consider not calling this on start as it will rebuild when
        // the scene changes.
        Init();
        Chunker.BuildChunk();
    }

    public void Update()
    {
        Chunker.CheckAndBuildChunk();
    }

    public void Init()
    {
        MapChunkerBuilder builder = MapChunkerBuilder.Instantiate(this.Player)
                                                     .WallContainer(this.WallContainer)
                                                     .FloorContainer(this.FloorContainer)
                                                     .MapData(this.MapData)
                                                     .Width(5)
                                                     .Height(5);
        foreach (MapTileDefinition def in Definitions)
            builder.AddTileSet(def.FloorCharacter, def.TileSet)
                   .AddTileSet(def.WallCharacter, def.TileSet)
                   .AddWallChar(def.WallCharacter);

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