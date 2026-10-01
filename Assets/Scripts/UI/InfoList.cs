using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class InfoList : MonoBehaviour
{
    [SerializeField]
    private Sprite[] medalSprites;
    [SerializeField]
    private Sprite[] backgroundSprites;

    [SerializeField]
    private Image medal;
    [SerializeField]
    private Image background;
    [SerializeField]
    private TextMeshProUGUI userRanking;
    [SerializeField]
    private TextMeshProUGUI userName;
    [SerializeField]
    private TextMeshProUGUI userScore;

    public void SetupNULL()
    {
        userRanking.text = string.Empty;
        userName.text = string.Empty;
        userScore.text = string.Empty;
    }

    public void Setup(int ranking, string name, int score, bool isPlayer = false)
    {
        userRanking.text = ranking.ToString();
        userName.text = name;
        userScore.text = score.ToString();

        if(ranking == 1)
        {
            medal.sprite = medalSprites[0];

            background.sprite = backgroundSprites[0];
        }
        else if(ranking == 2)
        {
            medal.sprite = medalSprites[1];

            background.sprite = backgroundSprites[1];
        }
        else if(ranking == 3)
        {
            medal.sprite = medalSprites[2];

            background.sprite = backgroundSprites[2];
        }
        else
        {
            medal.sprite = medalSprites[2];
            medal.color = new Color(0, 0, 0, 0);
            
            background.sprite = backgroundSprites[3];
        }

        if(isPlayer)
        {
            background.sprite = backgroundSprites[4];
        }
    }

}
