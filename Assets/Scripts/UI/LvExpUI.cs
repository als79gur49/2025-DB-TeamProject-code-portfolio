using UnityEngine.UI;
using UnityEngine;
using TMPro;

public class LvExpUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI level;
    [SerializeField]
    private TextMeshProUGUI expText;
    [SerializeField]
    private Slider expSlider;

    public void SetLevel(int level)
    {
        this.level.text = level.ToString();
    }
    public void SetExperience(int current, int max)
    {
        expText.text = $"{current} / {max}";
        expSlider.value = (float)current / max;
    }

}
