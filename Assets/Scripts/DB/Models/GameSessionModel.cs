using System;
using UnityEngine;

/// <summary>
/// 게임 세션 정보를 담는 데이터 모델 클래스 (각 엔티티 개별 점수 시스템)
/// </summary>
[System.Serializable]
public class GameSessionModel
{
    public int SessionID;
    public int TotalEntities;        // 참여한 총 엔티티 수
    public int PlayTimeSeconds;      // 플레이 시간 (초)
    public DateTime StartedAt;
    public DateTime? EndedAt;        // Nullable - 게임이 진행중이면 null
    public bool IsCompleted;         // 게임 완료 여부

    public GameSessionModel()
    {
        SessionID = 0;
        TotalEntities = 0;
        PlayTimeSeconds = 0;
        StartedAt = DateTime.Now;
        EndedAt = null;
        IsCompleted = false;
    }

    public GameSessionModel(int totalEntities)
    {
        SessionID = 0;
        TotalEntities = totalEntities;
        PlayTimeSeconds = 0;
        StartedAt = DateTime.Now;
        EndedAt = null;
        IsCompleted = false;
    }

    /// <summary>
    /// 게임이 진행중인지 확인
    /// </summary>
    public bool IsInProgress => !IsCompleted && EndedAt == null;

    /// <summary>
    /// 게임 완료 처리
    /// </summary>
    public void CompleteSession()
    {
        EndedAt = DateTime.Now;
        IsCompleted = true;
        PlayTimeSeconds = (int)(EndedAt.Value - StartedAt).TotalSeconds;
    }

    /// <summary>
    /// 플레이 시간을 시간:분:초 형태로 반환
    /// </summary>
    public string GetFormattedPlayTime()
    {
        int hours = PlayTimeSeconds / 3600;
        int minutes = (PlayTimeSeconds % 3600) / 60;
        int seconds = PlayTimeSeconds % 60;
        return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
    }

    /// <summary>
    /// 세션 상태 텍스트 반환
    /// </summary>
    public string GetStatusText()
    {
        if (IsCompleted)
            return $"완료 ({GetFormattedPlayTime()})";
        else if (IsInProgress)
            return "진행중";
        else
            return "대기중";
    }

    /// <summary>
    /// 현재까지의 경과 시간 계산 (초)
    /// </summary>
    public int GetElapsedSeconds()
    {
        DateTime endTime = EndedAt ?? DateTime.Now;
        return (int)(endTime - StartedAt).TotalSeconds;
    }

    /// <summary>
    /// 세션 요약 정보 반환
    /// </summary>
    public string GetSummary()
    {
        return $"세션 {SessionID}: {TotalEntities}개 엔티티, {GetStatusText()}";
    }
}
