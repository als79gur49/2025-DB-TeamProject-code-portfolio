using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

/// <summary>
/// 실시간 + 전체 랭킹 관리 (각 엔티티 개별 점수 지원)
/// Player와 AI 모두 포함한 통합 랭킹 시스템
/// </summary>
public static class RankingManager
{
    /// <summary>
    /// 실시간 랭킹에 엔티티 추가
    /// </summary>
    public static bool AddToLiveRanking(int sessionId, int entityId, string entityName, string entityType)
    {
        try
        {
            string query = @"
                INSERT OR REPLACE INTO SessionRanking 
                (SessionID, EntityID, EntityName, EntityType, CurrentScore, CurrentLevel, IsActive, LastUpdated)
                VALUES (@sessionId, @entityId, @entityName, @entityType, 0, 1, TRUE, datetime('now'))
            ";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query,
                ("@sessionId", sessionId),
                ("@entityId", entityId),
                ("@entityName", entityName),
                ("@entityType", entityType));

            if (rowsAffected > 0)
            {
                Debug.Log($"<color=black>실시간 랭킹에 추가: ID({entityId}) {entityName} ({entityType})</color>");
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"실시간 랭킹 추가 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 실시간 랭킹 업데이트 (특정 엔티티)
    /// </summary>
    public static bool UpdateLiveRanking(int sessionId, int entityId, int currentScore, int currentLevel = -1)
    {
        try
        {
            string query = @"
                UPDATE SessionRanking SET
                    CurrentScore = CurrentScore + @currentScore,
                    CurrentLevel = CASE WHEN @currentLevel > 0 THEN @currentLevel ELSE CurrentLevel END,
                    LastUpdated = datetime('now')
                WHERE SessionID = @sessionId AND EntityID = @entityId AND IsActive = TRUE
            ";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query,
                ("@currentScore", currentScore),
                ("@currentLevel", currentLevel),
                ("@sessionId", sessionId),
                ("@entityId", entityId));

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"실시간 랭킹 업데이트 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 실시간 랭킹에서 엔티티 제거 (사망 시)
    /// </summary>
    public static bool RemoveFromLiveRanking(int sessionId, int entityId)
    {
        try
        {
            string query = @"
                UPDATE SessionRanking SET
                    IsActive = FALSE,
                    LastUpdated = datetime('now')
                WHERE SessionID = @sessionId AND EntityID = @entityId
            ";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query,
                ("@sessionId", sessionId),
                ("@entityId", entityId));

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"실시간 랭킹 제거 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 현재 진행중인 실시간 랭킹 조회 (모든 활성 세션 통합)
    /// </summary>
    public static List<RankingData> GetLiveRanking(int limit = 10)
    {
        var rankings = new List<RankingData>();

        try
        {
            string query = @"
                SELECT EntityID, EntityName, EntityType, CurrentScore, CurrentLevel
                FROM SessionRanking
                WHERE IsActive = TRUE
                ORDER BY CurrentScore DESC, LastUpdated
                LIMIT @limit
            ";

            using (var reader = DatabaseManager.ExecuteReader(query, ("@limit", limit)))
            {
                int rank = 1;
                while (reader.Read())
                {
                    string entityType = reader["EntityType"].ToString();

                    rankings.Add(new RankingData
                    {
                        EntityID = (int)(long)reader["EntityID"],
                        EntityName = reader["EntityName"].ToString(),
                        EntityType = entityType,
                        Score = (int)(long)reader["CurrentScore"],
                        Level = (int)(long)reader["CurrentLevel"],
                        Rank = rank++
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"실시간 랭킹 조회 오류: {ex.Message}");
        }

        return rankings;
    }

    /// <summary>
    /// 특정 세션의 실시간 랭킹 조회
    /// </summary>
    public static List<RankingData> GetActiveSessionLiveRanking(int limit = 10)
    {
        var rankings = new List<RankingData>();

        try
        {
            string query = @"
                SELECT EntityID, EntityName, EntityType, CurrentScore, CurrentLevel
                FROM SessionRanking
                WHERE SessionID = @sessionId AND IsActive = TRUE
                ORDER BY CurrentScore DESC, LastUpdated ASC
                LIMIT @limit
            ";

            using (var reader = DatabaseManager.ExecuteReader(query,
                ("@sessionId", GameSessionManager.GetCurrentSessionId()),
                ("@limit", limit)))
            {
                int rank = 1;
                while (reader.Read())
                {
                    string entityType = reader["EntityType"].ToString();

                    rankings.Add(new RankingData
                    {
                        EntityID = (int)(long)reader["EntityID"],
                        EntityName = reader["EntityName"].ToString(),
                        EntityType = entityType,
                        Score = (int)(long)reader["CurrentScore"],
                        Level = (int)(long)reader["CurrentLevel"],
                        Rank = rank++
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"세션 실시간 랭킹 조회 오류: {ex.Message}");
        }

        return rankings;
    }


    /// <summary>
    /// 특정 세션의 종료된 랭킹 조회
    /// </summary>
    public static List<RankingData> GetSessionEndRanking(int sessionId, int limit = 10)
    {
        var rankings = new List<RankingData>();

        try
        {
            string query = @"
                SELECT EntityID, EntityName, EntityType, CurrentScore, CurrentLevel
                FROM SessionRanking
                WHERE SessionID = @sessionId AND IsActive = FALSE
                ORDER BY CurrentScore DESC, LastUpdated ASC
                LIMIT @limit
            ";

            using (var reader = DatabaseManager.ExecuteReader(query,
                ("@sessionId", sessionId),
                ("@limit", limit)))
            {
                int rank = 1;
                while (reader.Read())
                {
                    string entityType = reader["EntityType"].ToString();

                    rankings.Add(new RankingData
                    {
                        EntityID = (int)(long)reader["EntityID"],
                        EntityName = reader["EntityName"].ToString(),
                        EntityType = entityType,
                        Score = (int)(long)reader["CurrentScore"],
                        Level = (int)(long)reader["CurrentLevel"],
                        Rank = rank++
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"세션 실시간 랭킹 조회 오류: {ex.Message}");
        }

        return rankings;
    }
    /// <summary>
    /// 전체 최고 기록 랭킹 조회 (완료된 게임의 모든 엔티티)
    /// </summary>
    public static List<RankingData> GetAllTimeRanking(int limit = 10)
    {
        var rankings = new List<RankingData>();

        try
        {
            string query = @"
                SELECT 
                    se.EntityID,
                    e.EntityName,
                    e.EntityType,
                    se.Score,
                    se.Level,
                    gs.PlayTimeSeconds,
                    gs.StartedAt,
                    gs.EndedAt,
                    ROW_NUMBER() OVER (ORDER BY se.Score DESC, gs.EndedAt ASC) as Rank
                FROM SessionEntities se
                JOIN Entities e ON se.EntityID = e.EntityID
                JOIN GameSessions gs ON se.SessionID = gs.SessionID
                WHERE gs.IsCompleted = TRUE
                ORDER BY se.Score DESC, gs.EndedAt ASC
                LIMIT @limit
            ";

            using (var reader = DatabaseManager.ExecuteReader(query, ("@limit", limit)))
            {
                while (reader.Read())
                {
                    string entityType = reader["EntityType"].ToString();

                    rankings.Add(new RankingData
                    {
                        EntityID = (int)(long)reader["EntityID"],
                        EntityName = reader["EntityName"].ToString(),
                        EntityType = entityType,
                        Score = (int)(long)reader["Score"],
                        Level = (int)(long)reader["Level"],
                        PlayTime = reader["PlayTimeSeconds"] != DBNull.Value ? (int)(long)reader["PlayTimeSeconds"] : 0,
                        StartedAt = DatabaseManager.ConvertUtcToLocal(DateTime.Parse(reader["StartedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal)),
                        EndedAt = DatabaseManager.ConvertUtcToLocal(DateTime.Parse(reader["EndedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal)),
                        Rank = (int)(long)reader["Rank"]
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"전체 랭킹 조회 오류: {ex.Message}");
        }

        return rankings;
    }

    /// <summary>
    /// 플레이어만의 최고 기록 랭킹 조회
    /// </summary>
    public static List<RankingData> GetPlayerBestRanking(int limit = 10)
    {
        var rankings = new List<RankingData>();

        try
        {
            string query = @"
            SELECT 
                se.EntityID,
                e.EntityName,
                e.EntityType,
                se.Score,
                se.Level,
                gs.PlayTimeSeconds,
                gs.StartedAt,
                gs.EndedAt,
                ROW_NUMBER() OVER (ORDER BY se.Score DESC, gs.EndedAt ASC) as Rank
            FROM SessionEntities se
            JOIN Entities e ON se.EntityID = e.EntityID
            JOIN GameSessions gs ON se.SessionID = gs.SessionID
            WHERE gs.IsCompleted = TRUE AND e.EntityType = 'Player'
            ORDER BY se.Score DESC, gs.EndedAt ASC
            LIMIT @limit
        ";

            using (var reader = DatabaseManager.ExecuteReader(query, ("@limit", limit)))
            {
                while (reader.Read())
                {
                    string entityType = reader["EntityType"].ToString();

                    rankings.Add(new RankingData
                    {
                        EntityID = (int)(long)reader["EntityID"],
                        EntityName = reader["EntityName"].ToString(),
                        EntityType = entityType,
                        Score = (int)(long)reader["Score"],
                        Level = (int)(long)reader["Level"],
                        PlayTime = reader["PlayTimeSeconds"] != DBNull.Value ? (int)(long)reader["PlayTimeSeconds"] : 0,
                        StartedAt = DatabaseManager.ConvertUtcToLocal(DateTime.Parse(reader["StartedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal)),
                        EndedAt = DatabaseManager.ConvertUtcToLocal(DateTime.Parse(reader["EndedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal)),
                        Rank = (int)(long)reader["Rank"]
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"플레이어 랭킹 조회 오류: {ex.Message}");
        }

        return rankings;
    }


    /// <summary>
    /// 플레이어의 모든 플레이타임 반환
    /// </summary>
    public static int GetPlayerTotalPlayTime()
    {
        var rankings = new List<RankingData>();

        try
        {
            string query = @"
                SELECT 
                    SUM(atr.PlayTimeSeconds)
                FROM AllTimeRanking atr
                WHERE atr.EntityType = 'Player'
            ";

            var result = DatabaseManager.ExecuteScalar(query);
            return result != null ? (int)(long)result : 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"플레이어의 모든 플레이타임 조회 오류: {ex.Message}");
        }

        return -1;
    }

    /// <summary>
    /// 특정 점수의 순위 계산 (모든 엔티티 포함)
    /// </summary>
    public static int GetRankByScore(int score)
    {
        try
        {
            string query = @"
                SELECT COUNT(*) + 1 as Rank
                FROM SessionEntities se
                JOIN GameSessions gs ON se.SessionID = gs.SessionID
                WHERE se.Score > @score AND gs.IsCompleted = TRUE
            ";

            var result = DatabaseManager.ExecuteScalar(query, ("@score", score));

            return result != null ? (int)(long)result : -1;
        }
        catch (Exception ex)
        {
            Debug.LogError($"점수별 순위 조회 오류: {ex.Message}");
            return -1;
        }
    }

    /// <summary>
    /// 활성 엔티티 수 조회 (현재 진행중인 게임)
    /// </summary>
    public static int GetActiveEntityCount()
    {
        try
        {
            string query = "SELECT COUNT(*) FROM SessionRanking WHERE IsActive = TRUE";

            var result = DatabaseManager.ExecuteScalar(query);

            return result != null ? (int)(long)result : 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"활성 엔티티 수 조회 오류: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// 전체 게임 수 조회
    /// </summary>
    public static int GetTotalGameCount()
    {
        try
        {
            string query = "SELECT COUNT(*) FROM GameSessions WHERE IsCompleted = TRUE";

            var result = DatabaseManager.ExecuteScalar(query);

            return result != null ? (int)(long)result : 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"전체 게임 수 조회 오류: {ex.Message}");
            return 0;
        }
    }

    /// <summary>
    /// 최초 게임 생성일 조회 (전체 시스템)
    /// </summary>
    public static DateTime? GetCreatedAt()
    {
        try
        {
            string query = @"
                SELECT MIN(StartedAt) as FirstGame
                FROM GameSessions
            ";

            var result = DatabaseManager.ExecuteScalar(query);

            if (result != null && result != DBNull.Value)
            {
                return DatabaseManager.ConvertUtcToLocal(DateTime.Parse(result.ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal));
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"최초 생성일 조회 오류: {ex.Message}");
            return null;
        }
    }
    /// <summary>
    /// 마지막 플레이 기록 조회 (전체 시스템)
    /// </summary>
    public static DateTime? GetLastPlayedAt()
    {
        try
        {
            string query = @"
                SELECT MAX(EndedAt) as LastPlay
                FROM GameSessions
            ";

            var result = DatabaseManager.ExecuteScalar(query);

            if (result != null && result != DBNull.Value)
            {
                return DatabaseManager.ConvertUtcToLocal(DateTime.Parse(result.ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal));
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"마지막 플레이 기록 조회 오류: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 모든 랭킹 데이터 제거
    /// </summary>
    public static bool ClearAllSessionRanking()
    {
        try
        {
            string query = "DELETE FROM SessionRanking";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query);

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"세션 랭킹 정리 오류: {ex.Message}");
            return false;
        }
    }
}
