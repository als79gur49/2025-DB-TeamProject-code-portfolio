using System;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

/// <summary>
/// 플레이어 관련 데이터베이스 작업을 담당하는 클래스
/// </summary>
public static class PlayerRepository
{
    /// <summary>
    /// 새 플레이어 생성 또는 기존 플레이어 조회
    /// </summary>
    public static PlayerModel CreatePlayer(string playerName)
    {
        try
        {
            // 새 플레이어 생성
            string query = @"
                INSERT INTO Players (PlayerName, CreatedAt, LastPlayedAt)
                VALUES (@playerName, datetime('now'), datetime('now'))
            ";

            DatabaseManager.ExecuteNonQuery(query, ("@playerName", playerName));

            // 생성된 플레이어 ID 조회
            var playerId = DatabaseManager.ExecuteScalar("SELECT last_insert_rowid()");

            if (playerId != null)
            {
                Debug.Log($"새 플레이어 생성: {playerName} (ID: {(int)(long)playerId})");
                return GetPlayerById((int)(long)playerId);
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"플레이어 생성 오류: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// 플레이어 이름으로 조회
    /// </summary>
    public static PlayerModel GetPlayerByName(string playerName)
    {
        try
        {
            string query = @"
                SELECT PlayerID, PlayerName, HighestScore, TotalPlayTime, TotalGames, CreatedAt, LastPlayedAt
                FROM Players
                WHERE PlayerName = @playerName
                ORDER BY LastPlayedAt DESC
                LIMIT 1
            ";

            using (var reader = DatabaseManager.ExecuteReader(query, ("@playerName", playerName)))
            {
                if (reader.Read())
                {
                    return new PlayerModel
                    {
                        PlayerID = (int)(long)reader["PlayerID"],
                        PlayerName = reader["PlayerName"].ToString(),
                        HighestScore = (int)(long)reader["HighestScore"],
                        TotalPlayTime = (int)(long)reader["TotalPlayTime"],
                        TotalGames = (int)(long)reader["TotalGames"],
                        CreatedAt = DatabaseManager.ConvertUtcToLocal(DateTime.Parse(reader["CreatedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal)),
                        LastPlayedAt = DatabaseManager.ConvertUtcToLocal(DateTime.Parse(reader["LastPlayedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal))
                    };
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"플레이어 이름 조회 오류: {ex.Message}");
            return null;
        }
    }

    /// <summary>
    /// PlayerID로 플레이어 조회
    /// </summary>
    public static PlayerModel GetPlayerById(int playerId)
    {
        try
        {
            string query = @"
                SELECT PlayerID, PlayerName, HighestScore, TotalPlayTime, TotalGames, CreatedAt, LastPlayedAt
                FROM Players
                WHERE PlayerID = @playerId
            ";

            using (var reader = DatabaseManager.ExecuteReader(query, ("@playerId", playerId)))
            {
                if (reader.Read())
                {
                    return new PlayerModel
                    {
                        PlayerID = (int)(long)reader["PlayerID"],
                        PlayerName = reader["PlayerName"].ToString(),
                        HighestScore = (int)(long)reader["HighestScore"],
                        TotalPlayTime = (int)(long)reader["TotalPlayTime"],
                        TotalGames = (int)(long)reader["TotalGames"],
                        CreatedAt = DatabaseManager.ConvertUtcToLocal(DateTime.Parse(reader["CreatedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal)),
                        LastPlayedAt = DatabaseManager.ConvertUtcToLocal(DateTime.Parse(reader["LastPlayedAt"].ToString(), null, System.Globalization.DateTimeStyles.AssumeUniversal))
                    };
                }
            }

            return null;
        }
        catch (Exception ex)
        {
            Debug.LogError($"플레이어 조회 오류: {ex.Message}");
            Debug.LogError($"스택 트레이스: {ex.StackTrace}");
            return null;
        }
    }

    /// <summary>
    /// 플레이어 정보 업데이트
    /// </summary>
    public static bool UpdatePlayer(PlayerModel player)
    {
        try
        {
            string query = @"
                UPDATE Players SET
                    HighestScore = @highestScore,
                    TotalPlayTime = @totalPlayTime,
                    TotalGames = @totalGames,
                    LastPlayedAt = datetime('now')
                WHERE PlayerID = @playerId
            ";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query,
                ("@highestScore", player.HighestScore),
                ("@totalPlayTime", player.TotalPlayTime),
                ("@totalGames", player.TotalGames),
                ("@playerId", player.PlayerID));

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"플레이어 업데이트 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 플레이어의 최고 점수 업데이트 (새 점수가 더 높을 때만)
    /// </summary>
    public static bool UpdateHighestScore(int playerId, int newScore)
    {
        try
        {
            string query = @"
                UPDATE Players SET
                    HighestScore = @newScore,
                    LastPlayedAt = datetime('now')
                WHERE PlayerID = @playerId AND HighestScore < @newScore
            ";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query,
                ("@newScore", newScore),
                ("@playerId", playerId));

            if (rowsAffected > 0)
            {
                Debug.Log($"플레이어 {playerId}의 최고 점수가 {newScore}로 업데이트되었습니다.");
            }

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"최고 점수 업데이트 오류: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// 플레이어의 게임 통계 업데이트 (게임 횟수 및 플레이 시간 증가)
    /// </summary>
    public static bool UpdateGameStats(int playerId, int playTimeSeconds)
    {
        try
        {
            string query = @"
                UPDATE Players SET
                    TotalGames = TotalGames + 1,
                    TotalPlayTime = TotalPlayTime + @playTimeSeconds,
                    LastPlayedAt = datetime('now')
                WHERE PlayerID = @playerId
            ";

            int rowsAffected = DatabaseManager.ExecuteNonQuery(query,
                ("@playTimeSeconds", playTimeSeconds),
                ("@playerId", playerId));

            return rowsAffected > 0;
        }
        catch (Exception ex)
        {
            Debug.LogError($"게임 통계 업데이트 오류: {ex.Message}");
            return false;
        }
    }
}
