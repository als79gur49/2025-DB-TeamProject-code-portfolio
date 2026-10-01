using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

/// <summary>
/// 엔티티 관련 데이터베이스 작업을 담당하는 클래스
/// Player와 AI Enemy 모두 관리
/// </summary>
public static class EntityRepository
{
    /// <summary>
    /// 새로운 플레이어 엔티티 생성
    /// </summary>
    public static EntityModel CreatePlayerEntity(int playerId, string playerName)
    {
        try
        {
            string query = @"
                INSERT INTO Entities (EntityName, EntityType, PlayerID, CreatedAt)
                VALUES (@entityName, 'Player', @playerId, datetime('now'))
            ";

            DatabaseManager.ExecuteNonQuery(query, 
                ("@entityName", playerName), 
                ("@playerId", playerId));

            // 생성된 엔티티 ID 조회
            var entityId = DatabaseManager.ExecuteScalar("SELECT last_insert_rowid()");

            if (entityId != null)
            {
                return GetEntityById((int)(long)entityId);
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"플레이어 엔티티 생성 오류: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 새로운 AI 엔티티 생성
    /// </summary>
    public static EntityModel CreateAIEntity(string aiName)
    {
        try
        {
            string query = @"
                INSERT INTO Entities (EntityName, EntityType, PlayerID, CreatedAt)
                VALUES (@entityName, 'AI', NULL, datetime('now'))
            ";

            DatabaseManager.ExecuteNonQuery(query, ("@entityName", aiName));

            // 생성된 엔티티 ID 조회
            var entityId = DatabaseManager.ExecuteScalar("SELECT last_insert_rowid()");

            if (entityId != null)
            {
                return GetEntityById((int)(long)entityId);
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"AI 엔티티 생성 오류: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// EntityID로 엔티티 조회
    /// </summary>
    public static EntityModel GetEntityById(int entityId)
    {
        try
        {
            string query = @"
                SELECT EntityID, EntityName, EntityType, PlayerID, CreatedAt
                FROM Entities
                WHERE EntityID = @entityId
            ";

            using (var reader = DatabaseManager.ExecuteReader(query, ("@entityId", entityId)))
            {
                if (reader.Read())
                {
                    return new EntityModel
                    {
                        EntityID = (int)(long)reader["EntityID"],
                        EntityName = reader["EntityName"].ToString(),
                        EntityType = reader["EntityType"].ToString(),
                        PlayerID = reader["PlayerID"] == DBNull.Value ? null : (int)(long)reader["PlayerID"],
                        CreatedAt = DatabaseManager.ConvertUtcToLocal(DateTime.Parse(reader["CreatedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal))
                    };
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"엔티티 조회 오류: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 엔티티 이름으로 조회
    /// </summary>
    public static EntityModel GetEntityByName(string entityName, string entityType = null)
    {
        try
        {
            string query = @"
                SELECT EntityID, EntityName, EntityType, PlayerID, CreatedAt
                FROM Entities
                WHERE EntityName = @entityName
            ";

            if (!string.IsNullOrEmpty(entityType))
            {
                query += " AND EntityType = @entityType";
            }

            query += " ORDER BY CreatedAt DESC LIMIT 1";

            var parameters = new List<(string, object)> { ("@entityName", entityName) };
            if (!string.IsNullOrEmpty(entityType))
            {
                parameters.Add(("@entityType", entityType));
            }

            using (var reader = DatabaseManager.ExecuteReader(query, parameters.ToArray()))
            {
                if (reader.Read())
                {
                    return new EntityModel
                    {
                        EntityID = (int)(long)reader["EntityID"],
                        EntityName = reader["EntityName"].ToString(),
                        EntityType = reader["EntityType"].ToString(),
                        PlayerID = reader["PlayerID"] == DBNull.Value ? null : (int)(long)reader["PlayerID"],
                        CreatedAt = DatabaseManager.ConvertUtcToLocal(DateTime.Parse(reader["CreatedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal))
                    };
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"엔티티 이름 조회 오류: {ex.Message}");
            return null;
        }
    }

}
