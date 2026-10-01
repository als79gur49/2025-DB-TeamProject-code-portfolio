using System;
using UnityEngine;

/// <summary>
/// 랭킹 정보를 담는 데이터 모델 클래스 (기존 구조)
/// </summary>
[System.Serializable]
public class RankingData
{
    public int EntityID;
    public string EntityName;
    public string EntityType; // "Player" 또는 "AI"
    public int Score;
    public int Level;
    public int PlayTime; // 초 단위
    public DateTime StartedAt;
    public DateTime EndedAt;
    public int Rank;

    /// <summary>
    /// 플레이어 엔티티인지 확인
    /// </summary>
    public bool IsPlayer => EntityType == "Player";

    /// <summary>
    /// AI 엔티티인지 확인
    /// </summary>
    public bool IsAI => EntityType == "AI";

    public RankingData()
    {
        EntityID = 0;
        EntityName = "";
        EntityType = "AI";
        Score = 0;
        Level = 1;
        PlayTime = 0;
    }

    public RankingData(int entityID, string entityName, int score, int rank = 0, string entityType = "AI")
    {
        EntityID = entityID;
        EntityName = entityName;
        EntityType = entityType;
        Score = score;
        Level = 1;
        PlayTime = 0;
        StartedAt = DateTime.Now;
        EndedAt = DateTime.Now;
        Rank = rank;
    }

    public RankingData(int entityID, string entityName, int score, int rank = 0)
    {
        EntityID = entityID;
        EntityName = entityName;
        Score = score;
        Level = 1;
        PlayTime = 0;
        StartedAt = DateTime.Now;
        EndedAt = DateTime.Now;
        Rank = rank;
    }

    /// <summary>
    /// 플레이 시간을 시간:분:초 형태로 반환
    /// </summary>
    public string GetFormattedPlayTime()
    {
        int hours = PlayTime / 3600;
        int minutes = (PlayTime % 3600) / 60;
        int seconds = PlayTime % 60;
        return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
    }

    /// <summary>
    /// 점수를 천단위 구분자로 포맷
    /// </summary>
    public string GetFormattedScore()
    {
        return Score.ToString("N0");
    }
}
