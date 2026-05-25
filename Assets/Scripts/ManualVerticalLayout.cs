using UnityEngine;

public class ManualVerticalLayout : MonoBehaviour
{
    public float spacing = 120f;

    public void Arrange()
    {
        float y = 0;

        foreach (Transform child in transform)
        {
            RectTransform rt = child.GetComponent<RectTransform>();

            rt.anchoredPosition = new Vector2(0, -y);

            y += spacing;
        }

        // Resize content height
        GetComponent<RectTransform>().sizeDelta = new Vector2(0, y);
    }
}