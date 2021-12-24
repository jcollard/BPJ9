using System;
using System.Collections;
using System.Collections.Generic;
using CaptainCoder.Unity;
using UnityEngine;

public class SpriteLoader : MonoBehaviour
{
    public SpriteSet[] Sets;
    public int Width = 7;

    public void BuildSets()
    {
        int NextY = 0;
        int NextX = 0;
        int MaxHeight = 0;
        UnityEngineUtils.Instance.DestroyChildren(this.transform);
        foreach (SpriteSet set in this.Sets)
        {
            if (!set.IsValid)
            {
                Debug.Log($"Invalid SpriteSet expected {set.Width}x{set.Height} sprites but found {set.sprites.Length}", this);
                throw new System.Exception($"Invalid SpriteSet expected {set.Width}x{set.Height} sprites but found {set.sprites.Length}");
            }

            GameObject newSet = new GameObject(set.Name);
            newSet.transform.parent = this.transform;
            newSet.transform.localPosition = new Vector2(NextX, NextY);
            int x = 0;
            int y = 0;
            foreach (Sprite sprite in set.sprites)
            {
                GameObject nextSprite = new GameObject($"{x}, {y}");
                nextSprite.transform.parent = newSet.transform;
                nextSprite.transform.localPosition = new Vector2(x, y);
                SpriteRenderer renderer = nextSprite.AddComponent<SpriteRenderer>();
                renderer.sprite = sprite;
                x++;
                if (x >= set.Width)
                {
                    y--;
                    x = 0;
                }
            }
            MaxHeight = Mathf.Max(set.Height, MaxHeight);
            NextX += (set.Width + 1);
            if (NextX >= Width)
            {
                NextX = 0;
                NextY -= (MaxHeight + 1);
                MaxHeight = 0;
            }

        }
    }

    public Transform GetSetContainer(SpriteSet set)
    {
        for(int ix = 0; ix < this.transform.childCount; ix++)
        {
            Transform child = this.transform.GetChild(ix);
            if (child.name == set.Name)
            {
                return child;
            }
        }
        throw UnityEngineUtils.Instance.FailFast($"Set not found: {set}", this.gameObject);

    }

    public Sprite GetSprite(string SetKey, int ix)
    {
        foreach (SpriteSet set in Sets)
        {
            if (set.Name == SetKey) return set.sprites[ix];
        }
        throw UnityEngineUtils.Instance.FailFast($"SetKey not found: {SetKey}", this.gameObject);
    }

    public SpriteTemplate GetSpriteTemplate(SpriteSet set, int ix)
    {
        return new SpriteTemplate(this, set.Name, ix);
    }
}

[System.Serializable]
public class SpriteSet
{
    public string Name;
    public int Width;
    public int Height;
    public Sprite[] sprites;
    public bool IsValid => sprites.Length == Width * Height;
}


[System.Serializable]
public class SpriteTemplate
{
    public SpriteLoader Loader;

    public string SetKey;
    public int ix;

    public SpriteTemplate(SpriteLoader Loader, string SetKey, int ix)
    {
        this.Loader = Loader;
        this.SetKey = SetKey;
        this.ix = ix;
    }

    public Sprite GetSprite()
    {
        return Loader.GetSprite(SetKey, ix);
    }
}