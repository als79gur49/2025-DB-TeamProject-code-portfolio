using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class KillLogPanel : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI leftText;
    [SerializeField]
    private Image midImage;
    [SerializeField]
    private TextMeshProUGUI rightText;

    public void Setup(string lText, Sprite image, string rText)
    {
        leftText.text = lText;
        midImage.sprite = image;
        rightText.text = rText;
    }
}
