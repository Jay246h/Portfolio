using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ToolTip : MonoBehaviour
{
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI descriptionText;
    public Image Image;

    public void SetupToolTip(string title, string description, Sprite image)
    {
        titleText.text = title;
        descriptionText.text = description;
        Image.sprite = image;
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
