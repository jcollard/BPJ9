using System.Collections;
using System.Collections.Generic;
using CaptainCoder.Unity;
using UnityEngine;
using UnityEngine.UI;

public class HeartController : MonoBehaviour
{
    public List<Image> Hearts;
    public Image HeartTemplate;
    public Transform HeartContainer;
    public Sprite FullHeart, HalfHeart, EmptyHeart;
    public int Width = 32;

    public void UpdateHearts(float HP, int MaxHP)
    {
        if (Hearts.Count != Mathf.Ceil((float)MaxHP/2))
        {
            this.RebuildHearts((int)Mathf.Ceil((float)MaxHP/2));
        }
        float percent = HP / (float)MaxHP;
        int halves = (int)Mathf.Ceil((float)MaxHP * percent);
        int fulls = halves/2;
        for (int i = 0; i < Hearts.Count; i++)
        {
            if (fulls > i) Hearts[i].sprite = FullHeart;
            else if (halves > i * 2) Hearts[i].sprite = HalfHeart;
            else Hearts[i].sprite = EmptyHeart;
        }

    } 

    private void RebuildHearts(int count)
    {
        UnityEngineUtils.Instance.DestroyChildren(HeartContainer);
        this.Hearts.Clear();
        for (int i = 0; i < count; i++)
        {
            Image newHeart = UnityEngine.Object.Instantiate<Image>(HeartTemplate);
            newHeart.transform.SetParent(HeartContainer, false);
            RectTransform transform = newHeart.GetComponent<RectTransform>();

            transform.anchoredPosition = new Vector2(i*Width, 0);
            newHeart.gameObject.SetActive(true);
            this.Hearts.Add(newHeart);
        }
    }

}
