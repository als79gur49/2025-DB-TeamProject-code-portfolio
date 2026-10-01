using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [SerializeField]
    private InfoList infoPrefab;
    // 랭킹 정보들의 부모Transform
    [SerializeField]
    private Transform ListParent;

    // 플레이어 랭킹 정보 UI(개인)
    [SerializeField]
    private InfoList player;

    private int showInfoNum = 10;

    private RectTransform rect;
    private float duration = 1.5f;

    private string playerName;
    private int sessionId;

    private int playerId2;
    private int sessionId2;
    public void Setup(string playerName, int sessionId)
    {
        this.playerName = playerName;
        this.sessionId = sessionId;

        playerId2 = GameSessionManager.GetCurrentPlayerEntityId();
        sessionId2 = GameSessionManager.GetCurrentSessionId();
    }

    public void ShowGameOver()
    {
        MoveAnimation();
        ShowEntityInfo();
        ShowPlayerInfo();
    }

    // 아래에서 위로 올라오는 애니메이션
    private void MoveAnimation()
    {
        rect = GetComponent<RectTransform>();
        rect.DOAnchorPosY(1000, duration).From().SetEase(Ease.OutBack);
    }

    // 전체 랭킹 
    private void ShowEntityInfo()
    {
        //List<RankingData> list = RankingRepository.GetTopRankings(showInfoNum);
        //List<RankingData> list = RankingManager.GetLiveRanking(showInfoNum);
        
        //List<RankingData> list = RankingManager.GetSessionEndRanking(sessionId, showInfoNum);
        List<RankingData> list = RankingManager.GetSessionEndRanking(sessionId, showInfoNum);

        foreach (RankingData rankingData in list)
        {
            InfoList clone = Instantiate(infoPrefab);
            clone.transform.SetParent(ListParent, false);

            clone.Setup(rankingData.Rank, rankingData.EntityName, rankingData.Score);
        }

    }
    
    // 플레이어 랭킹
    private void ShowPlayerInfo()
    {
        List<RankingData> list = RankingManager.GetSessionEndRanking(sessionId, GameSessionManager.GetSession(sessionId2).TotalEntities);
        foreach (RankingData rankingData in list)
        {
            // 위치 변경, 따로 플레이어 랭킹만 찾을려고 여러 DB에서 가져와서 사용해봤지만, 랭킹이 제대로 표시X
            if (rankingData.IsPlayer)
            {
                player.Setup(rankingData.Rank, rankingData.EntityName, rankingData.Score);

                break;
            }
        }

        return;
    }
}
