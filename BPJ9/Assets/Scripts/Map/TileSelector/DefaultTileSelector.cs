
public class DefaultTileSelector : TileSelector
{
    // TilePostion, UseWallType
    public override (TilePosition, bool) GetTilePosition(int type)
    {
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
}