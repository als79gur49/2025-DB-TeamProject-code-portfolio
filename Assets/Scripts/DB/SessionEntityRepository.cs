using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

/// <summary>
/// 세션 내 엔티티 관련 데이터베이스 작업을 담당하는 클래스
/// 각 엔티티의 개별 점수, 레벨, 처치 수 등을 관리
/// </summary>
public static class SessionEntityRepository
{
    /// <summary>
    /// 세션에 엔티티 추가
    /// </summary>
    public static SessionEntityModel AddEntityToSession(int sessionId, int entityId, string entityName, string entityType)
    {
        try
        {
            string query = @"
                INSERT INTO SessionEntities (SessionID, EntityID, EntityName, EntityType, JoinedAt)
                VALUES (@sessionId, @entityId, @entityName, @entityType, datetime('now'))
            ";

            DatabaseManager.ExecuteNonQuery(query,
                ("@sessionId", sessionId),
                ("@entityId", entityId),
                ("@entityName", entityName),
                ("@entityType", entityType));

            // 생성된 SessionEntity ID 조회
            var sessionEntityId = DatabaseManager.ExecuteScalar("SELECT last_insert_rowid()");

            if (sessionEntityId != null)
            {
                return GetSessionEntityById((int)(long)sessionEntityId);
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"세션 엔티티 추가 오류: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// SessionEntityID로 세션 엔티티 조회
    /// </summary>
    public static SessionEntityModel GetSessionEntityById(int sessionEntityId)
    {
        try
        {
            string query = @"
                SELECT SessionEntityID, SessionID, EntityID, EntityName, EntityType,
                       Score, Level, EnemiesKilled, IsAlive, FinalRank, JoinedAt, DiedAt
                FROM SessionEntities
                WHERE SessionEntityID = @sessionEntityId
            ";

            using (var reader = DatabaseManager.ExecuteReader(query, ("@sessionEntityId", sessionEntityId)))
            {
                if (reader.Read())
                {
                    return new SessionEntityModel
                    {
                        SessionEntityID = (int)(long)reader["SessionEntityID"],
                        SessionID = (int)(long)reader["SessionID"],
                        EntityID = (int)(long)reader["EntityID"],
                        EntityName = reader["EntityName"].ToString(),
                        EntityType = reader["EntityType"].ToString(),
                        Score = (int)(long)reader["Score"],
                        Level = (int)(long)reader["Level"],
                        EnemiesKilled = (int)(long)reader["EnemiesKilled"],
                        IsAlive = (bool)reader["IsAlive"],
                        FinalRank = reader["FinalRank"] == DBNull.Value ? null : (int)(long)reader["FinalRank"],
                        JoinedAt = DatabaseManager.ConvertUtcToLocal(DateTime.Parse(reader["JoinedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal)),
                        DiedAt = reader["DiedAt"] == DBNull.Value ? null : DatabaseManager.ConvertUtcToLocal(DateTime.Parse(reader["DiedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal))
                    };
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"세션 엔티티 조회 오류: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 세션과 엔티티 ID로 세션 엔티티 조회
    /// </summary>
    public static SessionEntityModel GetSessionEntity(int sessionId, int entityId)
    {
        try
        {
            string query = @"
                SELECT SessionEntityID, SessionID, EntityID, EntityName, EntityType,
                       Score, Level, EnemiesKilled, IsAlive, FinalRank, JoinedAt, DiedAt
                FROM SessionEntities
                WHERE SessionID = @sessionId AND EntityID = @entityId
            ";

            using (var reader = DatabaseManager.ExecuteReader(query,
                ("@sessionId", sessionId),
                ("@entityId", entityId)))
            {
                if (reader.Read())
                {
                    return new SessionEntityModel
                    {
                        SessionEntityID = (int)(long)reader["SessionEntityID"],
                        SessionID = (int)(long)reader["SessionID"],
                        EntityID = (int)(long)reader["EntityID"],
                        EntityName = reader["EntityName"].ToString(),
                        EntityType = reader["EntityType"].ToString(),
                        Score = (int)(long)reader["Score"],
                        Level = (int)(long)reader["Level"],
                        EnemiesKilled = (int)(long)reader["EnemiesKilled"],
                        IsAlive = (bool)reader["IsAlive"],
                        FinalRank = reader["FinalRank"] == DBNull.Value ? null : (int)(long)reader["FinalRank"],
                        JoinedAt = DatabaseManager.ConvertUtcToLocal(DateTime.Parse(reader["JoinedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal)),
                        DiedAt = reader["DiedAt"] == DBNull.Value ? null : DatabaseManager.ConvertUtcToLocal(DateTime.Parse(reader["DiedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal))
                    };
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"세션 엔티티 조회 오류: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 세션 엔티티 정보 업데이트
    /// </summary>
    public static bool UpdateSessionEntity(SessionEntityModel sessionEntity)
    {
        try
        {
            string query = @"
                UPDATE SessionEntities SET
                    Score = @score,
                    Level = @level,
                    EnemiesKilled = @enemiesKilled,
                    IsAlive = @isAlive,
                    FinalRank = @finalRank,
                    DiedAt = @diedAt
                WHERE SessionEntityID = @sessionEntityId
            ";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query,
                ("@score", sessionEntity.Score),
                ("@level", sessionEntity.Level),
                ("@enemiesKilled", sessionEntity.EnemiesKilled),
                ("@isAlive", sessionEntity.IsAlive),
                ("@finalRank", sessionEntity.FinalRank),
                ("@diedAt", sessionEntity.DiedAt.HasValue ?
                    DatabaseManager.ConvertLocalToUtc(sessionEntity.DiedAt.Value).ToString("yyyy-MM-dd HH:mm:ss") :
                    (object)DBNull.Value),
                ("@sessionEntityId", sessionEntity.SessionEntityID));

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"세션 엔티티 업데이트 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 엔티티 점수 추가
    /// </summary>
    public static bool AddEntityScore(int sessionId, int entityId, int newScore, int newLevel = -1)
    {
        try
        {
            var sessionEntity = GetSessionEntity(sessionId, entityId);
            if (sessionEntity == null) return false;

            sessionEntity.Score += newScore;
            if (newLevel > 0) sessionEntity.Level += newLevel;

            return UpdateSessionEntity(sessionEntity);
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
    public static bool UpdateEntityScore(int sessionId, int entityId, int newScore, int newLevel = -1)
    {
        try
        {
            var sessionEntity = GetSessionEntity(sessionId, entityId);
            if (sessionEntity == null) return false;

            sessionEntity.Score += newScore;
            if (newLevel > 0) sessionEntity.Level = newLevel;

            return UpdateSessionEntity(sessionEntity);
        }
        catch (Exception ex)
        {
            Debug.LogError($"엔티티 점수 업데이트 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 엔티티 처치 수 증가
    /// </summary>
    public static bool IncrementEntityKills(int sessionId, int entityId, int scoreGain = 100)
    {
        try
        {
            var sessionEntity = GetSessionEntity(sessionId, entityId);
            if (sessionEntity == null) return false;

            sessionEntity.Score += scoreGain;
            sessionEntity.EnemiesKilled++;

            return UpdateSessionEntity(sessionEntity);
        }
        catch (Exception ex)
        {
            Debug.LogError($"엔티티 처치 수 증가 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 엔티티 사망 처리
    /// </summary>
    public static bool SetEntityDead(int sessionId, int entityId, int finalRank)
    {
        try
        {
            var sessionEntity = GetSessionEntity(sessionId, entityId);
            if (sessionEntity == null) return false;

            sessionEntity.SetDead(finalRank);

            return UpdateSessionEntity(sessionEntity);
        }
        catch (Exception ex)
        {
            Debug.LogError($"엔티티 사망 처리 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 특정 세션의 모든 엔티티 조회
    /// </summary>
    public static List<SessionEntityModel> GetSessionEntities(int sessionId, bool aliveOnly = false)
    {
        var entities = new List<SessionEntityModel>();

        try
        {
            string query = @"
                SELECT SessionEntityID, SessionID, EntityID, EntityName, EntityType,
                       Score, Level, EnemiesKilled, IsAlive, FinalRank, JoinedAt, DiedAt
                FROM SessionEntities
                WHERE SessionID = @sessionId
            ";

            if (aliveOnly)
            {
                query += " AND IsAlive = TRUE";
            }

            query += " ORDER BY Score DESC";

            using (var reader = DatabaseManager.ExecuteReader(query, ("@sessionId", sessionId)))
            {
                while (reader.Read())
                {
                    entities.Add(new SessionEntityModel
                    {
                        SessionEntityID = (int)(long)reader["SessionEntityID"],
                        SessionID = (int)(long)reader["SessionID"],
                        EntityID = (int)(long)reader["EntityID"],
                        EntityName = reader["EntityName"].ToString(),
                        EntityType = reader["EntityType"].ToString(),
                        Score = (int)(long)reader["Score"],
                        Level = (int)(long)reader["Level"],
                        EnemiesKilled = (int)(long)reader["EnemiesKilled"],
                        IsAlive = (bool)reader["IsAlive"],
                        FinalRank = reader["FinalRank"] == DBNull.Value ? null : (int)(long)reader["FinalRank"],
                        JoinedAt = DatabaseManager.ConvertUtcToLocal(DateTime.Parse(reader["JoinedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal)),
                        DiedAt = reader["DiedAt"] == DBNull.Value ? null : DatabaseManager.ConvertUtcToLocal(DateTime.Parse(reader["DiedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal))
                    });
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"세션 엔티티 목록 조회 오류: {ex.Message}");
        }

        return entities;
    }

}
