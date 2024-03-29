using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CaptainCoder.Unity;
using UnityEditor;

public class GridBuilder : MonoBehaviour
{

    public GameObject[] Base;

    public Transform Container;
    public int Rows;
    public int Cols;
    public int ZLayer;

    public void BuildGrid()
    {
        int Left = -(Rows / 2);
        int Right = (Rows / 2) + (Rows % 2);
        int Bottom = -(Cols / 2);
        int Top = (Cols / 2) + (Cols % 2);
        UnityEngineUtils.Instance.DestroyChildren(Container);
        for (int r = Bottom; r < Top; r++)
        {
            for (int c = Left; c < Right; c++)
            {
                GameObject toClone = Base[Random.Range(0, Base.Length)];
                Spawner.SpawnObject(toClone)
                       .LocalPosition(new Vector2(r, c))
                       .Parent(Container)
                       .Name($"({r},{c})")
                       .Spawn();
            }
        }
    }
}

