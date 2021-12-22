using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A GridTile is a tile that is placed in the world based on its surrounding 
/// neighbors. It specifies the variations that can be used as well as the 
/// criteria necessary to place the element.
/// </summary>
public class GridTile : MonoBehaviour
{
    /// <summary>
    /// The variations of this tile that can be used.
    /// </summary>
    public GameObject[] Templates;

    public bool IsFloor;

    [SerializeField]
    [ReadOnly]
    private int _Criteria;

    /// <summary>
    /// The bit encoded criteria for selecting this tile
    /// </summary>
    public int Criteria { get; private set; }

    /// <summary>
    /// The non-encoded NeighborSpace set which must be met to place this
    /// tile.
    /// </summary>
    public HashSet<NeighborSpace> CriteriaSet
    {
        get => NeighborSpaceUtil.DecodeSet(this._Criteria);
        set => this._Criteria = NeighborSpaceUtil.EncodeSet(value);
    }
}
