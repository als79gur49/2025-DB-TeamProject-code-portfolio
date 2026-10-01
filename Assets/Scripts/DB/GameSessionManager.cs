using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// 게임 세션 통합 관리 클래스
/// 각 엔티티(Player + AI) 개별 점수 기록 지원
/// </summary>
public static class GameSessionManager
{
    private static int currentSessionId = -1;
    private static int currentPlayerEntityId = -1;

    /// <summary>
    /// 새로운 게임 세션 시작 (플레이어 포함)
    /// </summary>
    public static GameSessionModel StartSession(string playerName)
    {
        try
        {
            // 1. 플레이어 생성 또는 조회
            var player = PlayerRepository.CreatePlayer(playerName);
            if (player == null)
            {
                Debug.LogError("플레이어 생성에 실패했습니다.");
                return null;
            }

            // 2. 플레이어 엔티티 생성
            var playerEntity = EntityRepository.CreatePlayerEntity(player.PlayerID, playerName);

            if (playerEntity == null)
            {
                Debug.LogError("플레이어 엔티티 생성에 실패했습니다.");
                return null;
            }

            // 3. 게임 세션 시작 (새로운 구조)
            var session = GameSessionRepository.StartNewSession();
            if (session == null)
            {
                Debug.LogError("게임 세션 시작에 실패했습니다.");
                return null;
            }

            // 4. 세션에 플레이어 엔티티 추가
            var sessionEntity = SessionEntityRepository.AddEntityToSession(
                session.SessionID,
                playerEntity.EntityID,
                playerEntity.EntityName,
                playerEntity.EntityType);

            if (sessionEntity == null)
            {
                Debug.LogError("세션에 플레이어 추가에 실패했습니다.");
                return null;
            }

            // 5. 현재 세션 정보 저장
            currentSessionId = session.SessionID;
            currentPlayerEntityId = playerEntity.EntityID;

            // 6. 실시간 랭킹에 추가
            RankingManager.AddToLiveRanking(session.SessionID, playerEntity.EntityID, playerEntity.EntityName, playerEntity.EntityType);

            Debug.Log($"게임 세션 시작: SessionID={session.SessionID}, PlayerEntity={playerEntity.EntityName} (ID={playerEntity.EntityID})");
            return session;
        }
        catch (Exception ex)
        {
            Debug.LogError($"게임 세션 시작 오류: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 세션에 AI 엔티티 추가
    /// </summary>
    public static bool AddAIEntityToSession(string aiName)
    {
        if (currentSessionId == -1)
        {
            Debug.LogWarning("활성 세션이 없습니다.");
            return false;
        }

        try
        {
            // 1. AI 엔티티 생성
            var aiEntity = EntityRepository.CreateAIEntity(aiName);
            if (aiEntity == null)
            {
                Debug.LogError($"AI 엔티티 생성 실패: {aiName}");
                return false;
            }

            // 2. 세션에 AI 엔티티 추가
            var sessionEntity = SessionEntityRepository.AddEntityToSession(
                currentSessionId,
                aiEntity.EntityID,
                aiEntity.EntityName,
                aiEntity.EntityType);

            if (sessionEntity == null)
            {
                Debug.LogError($"세션에 AI 추가 실패: {aiName}");
                return false;
            }

            // 3. 실시간 랭킹에 추가
            RankingManager.AddToLiveRanking(currentSessionId, aiEntity.EntityID, aiEntity.EntityName, aiEntity.EntityType);

            Debug.Log($"<color=green>AI 엔티티 추가: {aiName} (ID={aiEntity.EntityID})</color>");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"AI 엔티티 추가 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 엔티티 점수 추가 (Player + AI 모두 지원)
    /// </summary>
    public static bool AddEntityScore(int entityId, int newScore, int newLevel = -1)
    {
        if (currentSessionId == -1)
        {
            Debug.LogWarning("활성 세션이 없습니다.");
            return false;
        }

        try
        {
            // 세션 엔티티 점수 추가
            bool entityUpdated = SessionEntityRepository.AddEntityScore(currentSessionId, entityId, newScore, newLevel);

            // 실시간 랭킹 업데이트
            bool rankingUpdated = RankingManager.UpdateLiveRanking(currentSessionId, entityId, newScore, newLevel > 0 ? newLevel : -1);

            return entityUpdated && rankingUpdated;
        }
        catch (Exception ex)
        {
            Debug.LogError($"엔티티 점수 추가 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 엔티티 점수 업데이트
    /// </summary>
    public static bool UpdateEntityScore(int entityId, int newScore, int newLevel = -1)
    {
        if (currentSessionId == -1)
        {
            Debug.LogWarning("활성 세션이 없습니다.");
            return false;
        }

        try
        {
            // 세션 엔티티 점수 업데이트
            bool entityUpdated = SessionEntityRepository.UpdateEntityScore(currentSessionId, entityId, newScore, newLevel);

            // 실시간 랭킹 업데이트
            bool rankingUpdated = RankingManager.UpdateLiveRanking(currentSessionId, entityId, newScore, newLevel > 0 ? newLevel : -1);

            return entityUpdated && rankingUpdated;
        }
        catch (Exception ex)
        {
            Debug.LogError($"엔티티 점수 업데이트 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 현재 플레이어 점수 추가
    /// </summary>
    public static bool AddPlayerScore(int newScore, int newLevel = -1)
    {
        if (currentPlayerEntityId == -1)
        {
            Debug.LogWarning("활성 플레이어 엔티티가 없습니다.");
            return false;
        }

        return AddEntityScore(currentPlayerEntityId, newScore, newLevel);
    }

    /// <summary>
    /// 현재 플레이어 점수 업데이트
    /// </summary>
    public static bool UpdatePlayerScore(int newScore, int newLevel = -1)
    {
        if (currentPlayerEntityId == -1)
        {
            Debug.LogWarning("활성 플레이어 엔티티가 없습니다.");
            return false;
        }

        return UpdateEntityScore(currentPlayerEntityId, newScore, newLevel);
    }

    /// <summary>
    /// 엔티티 처치 시 처리 (처치한 엔티티 점수 증가)
    /// </summary>
    public static bool OnEntityKilled(int killerEntityId, string killedEntityName, int scoreGain = 100)
    {
        if (currentSessionId == -1)
        {
            Debug.LogWarning("활성 세션이 없습니다.");
            return false;
        }

        try
        {
            // 처치한 엔티티의 점수 및 처치 수 증가
            bool updated = SessionEntityRepository.IncrementEntityKills(currentSessionId, killerEntityId, scoreGain);

            if (updated)
            {
                var sessionEntity = SessionEntityRepository.GetSessionEntity(currentSessionId, killerEntityId);
                if (sessionEntity != null)
                {
                    // 실시간 랭킹 업데이트
                    RankingManager.UpdateLiveRanking(currentSessionId, killerEntityId, sessionEntity.Score, sessionEntity.Level);

                    Debug.Log($"{sessionEntity.EntityName}이 {killedEntityName}을 처치! +{scoreGain}점 (총 점수: {sessionEntity.Score}, 처치 수: {sessionEntity.EnemiesKilled})");
                }
            }

            return updated;
        }
        catch (Exception ex)
        {
            Debug.LogError($"엔티티 처치 처리 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 엔티티 사망 처리 (세션 종료 없이 사망만 처리)
    /// </summary>
    public static bool OnEntityDeath(int entityId)
    {
        if (currentSessionId == -1)
        {
            Debug.LogWarning("활성 세션이 없습니다.");
            return false;
        }

        try
        {
            // 엔티티 사망 처리
            bool entityUpdated = SessionEntityRepository.SetEntityDead(currentSessionId, entityId, 0);

            // 실시간 랭킹에서 제거
            bool rankingUpdated = RankingManager.RemoveFromLiveRanking(currentSessionId, entityId);

            Debug.Log($"<color=red>엔티티 사망 처리 완료 (Entity ID: {entityId})</color>");

            return entityUpdated && rankingUpdated;
        }
        catch (Exception ex)
        {
            Debug.LogError($"엔티티 사망 처리 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 플레이어 사망 처리 (사망만 처리, 세션 종료 안함)
    /// </summary>
    public static bool HandlePlayerDeath()
    {
        if (currentPlayerEntityId == -1)
        {
            Debug.LogWarning("활성 플레이어 엔티티가 없습니다.");
            return false;
        }

        return OnEntityDeath(currentPlayerEntityId);
    }

    /// <summary>
    /// 플레이어 사망으로 인한 세션 종료 (별도 함수)
    /// </summary>
    public static bool EndSessionByPlayerDeath()
    {
        if (currentSessionId == -1)
        {
            Debug.LogWarning("활성 세션이 없습니다.");
            return false;
        }

        try
        {
            Debug.Log("플레이어 사망으로 인해 게임 세션을 종료합니다.");
            return EndSession();
        }
        catch (Exception ex)
        {
            Debug.LogError($"플레이어 사망 세션 종료 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 플레이어 사망 통합 처리 (사망 처리 + 세션 종료)
    /// </summary>
    public static bool OnPlayerDeath()
    {
        try
        {
            // 1단계: 플레이어 사망 처리
            bool deathHandled = HandlePlayerDeath();

            if (!deathHandled)
            {
                Debug.LogError("플레이어 사망 처리에 실패했습니다.");
                return false;
            }

            // 2단계: 세션 종료 처리
            bool sessionEnded = EndSessionByPlayerDeath();

            if (!sessionEnded)
            {
                Debug.LogError("플레이어 사망 후 세션 종료에 실패했습니다.");
                return false;
            }

            Debug.Log("플레이어 사망 및 세션 종료가 완료되었습니다.");
            return true;
        }
        catch (Exception ex)
        {
            Debug.LogError($"플레이어 사망 통합 처리 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 게임 세션 종료
    /// </summary>
    public static bool EndSession()
    {
        if (currentSessionId == -1)
        {
            Debug.LogWarning("활성 세션이 없습니다.");
            return false;
        }

        try
        {
            // 세션의 모든 엔티티 조회
            var sessionEntities = SessionEntityRepository.GetSessionEntities(currentSessionId);

            // 플레이어 최고 점수 및 게임 통계 업데이트
            var playerEntity = sessionEntities.Find(e => e.EntityID == currentPlayerEntityId);
            if (playerEntity != null && playerEntity.EntityType == "Player")
            {
                var entity = EntityRepository.GetEntityById(playerEntity.EntityID);
                if (entity != null && entity.PlayerID.HasValue)
                {
                    // 최고 점수 업데이트
                    PlayerRepository.UpdateHighestScore(entity.PlayerID.Value, playerEntity.Score);

                    // 게임 횟수와 플레이 시간 업데이트
                    var session = GameSessionRepository.GetSessionById(currentSessionId);
                    if (session != null)
                    {
                        PlayerRepository.UpdateGameStats(entity.PlayerID.Value, session.PlayTimeSeconds);
                    }
                }
            }

            // 게임 세션 종료
            bool sessionEnded = GameSessionRepository.EndSession(currentSessionId, sessionEntities.Count);

            // 모든 엔티티를 실시간 랭킹에서 제거
            foreach (var entity in sessionEntities)
            {
                RankingManager.RemoveFromLiveRanking(currentSessionId, entity.EntityID);
            }

            Debug.Log($"게임 세션 종료: SessionID={currentSessionId}, 참여 엔티티 수={sessionEntities.Count}");

            // 세션 정보 초기화
            currentSessionId = -1;
            currentPlayerEntityId = -1;

            return sessionEnded;
        }
        catch (Exception ex)
        {
            Debug.LogError($"게임 세션 종료 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 현재 활성 세션 정보 반환
    /// </summary>
    public static GameSessionModel GetCurrentSession()
    {
        if (currentSessionId == -1) return null;
        return GameSessionRepository.GetSessionById(currentSessionId);
    }

    /// <summary>
    /// 특정 세션 정보 반환
    /// </summary>
    public static GameSessionModel GetSession(int sessionId)
    {
        return GameSessionRepository.GetSessionById(sessionId);
    }

    /// <summary>
    /// 현재 세션의 모든 엔티티 조회
    /// </summary>
    public static List<SessionEntityModel> GetCurrentSessionEntities(bool aliveOnly = false)
    {
        if (currentSessionId == -1) return new List<SessionEntityModel>();
        return SessionEntityRepository.GetSessionEntities(currentSessionId, aliveOnly);
    }


    /// <summary>
    /// 현재 세션 ID 반환
    /// </summary>
    public static int GetCurrentSessionId()
    {
        return currentSessionId;
    }

    /// <summary>
    /// 현재 플레이어 엔티티 ID 반환
    /// </summary>
    public static int GetCurrentPlayerEntityId()
    {
        return currentPlayerEntityId;
    }

    /// <summary>
    /// 활성 세션이 있는지 확인
    /// </summary>
    public static bool HasActiveSession()
    {
        return currentSessionId != -1;
    }

    /// <summary>
    /// 강제 세션 종료 (게임 나가기 등)
    /// </summary>
    public static void ForceEndSession()
    {
        try
        {
            if (HasActiveSession())
            {
                EndSession();
                Debug.Log("게임이 강제 종료되었습니다.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"강제 세션 종료 오류: {ex.Message}");
        }
    }
}
