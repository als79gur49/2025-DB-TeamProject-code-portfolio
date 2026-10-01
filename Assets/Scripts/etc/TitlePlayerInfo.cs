using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;
public class TitlePlayerInfo : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI playerName;
    [SerializeField]
    private TextMeshProUGUI playerId;

    [SerializeField]
    private TextMeshProUGUI highestScore;
    [SerializeField]
    private TextMeshProUGUI totalPlayTime;
    [SerializeField]
    private TextMeshProUGUI totalGames;
    [SerializeField]
    private TextMeshProUGUI createdAt;
    [SerializeField]
    private TextMeshProUGUI lastPlayedAt;

    private void Awake()
    {
        
    }

    private void Start()
    {   
        // 최고 점수
        RankingData highestData = RankingManager.GetPlayerBestRanking(1).FirstOrDefault();
        if (highestScore != null || highestData != null)
        {
            highestScore.text = highestData.Score.ToString();
        }

        // 누적 플레이 시간
        if(totalPlayTime != null) 
        {
            totalPlayTime.text = RankingManager.GetPlayerTotalPlayTime().ToString();
        }

        if(totalGames != null) 
        {
            totalGames.text = RankingManager.GetTotalGameCount().ToString();    
        }


        if(createdAt != null) 
        {
            // null -> ""
            createdAt.text = RankingManager.GetCreatedAt().ToString();
        }

        if(lastPlayedAt != null)
        {
            lastPlayedAt.text = RankingManager.GetLastPlayedAt().ToString();
        }
    }
}
