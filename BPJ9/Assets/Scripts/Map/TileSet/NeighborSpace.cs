
using System.Collections.Generic;
using UnityEngine;

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

    public static HashSet<NeighborSpace> DiscoverCriteria(Transform target)
    {
        HashSet<NeighborSpace> newCriteria = new HashSet<NeighborSpace>();
        Transform parent = target.transform.parent;
        for (int ix = 0; ix < parent.childCount; ix++)
        {
            Transform siblingTransform = parent.GetChild(ix);
            GameObject sibling = siblingTransform.gameObject;
            if (sibling == target.gameObject) continue;
            Vector2 diff = siblingTransform.position - target.transform.position;
            if (NeighborSpaceUtil
                .SpaceLookup
                .TryGetValue(((int)diff.x, (int)diff.y), out NeighborSpace space))
            {
                newCriteria.Add(space);
            }
        }
        return newCriteria;
    }

    public static int EncodeSet(HashSet<NeighborSpace> set)
    {
        int criteria = 0;
        foreach (NeighborSpace space in set)
            criteria += (1 << (int)space);
        return criteria;
    }

    public static HashSet<NeighborSpace> DecodeSet(int encoded)
    {
        HashSet<NeighborSpace> spaces = new HashSet<NeighborSpace>();
        for (int ix = 0; ix < NeighborSpaceUtil.Spaces.Length; ix++)
        {
            int bit = (encoded >> ix) & 1;
            if (bit == 1)
            {
                spaces.Add(NeighborSpaceUtil.Spaces[ix]);
            }
        }
        return spaces;
    }
}