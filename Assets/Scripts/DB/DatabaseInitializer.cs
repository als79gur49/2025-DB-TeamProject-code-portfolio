using UnityEngine;
using System.Collections;

/// <summary>
/// 게임 시작 시 데이터베이스를 초기화하는 MonoBehaviour
/// 각 엔티티 개별 점수 시스템 지원
/// </summary>
public class DatabaseInitializer : MonoBehaviour
{
    [Header("Database Settings")]
    [SerializeField] private bool initializeOnAwake = true;
    [SerializeField] private bool closeOnApplicationQuit = true;
    [SerializeField] private bool enableDebugLogs = true;
    [SerializeField] private bool runStartupTests = true;

    [Header("Game Settings")]
    [SerializeField] private bool autoCleanupExpiredSessions = true;
    [SerializeField] private int maxActiveSessions = 20;

    // Singleton 패턴으로 중복 실행 방지
    private static bool isInitialized = false;
    private static DatabaseInitializer instance;

    public static DatabaseInitializer Instance => instance;

    private void Awake()
    {
        // 이미 인스턴스가 있으면 중복 방지
        if (instance != null && instance != this)
        {
            if (enableDebugLogs)
                Debug.LogWarning($"DatabaseInitializer가 이미 존재합니다. 중복 인스턴스를 제거합니다.");

            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 시에도 유지

        if (enableDebugLogs)
            Debug.Log($"DatabaseInitializer 초기화됨 - GameObject: {gameObject.name}");

        if (initializeOnAwake && !isInitialized)
        {
            InitializeDatabase();
        }
    }

    private void Start()
    {
        // 인스턴스가 본인인 경우에만 실행
        if (instance == this && isInitialized)
        {
            if (runStartupTests)
            {
                StartCoroutine(RunStartupTestsCoroutine());
            }

            if (autoCleanupExpiredSessions)
            {
                CleanupExpiredSessions();
            }
        }
    }

    /// <summary>
    /// 데이터베이스 초기화 (각 엔티티 개별 점수 시스템)
    /// </summary>
    public void InitializeDatabase()
    {
        try
        {
            if (enableDebugLogs)
                Debug.Log("=== 게임 DB 시스템 초기화 시작 ===");

            // 핵심 DB 초기화
            DatabaseManager.Initialize();

            // 초기화 성공 플래그 설정
            isInitialized = true;

            if (enableDebugLogs)
            {
                Debug.Log("✅ 게임 DB 시스템 초기화 완료");
                Debug.Log("지원 기능: Player + AI 엔티티 개별 점수, 실시간 랭킹, UTC 시간 처리");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ 데이터베이스 초기화 실패: {ex.Message}");
            isInitialized = false;
        }
    }

    /// <summary>
    /// 시작 시 테스트 실행 (코루틴)
    /// </summary>
    private IEnumerator RunStartupTestsCoroutine()
    {
        yield return new WaitForSeconds(0.1f); // DB 초기화 완료 대기

        if (enableDebugLogs)
            Debug.Log("=== DB 시스템 상태 확인 시작 ===");

        TestDatabaseConnection();
        TestNewTableStructure();
        CheckDatabaseStatistics();

        if (enableDebugLogs)
            Debug.Log("=== DB 시스템 상태 확인 완료 ===");
    }

    /// <summary>
    /// 데이터베이스 연결 테스트
    /// </summary>
    private void TestDatabaseConnection()
    {
        try
        {
            // 플레이어 테이블 확인
            var playerResult = DatabaseManager.ExecuteScalar("SELECT COUNT(*) FROM Players");
            int playerCount = playerResult != null ? (int)(long)playerResult : 0;

            // 엔티티 테이블 확인 (새로운 구조)
            var entityResult = DatabaseManager.ExecuteScalar("SELECT COUNT(*) FROM Entities");
            int entityCount = entityResult != null ? (int)(long)entityResult : 0;

            // 세션 테이블 확인
            var sessionResult = DatabaseManager.ExecuteScalar("SELECT COUNT(*) FROM GameSessions");
            int sessionCount = sessionResult != null ? (int)(long)sessionResult : 0;

            if (enableDebugLogs)
            {
                Debug.Log($"✅ DB 연결 테스트 성공");
                Debug.Log($"📊 기록된 플레이어: {playerCount}명");
                Debug.Log($"🤖 등록된 엔티티: {entityCount}개");
                Debug.Log($"🎮 총 게임 세션: {sessionCount}개");
            }

            // 데이터베이스 버전 확인
            int dbVersion = DatabaseManager.GetCurrentDatabaseVersion();
            if (enableDebugLogs)
                Debug.Log($"📋 DB 버전: {dbVersion}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ DB 연결 테스트 실패: {ex.Message}");
        }
    }

    /// <summary>
    /// 새로운 테이블 구조 확인
    /// </summary>
    private void TestNewTableStructure()
    {
        try
        {
            // 새로운 테이블들이 제대로 생성되었는지 확인
            string[] requiredTables = {
                "Players", "Entities", "GameSessions",
                "SessionEntities", "SessionRanking"
            };

            int existingTables = 0;
            foreach (string tableName in requiredTables)
            {
                var result = DatabaseManager.ExecuteScalar($"SELECT name FROM sqlite_master WHERE type='table' AND name='{tableName}'");
                if (result != null)
                {
                    existingTables++;
                    if (enableDebugLogs)
                        Debug.Log($"✅ 테이블 확인: {tableName}");
                }
                else
                {
                    Debug.LogWarning($"⚠️ 테이블 누락: {tableName}");
                }
            }

            if (enableDebugLogs)
                Debug.Log($"📋 테이블 상태: {existingTables}/{requiredTables.Length}개 존재");

            // 뷰(View) 확인
            string[] requiredViews = { "AllTimeRanking", "PlayerBestRanking", "LiveRanking", "SessionFinalRanking" };
            int existingViews = 0;
            foreach (string viewName in requiredViews)
            {
                var result = DatabaseManager.ExecuteScalar($"SELECT name FROM sqlite_master WHERE type='view' AND name='{viewName}'");
                if (result != null)
                {
                    existingViews++;
                }
            }

            if (enableDebugLogs)
                Debug.Log($"📋 뷰 상태: {existingViews}/{requiredViews.Length}개 존재");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ 테이블 구조 확인 실패: {ex.Message}");
        }
    }

    /// <summary>
    /// 데이터베이스 통계 확인 (간단한 정보만)
    /// </summary>
    private void CheckDatabaseStatistics()
    {
        try
        {
            // 기본 카운트만 조회
            int totalPlayers = (int)(long)DatabaseManager.ExecuteScalar("SELECT COUNT(*) FROM Players");
            int totalEntities = (int)(long)DatabaseManager.ExecuteScalar("SELECT COUNT(*) FROM Entities");
            int totalSessions = (int)(long)DatabaseManager.ExecuteScalar("SELECT COUNT(*) FROM GameSessions");
            int activeEntities = RankingManager.GetActiveEntityCount();

            if (enableDebugLogs)
            {
                Debug.Log($"📊 === DB 통계 정보 ===");
                Debug.Log($"📊 기록된 플레이어: {totalPlayers}명");
                Debug.Log($"🤖 등록된 엔티티: {totalEntities}개");
                Debug.Log($"🎮 총 게임 세션: {totalSessions}개");
                Debug.Log($"🔥 현재 활성 엔티티: {activeEntities}개");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ DB 통계 확인 실패: {ex.Message}");
        }
    }

    /// <summary>
    /// 만료된 세션 정리
    /// </summary>
    private void CleanupExpiredSessions()
    {
        try
        {
            // 24시간 이상 된 미완료 세션들을 찾아서 정리
            string query = @"
                UPDATE GameSessions 
                SET IsCompleted = TRUE, EndedAt = datetime('now')
                WHERE IsCompleted = FALSE 
                AND datetime(StartedAt, '+1 day') < datetime('now')
            ";

            int cleanedSessions = DatabaseManager.ExecuteNonQuery(query);

            if (cleanedSessions > 0)
            {
                if (enableDebugLogs)
                    Debug.Log($"🧹 만료된 세션 정리: {cleanedSessions}개 세션 자동 완료 처리");

                // 만료된 세션의 엔티티들도 비활성화
                string cleanupRankingQuery = @"
                    UPDATE SessionRanking 
                    SET IsActive = FALSE 
                    WHERE SessionID IN (
                        SELECT SessionID FROM GameSessions 
                        WHERE IsCompleted = TRUE AND datetime(StartedAt, '+1 day') < datetime('now')
                    )
                ";
                DatabaseManager.ExecuteNonQuery(cleanupRankingQuery);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"❌ 만료된 세션 정리 실패: {ex.Message}");
        }
    }

    /// <summary>
    /// 수동으로 DB 재초기화
    /// </summary>
    [ContextMenu("데이터베이스 재초기화")]
    public void ReinitializeDatabase()
    {
        if (enableDebugLogs)
            Debug.Log("데이터베이스 수동 재초기화 시작...");

        isInitialized = false;
        DatabaseManager.Close();
        InitializeDatabase();

        if (runStartupTests)
        {
            StartCoroutine(RunStartupTestsCoroutine());
        }
    }

    /// <summary>
    /// 현재 게임 세션 강제 종료
    /// </summary>
    [ContextMenu("현재 게임 세션 강제 종료")]
    public void ForceEndCurrentSession()
    {
        if (GameSessionManager.HasActiveSession())
        {
            EntityGameManager.ForceEndGame();
            if (enableDebugLogs)
                Debug.Log("현재 게임 세션이 강제로 종료되었습니다.");
        }
        else
        {
            if (enableDebugLogs)
                Debug.Log("진행중인 게임 세션이 없습니다.");
        }
    }

    /// <summary>
    /// DB 상태 정보 출력
    /// </summary>
    [ContextMenu("DB 상태 정보 출력")]
    public void PrintDatabaseStatus()
    {
        if (!isInitialized)
        {
            Debug.LogWarning("데이터베이스가 초기화되지 않았습니다.");
            return;
        }

        Debug.Log("=== DB 상태 정보 출력 ===");
        TestDatabaseConnection();
        CheckDatabaseStatistics();

        // 현재 게임 세션 정보
        if (GameSessionManager.HasActiveSession())
        {
            var status = EntityGameManager.GetCurrentGameStatus();
            Debug.Log($"🎮 현재 게임 진행중:");
            Debug.Log($"   세션 ID: {status.SessionId}");
            Debug.Log($"   총 엔티티: {status.TotalEntities} (살아있음: {status.AliveEntities})");
            Debug.Log($"   플레이어 점수: {status.CurrentScore}, 레벨: {status.CurrentLevel}");
        }
        else
        {
            Debug.Log("🎮 현재 진행중인 게임 세션이 없습니다.");
        }
    }

    /// <summary>
    /// 애플리케이션 종료 시 처리
    /// </summary>
    private void OnApplicationQuit()
    {
        if (closeOnApplicationQuit)
        {
            // 진행중인 게임 세션 강제 종료
            if (GameSessionManager.HasActiveSession())
            {
                if (enableDebugLogs)
                    Debug.Log("애플리케이션 종료로 인한 게임 세션 자동 종료");
                EntityGameManager.ForceEndGame();
            }

            // DB 연결 종료
            DatabaseManager.Close();

            if (enableDebugLogs)
                Debug.Log("데이터베이스 연결이 안전하게 종료되었습니다.");
        }
    }

    /// <summary>
    /// 애플리케이션 일시정지 시 처리 (모바일)
    /// </summary>
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            // 앱이 백그라운드로 갈 때 현재 게임 저장
            if (GameSessionManager.HasActiveSession())
            {
                if (enableDebugLogs)
                    Debug.Log("앱 일시정지로 인한 게임 세션 자동 종료");
                //EntityGameManager.ForceEndGame();
            }

            if (closeOnApplicationQuit)
            {
                DatabaseManager.Close();
            }
        }
        else
        {
            // 앱이 다시 활성화될 때 DB 재연결
            if (!isInitialized)
            {
                InitializeDatabase();
            }
        }
    }

    /// <summary>
    /// 초기화 상태 확인
    /// </summary>
    public static bool IsInitialized => isInitialized;

    /// <summary>
    /// 디버그 로그 활성화/비활성화
    /// </summary>
    public void SetDebugLogs(bool enabled)
    {
        enableDebugLogs = enabled;
        if (enableDebugLogs)
            Debug.Log("DatabaseInitializer 디버그 로그가 활성화되었습니다.");
    }
}
