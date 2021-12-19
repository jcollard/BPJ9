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
    public TileDefinition[] Definitions;
    public bool IsDirty;
    private Dictionary<char, GameObject> _Templates;
    private Dictionary<char, GameObject> Templates => GetTemplates();
    public Transform Container;
    public int ZLayer;

    private Dictionary<char, GameObject> GetTemplates()
    {
        if (IsDirty || _Templates == null)
        {
            _Templates = new Dictionary<char, GameObject>();
            foreach (TileDefinition def in Definitions)
            {
                if (_Templates.ContainsKey(def.TextRepresentation))
                {
                    throw new System.Exception($"Duplicate definition for {def.TextRepresentation} found while building map.");
                }
                _Templates[def.TextRepresentation] = def.Template;
            }
        }
        IsDirty = false;
        return _Templates;
    }

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

            if (c == ' ')
            {
                if (Base.Length > 0)
                {
                    GameObject baseTile = Base[Random.Range(0, Base.Length)];
                    AddTile(row, col, baseTile);
                }
                col++;
                continue;
            }

            if (!Templates.ContainsKey(c))
            {
                throw new System.Exception($"No definition for {c} found while building grid.");
            }

            GameObject toClone = Templates[c];
            AddTile(row, col, toClone);
            col++;
        }
    }

    private void AddTile(int row, int col, GameObject toClone)
    {
        Spawner.SpawnObject(toClone)
               .LocalPosition(new Vector2(col + Offset.x, row + Offset.y))
               .Parent(Container)
               .Name($"({row},{col})")
               .Spawn();
    }
}


[System.Serializable]
public class TileDefinition
{
    public char TextRepresentation;
    public GameObject Template;
}