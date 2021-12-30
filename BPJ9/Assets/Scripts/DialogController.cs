using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class DialogController : MonoBehaviour
{
    public static DialogController Instance;
    public GameObject DialogPanel;
    public UnityEngine.UI.Text TextBox;
    public bool IsVisible => DialogPanel.activeInHierarchy;
    public bool CanContinue => DisplayQueue.Count == 0;
    public float CharDelay = .3f;
    private float LastDraw;
    private StringBuilder Builder = new StringBuilder();
    private Queue<char> DisplayQueue = new Queue<char>();
    
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        HandleQueue();
        LastDraw = Time.time;

    }

    public void ContinueDialog()
    {
        if (!CanContinue) return;
        this.DialogPanel.SetActive(false);
        ClearText();
    }

    public void HandleQueue()
    {
        if (DisplayQueue.Count == 0) return;
        
        while (DisplayQueue.Count > 0 && LastDraw < Time.time)
        {
            Builder.Append (DisplayQueue.Dequeue());
            LastDraw += CharDelay;
        }
        TextBox.text = Builder.ToString();
    }

    public void ClearText()
    {
        DisplayQueue.Clear();
        Builder.Clear();
        TextBox.text = "";
    }

    public void WriteText(string toWrite)
    {
        DisplayQueue.Clear();
        Builder.Clear();
        TextBox.text = "";
        DialogPanel.SetActive(true);
        foreach (char c in toWrite)
        {
            DisplayQueue.Enqueue(c);
        }
    }

    public void HideDialog()
    {
        DialogPanel.SetActive(false);
    }
}
