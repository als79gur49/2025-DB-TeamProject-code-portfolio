using System;
using UnityEngine;

/// <summary>
/// 엔티티 정보를 담는 데이터 모델 클래스
/// Player와 AI Enemy 모두 포함
/// </summary>
[System.Serializable]
public class EntityModel
{
    public int EntityID;
    public string EntityName;
    public string EntityType; // "Player" 또는 "AI"
    public int? PlayerID; // Player인 경우만 값이 있음
    public DateTime CreatedAt;

    public EntityModel()
    {
        EntityID = 0;
        EntityName = "";
        EntityType = "AI";
        PlayerID = null;
        CreatedAt = DateTime.Now;
    }

    public EntityModel(string entityName, string entityType, int? playerID = null)
    {
        EntityID = 0;
        EntityName = entityName;
        EntityType = entityType;
        PlayerID = playerID;
        CreatedAt = DateTime.Now;
    }

    /// <summary>
    /// 플레이어 엔티티인지 확인
    /// </summary>
    public bool IsPlayer => EntityType == "Player";

    /// <summary>
    /// AI 엔티티인지 확인
    /// </summary>
    public bool IsAI => EntityType == "AI";
}

/// <summary>
/// 세션 내 엔티티 정보를 담는 데이터 모델 클래스
/// </summary>
[System.Serializable]
public class SessionEntityModel
{
    public int SessionEntityID;
    public int SessionID;
    public int EntityID;
    public string EntityName;
    public string EntityType;
    public int Score;
    public int Level;
    public int EnemiesKilled;
    public bool IsAlive;
    public int? FinalRank;
    public DateTime JoinedAt;
    public DateTime? DiedAt;

    public SessionEntityModel()
    {
        SessionEntityID = 0;
        SessionID = 0;
        EntityID = 0;
        EntityName = "";
        EntityType = "AI";
        Score = 0;
        Level = 1;
        EnemiesKilled = 0;
        IsAlive = true;
        FinalRank = null;
        JoinedAt = DateTime.Now;
        DiedAt = null;
    }

    public SessionEntityModel(int sessionId, int entityId, string entityName, string entityType)
    {
        SessionEntityID = 0;
        SessionID = sessionId;
        EntityID = entityId;
        EntityName = entityName;
        EntityType = entityType;
        Score = 0;
        Level = 1;
        EnemiesKilled = 0;
        IsAlive = true;
        FinalRank = null;
        JoinedAt = DateTime.Now;
        DiedAt = null;
    }

    /// <summary>
    /// 엔티티 사망 처리
    /// </summary>
    public void SetDead(int finalRank)
    {
        IsAlive = false;
        FinalRank = finalRank;
        DiedAt = DateTime.Now;
    }

    /// <summary>
    /// 플레이어 엔티티인지 확인
    /// </summary>
    public bool IsPlayer => EntityType == "Player";

    /// <summary>
    /// AI 엔티티인지 확인
    /// </summary>
    public bool IsAI => EntityType == "AI";
}
