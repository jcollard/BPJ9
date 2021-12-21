
public class LavaTileSelector : TileSelector
{
    // TilePostion, UseWallType
    public override (TilePosition, bool) GetTilePosition(int type)
    {
        return (TilePosition.Middle, true);
    }
}