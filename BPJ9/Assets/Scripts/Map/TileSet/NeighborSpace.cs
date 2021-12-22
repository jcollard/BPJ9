
using System.Collections.Generic;

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

    public static readonly Dictionary<(int, int), NeighborSpace> SpaceLookup;

    static NeighborSpaceUtil()
    {
        SpaceLookup = new Dictionary<(int, int), NeighborSpace>();
        SpaceLookup[(-1, 1)] = NeighborSpace.TopLeft;
        SpaceLookup[(0, 1)] = NeighborSpace.Top;
        SpaceLookup[(1, 1)] = NeighborSpace.TopRight;
        SpaceLookup[(-1, 0)] = NeighborSpace.Left;
        SpaceLookup[(1, 0)] = NeighborSpace.Right;
        SpaceLookup[(-1, -1)] = NeighborSpace.BottomLeft;
        SpaceLookup[(0, -1)] = NeighborSpace.Bottom;
        SpaceLookup[(1, -1)] = NeighborSpace.BottomRight;
        
    }
    
}