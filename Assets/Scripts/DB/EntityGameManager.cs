using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;

/// <summary>
/// Entity.cs와 Player.cs 연동을 위한 게임 매니저
/// 각 엔티티(Player + AI) 개별 점수 기록 지원
/// </summary>
public static class EntityGameManager
{
    // 엔티티 ID 매핑 (게임 오브젝트 ↔ DB 엔티티 ID)
    private static Dictionary<int, int> gameObjectToEntityMap = new Dictionary<int, int>();
    private static Dictionary<int, int> entityToGameObjectMap = new Dictionary<int, int>();

    /// <summary>
    /// 플레이어 게임 시작 (Entity.cs/Player.cs에서 호출)
    /// </summary>
    public static GameSessionModel StartNewGame(string playerName)
    {
        try
        {
            if (string.IsNullOrEmpty(playerName))
            {
                Debug.LogError("플레이어 이름이 비어있습니다.");
                return null;
            }

            // 기존 세션이 있으면 종료
            if (GameSessionManager.HasActiveSession())
            {
                Debug.LogWarning("기존 활성 세션이 있습니다. 강제로 종료합니다.");
                GameSessionManager.ForceEndSession();
            }

            // 매핑 테이블 초기화
            gameObjectToEntityMap.Clear();
            entityToGameObjectMap.Clear();

            var session = GameSessionManager.StartSession(playerName);

            if (session != null)
            {
                Debug.Log($"새로운 게임 시작! 플레이어: {playerName}, 세션 ID: {session.SessionID}");
            }

            return session;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"새 게임 시작 오류: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// AI 엔티티를 게임에 추가 (EntitySpawner에서 호출)
    /// </summary>
    public static bool AddAIEntity(int gameObjectInstanceId, string aiName)
    {
        try
        {
            bool added = GameSessionManager.AddAIEntityToSession(aiName);

            if (added)
            {
                // 새로 추가된 AI 엔티티의 ID를 찾기 위해 엔티티 이름으로 조회
                var aiEntity = EntityRepository.GetEntityByName(aiName, "AI");
                if (aiEntity != null)
                {
                    // 매핑 테이블에 추가
                    gameObjectToEntityMap[gameObjectInstanceId] = aiEntity.EntityID;
                    entityToGameObjectMap[aiEntity.EntityID] = gameObjectInstanceId;

                    Debug.Log($"<color=green>AI 엔티티 추가: {aiName} (GameObject ID: {gameObjectInstanceId}, Entity ID: {aiEntity.EntityID})</color>");
                    return true;
                }
            }

            return false;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"AI 엔티티 추가 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 플레이어 GameObject와 Entity ID 매핑 등록
    /// </summary>
    public static bool RegisterPlayerEntity(int gameObjectInstanceId)
    {
        try
        {
            if (!GameSessionManager.HasActiveSession())
            {
                Debug.LogWarning("활성 세션이 없습니다.");
                return false;
            }

            int playerEntityId = GameSessionManager.GetCurrentPlayerEntityId();
            if (playerEntityId > 0)
            {
                gameObjectToEntityMap[gameObjectInstanceId] = playerEntityId;
                entityToGameObjectMap[playerEntityId] = gameObjectInstanceId;

                Debug.Log($"플레이어 Entity 매핑 등록: GameObject ID={gameObjectInstanceId}, Entity ID={playerEntityId}");
                return true;
            }

            return false;
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"플레이어 엔티티 매핑 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 엔티티 점수 추가 
    /// </summary>
    public static void OnEntityScoreAddbyName(string entityName, int newScore, int newLevel = -1)
    {
        try
        {
            if (!GameSessionManager.HasActiveSession())
            {
                return; // 활성 세션이 없으면 무시
            }

            var entity = EntityRepository.GetEntityByName(entityName);

            if (entity != null)
            {
                bool added = GameSessionManager.AddEntityScore(entity.EntityID, newScore, newLevel);

                if (!added)
                {
                    Debug.LogWarning($"엔티티 점수 추가가 실패했습니다. Entity ID: {entity.EntityID}");
                }
            }
            else
            {
                Debug.LogWarning($"엔티티 매핑을 찾을 수 없습니다. EntityName: {entityName}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"엔티티 점수 추가 오류: {ex.Message}");
        }
    }

    /// <summary>
    /// 엔티티 사망 처리 (Entity.cs에서 호출)
    /// </summary>
    public static void OnEntityDeath(int gameObjectInstanceId, string entityName)
    {
        try
        {
            if (!GameSessionManager.HasActiveSession())
            {
                Debug.LogWarning($"{entityName}이 사망했지만 활성 세션이 없습니다.");
                return;
            }

            if (gameObjectToEntityMap.TryGetValue(gameObjectInstanceId, out int entityId))
            {
                bool updated = GameSessionManager.OnEntityDeath(entityId);

                if (updated)
                {
                    Debug.Log($"<color=red>{entityName} 사망 처리 완료 (Entity ID: {entityId})</color>");
                }
                else
                {
                    Debug.LogError($"{entityName} 사망 처리 중 오류가 발생했습니다.");
                }

                // 매핑 제거
                gameObjectToEntityMap.Remove(gameObjectInstanceId);
                entityToGameObjectMap.Remove(entityId);
            }
            else
            {
                Debug.LogWarning($"{entityName}의 매핑을 찾을 수 없습니다. GameObject ID: {gameObjectInstanceId}");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"엔티티 사망 처리 오류: {ex.Message}");
        }
    }

    /// <summary>
    /// 플레이어 사망 처리 (Player.cs에서 호출)
    /// 사망 처리와 세션 종료를 순차적으로 실행
    /// </summary>
    public static void OnPlayerDeath()
    {
        try
        {
            if (!GameSessionManager.HasActiveSession())
            {
                Debug.LogWarning("플레이어가 사망했지만 활성 세션이 없습니다.");
                return;
            }

            // 분리된 함수를 통한 통합 처리 (사망 처리 + 세션 종료)
            bool result = GameSessionManager.OnPlayerDeath();

            if (result)
            {
                Debug.Log("플레이어 사망 및 게임 세션 종료가 완료되었습니다.");

                // 매핑 테이블 초기화
                gameObjectToEntityMap.Clear();
                entityToGameObjectMap.Clear();

                Debug.Log("게임 오브젝트 매핑이 초기화되었습니다.");
            }
            else
            {
                Debug.LogError("플레이어 사망 처리 중 오류가 발생했습니다.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"플레이어 사망 처리 오류: {ex.Message}");

            // 오류 발생 시에도 매핑 테이블 정리
            try
            {
                gameObjectToEntityMap.Clear();
                entityToGameObjectMap.Clear();
                Debug.Log("오류 발생으로 인한 매핑 테이블 정리 완료");
            }
            catch (System.Exception cleanupEx)
            {
                Debug.LogError($"매핑 테이블 정리 중 오류: {cleanupEx.Message}");
            }
        }
    }

    /// <summary>
    /// 게임 강제 종료 (게임 나가기 등에서 호출)
    /// </summary>
    public static void ForceEndGame()
    {
        try
        {
            if (GameSessionManager.HasActiveSession())
            {
                GameSessionManager.ForceEndSession();

                // 매핑 테이블 초기화
                gameObjectToEntityMap.Clear();
                entityToGameObjectMap.Clear();

                Debug.Log("게임이 강제 종료되었습니다.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"게임 강제 종료 오류: {ex.Message}");
        }
    }

    /// <summary>
    /// 현재 게임 상태 정보 반환 (각 엔티티 포함)
    /// </summary>
    public static GameStatusInfo GetCurrentGameStatus()
    {
        try
        {
            if (!GameSessionManager.HasActiveSession())
            {
                return new GameStatusInfo { IsActive = false };
            }

            var session = GameSessionManager.GetCurrentSession();
            var sessionEntities = GameSessionManager.GetCurrentSessionEntities(true); // 살아있는 엔티티만
            var liveRanking = RankingManager.GetActiveSessionLiveRanking(5);

            // 플레이어 엔티티 정보 찾기
            var playerEntity = sessionEntities.Find(e => e.EntityType == "Player");

            return new GameStatusInfo
            {
                IsActive = true,
                CurrentScore = playerEntity?.Score ?? 0,
                CurrentLevel = playerEntity?.Level ?? 1,
                EnemiesKilled = playerEntity?.EnemiesKilled ?? 0,
                TotalEntities = sessionEntities.Count,
                AliveEntities = sessionEntities.Count,
                PlayerEntities = sessionEntities.FindAll(e => e.EntityType == "Player").Count,
                AIEntities = sessionEntities.FindAll(e => e.EntityType == "AI").Count,
                SessionId = GameSessionManager.GetCurrentSessionId(),
                PlayerEntityId = GameSessionManager.GetCurrentPlayerEntityId(),
                TopEntities = liveRanking
            };
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"게임 상태 조회 오류: {ex.Message}");
            return new GameStatusInfo { IsActive = false };
        }
    }
}

/// <summary>
/// 현재 게임 상태 정보 (확장)
/// </summary>
[System.Serializable]
public class GameStatusInfo
{
    public bool IsActive;
    public int CurrentScore;
    public int CurrentLevel;
    public int EnemiesKilled;
    public int TotalEntities;
    public int AliveEntities;
    public int PlayerEntities;
    public int AIEntities;
    public int SessionId;
    public int PlayerEntityId;
    public List<RankingData> TopEntities;

    public GameStatusInfo()
    {
        TopEntities = new List<RankingData>();
    }
}
