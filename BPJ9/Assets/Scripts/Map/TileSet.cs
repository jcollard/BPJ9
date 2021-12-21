using UnityEngine;
using System.Collections.Generic;

public class TileSet : MonoBehaviour
{
    public List<GameObject> TopLeft = new List<GameObject>();
    public List<GameObject> Top = new List<GameObject>();
    public List<GameObject> TopRight = new List<GameObject>();
    public List<GameObject> Left = new List<GameObject>();
    public List<GameObject> Middle = new List<GameObject>();
    public List<GameObject> Right = new List<GameObject>();
    public List<GameObject> BottomLeft = new List<GameObject>();
    public List<GameObject> Bottom = new List<GameObject>();
    public List<GameObject> BottomRight = new List<GameObject>();

    public GameObject GetTileTemplate(TilePosition position, System.Random RNG = null)
    {
        List<GameObject>[] hack = { TopLeft, Top, TopRight, Left, Middle, Right, BottomLeft, Bottom, BottomRight };
        List<GameObject> options = hack[(int)position];
        int ix = RNG == null ? Random.Range(0, options.Count) : RNG.Next(0, options.Count);
        return options[ix];
    }

    public void FindTiles()
    {

        List<GameObject>[] hack = { TopLeft, Top, TopRight, Left, Middle, Right, BottomLeft, Bottom, BottomRight };
        foreach (List<GameObject> container in hack)
        {
            container.Clear();
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform child = transform.GetChild(i);
            int ix = ((int)child.localPosition.x) + ((int)-(child.localPosition.y)) * 3;

            hack[ix].Add(child.gameObject);
        }
    }
}



public enum TilePosition
{
    TopLeft,
    Top,
    TopRight,
    Left,
    Middle,
    Right,
    BottomLeft,
    Bottom,
    BottomRight
}