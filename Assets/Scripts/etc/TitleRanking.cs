using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class TitleRanking : MonoBehaviour
{
    [SerializeField]
    private Transform ListParent;
    [SerializeField]
    private InfoListDateTime prefab;

    [SerializeField]
    private int showInfoNum = 10;
    private List<InfoListDateTime> rankingLists;

    [SerializeField]
    private bool isShowAllEntity = false;

    private void Awake()
    {
        rankingLists = new List<InfoListDateTime>();
    }

    private void Start()
    {
        ShowRanking();
    }

    public void ShowRanking()
    {
        if(isShowAllEntity)
        {
            ShowAllRankings(showInfoNum);
        }
        else
        {
            ShowPlayerRankings(showInfoNum);    
        }
    }

    private void ShowPlayerRankings(int showInfoNum)
    {
        // 플레이어별 최고 점수 랭킹 (AI 제외)
        var playerRanking = RankingManager.GetPlayerBestRanking(10);

        // 부족한 개수만큼 추가
        int num = Mathf.Min(playerRanking.Count, showInfoNum);
        // 부족한 만큼만 생성
        while (rankingLists.Count < num)
        {
            rankingLists.Add(Instantiate(prefab, ListParent));
        }


        Debug.Log($"{num} {playerRanking.Count} {rankingLists.Count} {showInfoNum}");
        for (int i = 0; i < num; ++i)
        {
            RankingData rank = playerRanking[i];

            rankingLists[i].Setup(rank.Rank, rank.EntityName, rank.Score, rank.EndedAt);
        }
    }

    private void ShowAllRankings(int showInfoNum)
    {
        // 완료된 게임들의 모든 엔티티 최고 점수 랭킹
        var allTimeRanking = RankingManager.GetAllTimeRanking(10);

        // 부족한 개수만큼 추가
        int num = Mathf.Min(allTimeRanking.Count, showInfoNum);
        // 부족한 만큼만 생성
        while (rankingLists.Count < num)
        {
            rankingLists.Add(Instantiate(prefab, ListParent));
        }


        for(int i = 0; i < num; ++i)
        {
            RankingData rank = allTimeRanking[i];

            rankingLists[i].Setup(rank.Rank, rank.EntityName, rank.Score, rank.EndedAt);
        }
    }
}
