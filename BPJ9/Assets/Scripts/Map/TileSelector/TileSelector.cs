using System.Collections.Generic;
[System.Serializable]
public class TileSelector
{
    public TileSelectorStrategy Strategy = TileSelectorStrategy.Default;

    private readonly static Dictionary<TileSelectorStrategy, TileSelector> Strategies = new Dictionary<TileSelectorStrategy, TileSelector>();

    static TileSelector()
    {
        Strategies.Add(TileSelectorStrategy.Default, new DefaultTileSelector());
        Strategies.Add(TileSelectorStrategy.NoInner, new NoInnerTileSelector());
        Strategies.Add(TileSelectorStrategy.Lava, new LavaTileSelector());
    }

    public virtual (TilePosition, bool) GetTilePosition(int type)
    {
        return Strategies[this.Strategy].GetTilePosition(type);
    }
}

public enum TileSelectorStrategy
{
    Default,
    NoInner,
    Lava
}