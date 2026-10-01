using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IconData : MonoBehaviour
{
    [SerializeField]
    private Image iconImage;
    [SerializeField]
    private TextMeshProUGUI iconName;
    [SerializeField]
    private TextMeshProUGUI iconDescription;

    [SerializeField]
    private Button button;
    public Button SelectButton => button;
    public void Setup(Sprite sprite, string name, string description)
    {
        iconImage.sprite = sprite;
        iconName.text = name;
        iconDescription.text = description;
    }
}
