using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaptainCoder.Unity;
using UnityEditor;

public class ShapeBuilder : MonoBehaviour
{
    public string Shape;
    public Vector2 Offset;
    public GameObject[] Base;
    public Transform Container;
    public int ZLayer;
 
    public void BuildShape()
    {
        UnityEngineUtils.Instance.DestroyChildren(Container);
        int row = Shape.Split('\n').Length - 1;
        int col = 0;
        foreach (char c in Shape)
        {
            if (c == '\n')
            {
                row--;
                col = 0;
                continue;
            }

            if (c == '*')
            {
                GameObject toClone = Base[Random.Range(0, Base.Length)];
                GameObject tile = UnityEngine.Object.Instantiate(toClone);
                tile.transform.SetParent(Container);
                tile.transform.localPosition = new Vector3(col + Offset.x, row + Offset.y, ZLayer);
                tile.name = $"({row},{col})";
                tile.SetActive(true);
            }
            col++;
        }
    }
}


