namespace CaptainCoder.Unity
{
    using UnityEngine.UI;

    [System.Serializable]
    public class TextGroup
    {
        public Text[] TextElements;

        public void SetText(string text)
        {
            foreach (Text el in this.TextElements)
            {
                el.text = text;
            }
        }
    }
}