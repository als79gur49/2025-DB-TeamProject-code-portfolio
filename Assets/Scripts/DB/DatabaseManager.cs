using System;
using System.Data;
using UnityEngine;
using Mono.Data.Sqlite;
using System.IO;

/// <summary>
/// SQLite 데이터베이스 연결 및 기본 CRUD 작업 관리 (각 엔티티 개별 점수 지원)
/// UTC 시간으로 통일 처리
/// </summary>
public static class DatabaseManager
{
    private static SqliteConnection connection;
    private static string connectionString;
    private const int DATABASE_VERSION = 3;

    /// <summary>
    /// 데이터베이스 초기화 및 연결
    /// </summary>
    public static void Initialize()
    {
        try
        {
            string dbPath = Path.Combine(Application.persistentDataPath, "gamedata.db");
            connectionString = $"URI=file:{dbPath},version=3";

            Debug.Log($"데이터베이스 경로: {dbPath}");

            // 연결 생성 및 테스트
            GetConnection();

            // 테이블 생성
            CreateTables();

            // 뷰 생성
            CreateViews();

            Debug.Log("데이터베이스 초기화 성공");
        }
        catch (Exception ex)
        {
            Debug.LogError($"데이터베이스 초기화 실패: {ex.Message}");
            Debug.LogError($"스택 트레이스: {ex.StackTrace}");
            throw;
        }
    }

    /// <summary>
    /// 데이터베이스 연결 반환 (자동 재연결)
    /// </summary>
    private static SqliteConnection GetConnection()
    {
        try
        {
            if (connection == null || connection.State != ConnectionState.Open)
            {
                if (connection != null)
                {
                    connection.Close();
                    connection.Dispose();
                }

                connection = new SqliteConnection(connectionString);
                connection.Open();

                // 외래 키 제약조건 활성화
                ExecuteNonQuery("PRAGMA foreign_keys = ON");

                // 성능 최적화 설정 개선
                ExecuteNonQuery("PRAGMA journal_mode = WAL");
                ExecuteNonQuery("PRAGMA synchronous = NORMAL");
                ExecuteNonQuery("PRAGMA cache_size = 10000");  // 메모리 캐시 크기 증가
                ExecuteNonQuery("PRAGMA temp_store = memory"); // 임시 데이터를 메모리에 저장
                ExecuteNonQuery("PRAGMA mmap_size = 268435456"); // 메모리 맵 크기 설정 (256MB)
            }

            return connection;
        }
        catch (Exception ex)
        {
            Debug.LogError($"데이터베이스 연결 실패: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// 데이터베이스 연결 종료
    /// </summary>
    public static void Close()
    {
        try
        {
            if (connection != null)
            {
                if (connection.State == ConnectionState.Open)
                {
                    connection.Close();
                }
                connection.Dispose();
                connection = null;
                Debug.Log("데이터베이스 연결이 안전하게 종료되었습니다");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"데이터베이스 종료 오류: {ex.Message}");
        }
    }

    /// <summary>
    /// 각 엔티티 개별 점수 지원 테이블들을 생성
    /// </summary>
    private static void CreateTables()
    {
        // 플레이어 기본 정보 테이블 (UTC 시간 사용)
        ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS Players (
                PlayerID INTEGER PRIMARY KEY AUTOINCREMENT,
                PlayerName VARCHAR(100) NOT NULL,
                HighestScore INTEGER DEFAULT 0,
                TotalPlayTime INTEGER DEFAULT 0,
                TotalGames INTEGER DEFAULT 0,
                CreatedAt DATETIME DEFAULT (datetime('now')),
                LastPlayedAt DATETIME DEFAULT (datetime('now'))
            )
        ");

        // 모든 엔티티 정보 테이블 (Player + AI Enemy)
        ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS Entities (
                EntityID INTEGER PRIMARY KEY AUTOINCREMENT,
                EntityName VARCHAR(100) NOT NULL,
                EntityType VARCHAR(20) NOT NULL,
                PlayerID INTEGER NULL,
                CreatedAt DATETIME DEFAULT (datetime('now')),
                FOREIGN KEY (PlayerID) REFERENCES Players(PlayerID) ON DELETE CASCADE
            )
        ");

        // EntityType 체크 제약조건을 별도로 추가 (SQLite 호환성)
        try
        {
            ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_entities_type_check ON Entities(EntityType) WHERE EntityType IN ('Player', 'AI')");
        }
        catch
        {
            Debug.Log("EntityType 인덱스 생성 건너뜀 (구버전 SQLite)");
        }

        // 게임 세션 기본 정보 테이블 (UTC 시간 사용)
        ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS GameSessions (
                SessionID INTEGER PRIMARY KEY AUTOINCREMENT,
                TotalEntities INTEGER DEFAULT 0,
                PlayTimeSeconds INTEGER DEFAULT 0,
                StartedAt DATETIME DEFAULT (datetime('now')),
                EndedAt DATETIME NULL,
                IsCompleted BOOLEAN DEFAULT FALSE
            )
        ");

        // 세션 내 모든 엔티티 개별 점수 기록 테이블
        ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS SessionEntities (
                SessionEntityID INTEGER PRIMARY KEY AUTOINCREMENT,
                SessionID INTEGER NOT NULL,
                EntityID INTEGER NOT NULL,
                EntityName VARCHAR(100) NOT NULL,
                EntityType VARCHAR(20) NOT NULL,
                Score INTEGER NOT NULL DEFAULT 0,
                Level INTEGER NOT NULL DEFAULT 1,
                EnemiesKilled INTEGER DEFAULT 0,
                IsAlive BOOLEAN DEFAULT TRUE,
                FinalRank INTEGER NULL,
                JoinedAt DATETIME DEFAULT (datetime('now')),
                DiedAt DATETIME NULL,
                FOREIGN KEY (SessionID) REFERENCES GameSessions(SessionID) ON DELETE CASCADE,
                FOREIGN KEY (EntityID) REFERENCES Entities(EntityID) ON DELETE CASCADE,
                UNIQUE(SessionID, EntityID)
            )
        ");

        // 실시간 랭킹용 뷰 테이블 (진행중인 세션의 엔티티들)
        ExecuteNonQuery(@"
            CREATE TABLE IF NOT EXISTS SessionRanking (
                RankingID INTEGER PRIMARY KEY AUTOINCREMENT,
                SessionID INTEGER NOT NULL,
                EntityID INTEGER NOT NULL,
                EntityName VARCHAR(100) NOT NULL,
                EntityType VARCHAR(20) NOT NULL,
                CurrentScore INTEGER NOT NULL DEFAULT 0,
                CurrentLevel INTEGER NOT NULL DEFAULT 1,
                IsActive BOOLEAN DEFAULT TRUE,
                LastUpdated DATETIME DEFAULT (datetime('now')),
                FOREIGN KEY (SessionID) REFERENCES GameSessions(SessionID) ON DELETE CASCADE,
                FOREIGN KEY (EntityID) REFERENCES Entities(EntityID) ON DELETE CASCADE,
                UNIQUE(SessionID, EntityID)
            )
        ");

        // 인덱스 생성
        CreateIndexes();

        // 뷰 생성
        CreateViews();

        Debug.Log("각 엔티티 개별 점수 시스템 테이블 생성 완료");
    }

    /// <summary>
    /// 성능 최적화를 위한 인덱스 생성
    /// </summary>
    private static void CreateIndexes()
    {
        // Players 테이블 인덱스
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_players_score ON Players(HighestScore DESC)");
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_players_lastplayed ON Players(LastPlayedAt DESC)");
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_players_name ON Players(PlayerName)"); // 이름으로 검색 최적화

        // Entities 테이블 인덱스
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_entities_type ON Entities(EntityType)");
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_entities_player ON Entities(PlayerID)");
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_entities_name_type ON Entities(EntityName, EntityType)"); // 복합 인덱스

        // GameSessions 테이블 인덱스
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_sessions_completed ON GameSessions(IsCompleted)");
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_sessions_started ON GameSessions(StartedAt DESC)");
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_sessions_entities_time ON GameSessions(TotalEntities, PlayTimeSeconds)"); // 성능 분석용

        // SessionEntities 테이블 인덱스
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_sessionentities_session ON SessionEntities(SessionID)");
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_sessionentities_entity ON SessionEntities(EntityID)");
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_sessionentities_score ON SessionEntities(Score DESC)");
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_sessionentities_alive ON SessionEntities(IsAlive)");
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_sessionentities_rank ON SessionEntities(SessionID, FinalRank)"); // 순위 조회 최적화

        // SessionRanking 테이블 인덱스
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_sessionranking_score ON SessionRanking(CurrentScore DESC)");
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_sessionranking_active ON SessionRanking(IsActive, CurrentScore DESC)");
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_sessionranking_session ON SessionRanking(SessionID)");
        ExecuteNonQuery("CREATE INDEX IF NOT EXISTS idx_sessionranking_updated ON SessionRanking(LastUpdated DESC)"); // 최근 업데이트 순 조회
    }

    /// <summary>
    /// 편의를 위한 뷰 생성
    /// </summary>
    private static void CreateViews()
    {
        // 전체 엔티티 랭킹 뷰 (완료된 게임의 모든 엔티티)
        ExecuteNonQuery(@"
            CREATE VIEW IF NOT EXISTS AllTimeRanking AS
            SELECT
                se.EntityID,
                se.EntityName,
                se.EntityType,
                se.Score,
                se.Level,
                gs.PlayTimeSeconds,
                gs.StartedAt,
                gs.EndedAt,
                ROW_NUMBER() OVER (ORDER BY se.Score DESC, gs.EndedAt ASC) as Rank
            FROM SessionEntities se
            JOIN GameSessions gs ON se.SessionID = gs.SessionID
            WHERE gs.IsCompleted = TRUE
            ORDER BY se.Score DESC
        ");

        // 플레이어별 최고 기록 랭킹 뷰
        ExecuteNonQuery(@"
            CREATE VIEW IF NOT EXISTS PlayerBestRanking AS
            SELECT
                p.PlayerID,
                p.PlayerName,
                p.HighestScore,
                p.TotalGames,
                p.TotalPlayTime,
                ROW_NUMBER() OVER (ORDER BY p.HighestScore DESC, p.LastPlayedAt ASC) as Rank
            FROM Players p
            WHERE p.HighestScore > 0
            ORDER BY p.HighestScore DESC
        ");

        // 실시간 랭킹 뷰 (진행중인 세션의 모든 엔티티)
        ExecuteNonQuery(@"
            CREATE VIEW IF NOT EXISTS LiveRanking AS
            SELECT
                sr.EntityID,
                sr.EntityName,
                sr.EntityType,
                sr.CurrentScore,
                sr.CurrentLevel,
                sr.IsActive,
                sr.LastUpdated,
                ROW_NUMBER() OVER (ORDER BY sr.CurrentScore DESC, sr.LastUpdated ASC) as Rank
            FROM SessionRanking sr
            WHERE sr.IsActive = TRUE
            ORDER BY sr.CurrentScore DESC
        ");

        // 세션별 엔티티 최종 순위 뷰
        ExecuteNonQuery(@"
            CREATE VIEW IF NOT EXISTS SessionFinalRanking AS
            SELECT
                se.SessionID,
                se.EntityID,
                se.EntityName,
                se.EntityType,
                se.Score,
                se.Level,
                se.EnemiesKilled,
                se.IsAlive,
                se.FinalRank,
                gs.EndedAt,
                ROW_NUMBER() OVER (PARTITION BY se.SessionID ORDER BY se.Score DESC) as CalculatedRank
            FROM SessionEntities se
            JOIN GameSessions gs ON se.SessionID = gs.SessionID
            WHERE gs.IsCompleted = TRUE
            ORDER BY se.SessionID DESC, se.Score DESC
        ");
    }


    /// <summary>
    /// SELECT 쿼리 실행 후 DataReader 반환
    /// </summary>
    public static IDataReader ExecuteReader(string query, params (string name, object value)[] parameters)
    {
        var cmd = GetConnection().CreateCommand();
        cmd.CommandText = query;

        // 파라미터 추가
        foreach (var param in parameters)
        {
            var dbParam = cmd.CreateParameter();
            dbParam.ParameterName = param.name;
            dbParam.Value = param.value ?? DBNull.Value;
            cmd.Parameters.Add(dbParam);
        }

        return cmd.ExecuteReader();
    }

    /// <summary>
    /// INSERT/UPDATE/DELETE 쿼리 실행
    /// </summary>
    public static int ExecuteNonQuery(string query, params (string name, object value)[] parameters)
    {
        using (var cmd = GetConnection().CreateCommand())
        {
            cmd.CommandText = query;

            // 파라미터 추가
            foreach (var param in parameters)
            {
                var dbParam = cmd.CreateParameter();
                dbParam.ParameterName = param.name;
                dbParam.Value = param.value ?? DBNull.Value;
                cmd.Parameters.Add(dbParam);
            }

            return cmd.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// 단일 값 반환 쿼리 실행 (COUNT, MAX 등)
    /// </summary>
    public static object ExecuteScalar(string query, params (string name, object value)[] parameters)
    {
        using (var cmd = GetConnection().CreateCommand())
        {
            cmd.CommandText = query;

            // 파라미터 추가
            foreach (var param in parameters)
            {
                var dbParam = cmd.CreateParameter();
                dbParam.ParameterName = param.name;
                dbParam.Value = param.value ?? DBNull.Value;
                cmd.Parameters.Add(dbParam);
            }

            return cmd.ExecuteScalar();
        }
    }

    /// <summary>
    /// UTC 시간을 로컬 시간으로 변환
    /// </summary>
    public static DateTime ConvertUtcToLocal(DateTime utcTime)
    {
        return utcTime.Kind == DateTimeKind.Utc ? utcTime.ToLocalTime() : utcTime;
    }

    /// <summary>
    /// 로컬 시간을 UTC 시간으로 변환
    /// </summary>
    public static DateTime ConvertLocalToUtc(DateTime localTime)
    {
        return localTime.Kind == DateTimeKind.Local ? localTime.ToUniversalTime() : localTime;
    }

    /// <summary>
    /// 데이터베이스 버전 조회
    /// </summary>
    public static int GetCurrentDatabaseVersion()
    {
        return DATABASE_VERSION;
    }

    /// <summary>
    /// 트랜잭션 시작
    /// </summary>
    public static SqliteTransaction BeginTransaction()
    {
        return GetConnection().BeginTransaction();
    }

    /// <summary>
    /// 데이터베이스 연결 상태 확인
    /// </summary>
    public static bool IsConnected()
    {
        return connection != null && connection.State == ConnectionState.Open;
    }

    /// <summary>
    /// 데이터베이스 파일 크기 조회 (바이트)
    /// </summary>
    public static long GetDatabaseSize()
    {
        try
        {
            string dbPath = Path.Combine(Application.persistentDataPath, "gamedata.db");
            if (File.Exists(dbPath))
            {
                return new FileInfo(dbPath).Length;
            }
            return 0;
        }
        catch
        {
            return 0;
        }
    }

    /// <summary>
    /// 데이터베이스 최적화 (VACUUM)
    /// </summary>
    public static void OptimizeDatabase()
    {
        try
        {
            ExecuteNonQuery("VACUUM");
            Debug.Log("데이터베이스 최적화 완료");
        }
        catch (Exception ex)
        {
            Debug.LogError($"데이터베이스 최적화 실패: {ex.Message}");
        }
    }
}
