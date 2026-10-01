using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LiveRankingUI : MonoBehaviour
{
    [SerializeField]
    private InfoList[] entities;
    [SerializeField]
    private InfoList player;

    private string playerName = "";

    public void Setup(string playerName)
    {
        this.playerName = playerName;
    }

    public void UpdateEntitiesRanking(List<RankingData> data)
    {
        if(entities == null || data == null)
        {
            return;
        }

        int count = Mathf.Min(entities.Length, data.Count);

        for(int i = 0; i < entities.Length; ++i)
        {
            if(i < count)
            {
                entities[i].Setup(data[i].Rank, data[i].EntityName, data[i].Score);
            }
            else
            {
                entities[i].SetupNULL();
            }
        }
    }
    public void UpdatePlayerRanking(List<RankingData> datas)
    {
        if(player == null || datas == null || datas.Count <= 0)
        {
            player.SetupNULL();

            return;
        }

        RankingData rank = datas.FirstOrDefault();

        player.Setup(rank.Rank, rank.EntityName, rank.Score, true);
    }
    public void UpdatePlayerRankingTMP(List<RankingData> datas)
    {
        foreach(var t in datas)
        {
            if(t.IsPlayer)
            {
                player.Setup(t.Rank, t.EntityName, t.Score, true);
                break;
            }
        }
    }

    public void UpdatePlayerRanking(PlayerModel data)
    {
        if (player == null)
        {
            return;
        }
        int rank = RankingManager.GetRankByScore(data.HighestScore);
        player.Setup(rank, data.PlayerName, data.HighestScore, true);

        
    }
    private void Update()
    {
        UpdateEntitiesRanking(RankingManager.GetActiveSessionLiveRanking(10));
        UpdatePlayerRankingTMP(RankingManager.GetActiveSessionLiveRanking(RankingManager.GetActiveEntityCount()));
        //UpdatePlayerRanking(RankingManager.GetPlayerLiveRanking(playerName, 1));
    }
}
