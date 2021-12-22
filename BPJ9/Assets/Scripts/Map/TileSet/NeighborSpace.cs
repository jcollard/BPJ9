
public enum NeighborSpace
{
    TopLeft,
    Top,
    TopRight,
    Left,
    Right,
    BottomLeft,
    Bottom,
    BottomRight
}

public class NeighborSpaceUtil
{
    public static NeighborSpace[] Spaces = {
        NeighborSpace.TopLeft,
        NeighborSpace.Top,
        NeighborSpace.TopRight,
        NeighborSpace.Left,
        NeighborSpace.Right,
        NeighborSpace.BottomLeft,
        NeighborSpace.Bottom,
        NeighborSpace.BottomRight,
    };
}