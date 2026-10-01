using System;
using UnityEngine;

/// <summary>
/// 플레이어 정보를 담는 데이터 모델 클래스 (레벨 시스템 제외)
/// </summary>
[System.Serializable]
public class PlayerModel
{
    public int PlayerID;
    public string PlayerName;
    public int HighestScore;
    public int TotalPlayTime; // 초 단위
    public int TotalGames;    // 총 게임 횟수
    public DateTime CreatedAt;
    public DateTime LastPlayedAt;

    // 향후 확장용 (현재 미사용)
    public int CurrentLevel = 1;
    public int CurrentExp = 0;
    public int ExpToNextLevel = 100;

    public PlayerModel()
    {
        PlayerID = 0;
        PlayerName = "";
        HighestScore = 0;
        TotalPlayTime = 0;
        TotalGames = 0;
        CreatedAt = DateTime.Now;
        LastPlayedAt = DateTime.Now;
        
        // 향후 확장용 기본값
        CurrentLevel = 1;
        CurrentExp = 0;
        ExpToNextLevel = 100;
    }

    public PlayerModel(string playerName)
    {
        PlayerID = 0;
        PlayerName = playerName;
        HighestScore = 0;
        TotalPlayTime = 0;
        TotalGames = 0;
        CreatedAt = DateTime.Now;
        LastPlayedAt = DateTime.Now;
        
        // 향후 확장용 기본값
        CurrentLevel = 1;
        CurrentExp = 0;
        ExpToNextLevel = 100;
    }

    /// <summary>
    /// 총 플레이 시간을 시간:분:초 형태로 반환
    /// </summary>
    public string GetFormattedPlayTime()
    {
        int hours = TotalPlayTime / 3600;
        int minutes = (TotalPlayTime % 3600) / 60;
        int seconds = TotalPlayTime % 60;
        return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
    }

    /// <summary>
    /// 게임 플레이 횟수 증가
    /// </summary>
    public void IncrementGameCount()
    {
        TotalGames++;
        LastPlayedAt = DateTime.Now;
    }

    /// <summary>
    /// 플레이 시간 추가 (초 단위)
    /// </summary>
    public void AddPlayTime(int seconds)
    {
        TotalPlayTime += seconds;
        LastPlayedAt = DateTime.Now;
    }
}