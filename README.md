# Arrow.io 클론 프로젝트: DB 연동 시스템

개발 기간: **2025-05-20 ~ 2025-06-15**. 프로젝트명과 기간은 사용자 확인 사항입니다.

Unity 게임플레이를 SQLite 기반 엔티티·세션·랭킹 기록과 연결한 팀 프로젝트의 **공개 코드 검토용 선별 사본**입니다. AI 상태와 전이, 플레이어·적 생성, 점수와 사망 이벤트, DB 조회, UI 게임 로직 연결을 읽을 수 있습니다.

원본: 비공개 [als79gur49/2025-DB-TeamProject](https://github.com/als79gur49/2025-DB-TeamProject), `Branch_03_Diet`의 고정 커밋 [`67ee6cdf472e2a3888d367b96d426ae3cf9443da`](https://github.com/als79gur49/2025-DB-TeamProject/commit/67ee6cdf472e2a3888d367b96d426ae3cf9443da). 모든 보존 파일은 이 커밋을 기준으로 합니다. 링크는 원본 접근 권한이 있어야 열립니다.

## 검토 범위와 실행 제한

원본 트리의 파일 14,099개 중 `Assets/Scripts` C# 84개와 `Assets/SO` C# 3개를 후보로 삼아, 승인된 8개를 제외한 **C# 79개**를 보존했습니다. `Assets/SO`의 3개는 ScriptableObject **클래스 정의**이며 실제 `.asset` 인스턴스는 포함하지 않습니다. 설정 3개는 `ReviewContext` 아래에 원래 경로를 유지했습니다.

총 **87개 파일**: 코드 79 + 설정 3 + `README.md`, `CONTRIBUTIONS.md`, `MANIFEST.csv`, `VALIDATION.json`, `.gitignore` 5개. 원본에서 보존하지 않은 파일은 14,017개입니다. 원본 Git 이력과 `.git`은 포함하지 않으며 로컬 Git 초기화는 수행하지 않았습니다. 공개 게시 대상은 [als79gur49/2025-DB-TeamProject-code-portfolio](https://github.com/als79gur49/2025-DB-TeamProject-code-portfolio)입니다. 사용자가 이 정확한 저장소 이름과 Public 게시를 승인했습니다. 원본 저장소는 비공개로 유지하며, 게시 사본은 원본 이력을 가져오지 않는 별도 코드 검토 자료입니다. 최종 원격 파일 목록과 blob 검증 결과는 게시 완료 보고에서 별도로 확인할 수 있습니다.

**이 사본은 실행·빌드할 수 있는 Unity 프로젝트가 아닙니다.** 씬, 프리팹, meta, 모델, UI 리소스, DLL 및 외부 패키지 구현을 제외했습니다. `Projectile.cs` 제외로 `Straight`, `ProjectileStorage`, `WeaponBase`, `Entity` 등의 타입 참조도 해결되지 않습니다. `ReviewContext` 설정은 버전과 의존성 이해를 위한 자료이며 실행 환경을 복구하지 않습니다. Unity 실행, DB 실행, 빌드, 자동 테스트는 수행하지 않았습니다. 바이트 일치 검증은 실행 동작의 검증을 의미하지 않습니다.

## 권장 읽기 순서

1. [기여 구분](CONTRIBUTIONS.md)과 [파일별 목적·해시](MANIFEST.csv)를 먼저 확인합니다.
2. `Assets/Scripts/SessionManager.cs` → `PlayerSpawner.cs`, `EnemySpawner.cs`, `EntitySpawner.cs`: 의존성 주입, 세션 시작·종료, 스폰 이벤트, UI·카메라 연결.
3. `Assets/Scripts/Entity`와 `FSM`: 공통 엔티티의 점수·피해·사망 이벤트, 플레이어 입력, 적 AI의 상태·전이 분리와 풀 반환.
4. `Assets/Scripts/DB/DatabaseManager.cs` → `Models` → 각 `Repository` → `GameSessionManager.cs`, `EntityGameManager.cs`: 테이블과 뷰, 데이터 모델, SQL 접근, Unity 인스턴스와 DB 엔티티 매핑.
5. `DB/RankingManager.cs` → `UI/UIController.cs`, `UI/GameOverUI.cs`, `UI/LiveRankingUI.cs`, `etc/TitleRanking.cs`, `etc/TitlePlayerInfo.cs`: 실시간·종료·완료 기록 조회와 화면 표시.
6. `Weapon`, `Assets/SO`, `Entity/Player/LevelupStorage.cs`: 무기·스킬 데이터와 레벨업 연결. 제외된 투사체 기반 클래스와 외부 구현이 필요한 구조입니다.

DB 코드는 `Players`, `Entities`, `GameSessions`, `SessionEntities`, `SessionRanking`과 `AllTimeRanking`, `PlayerBestRanking`, `LiveRanking`, `SessionFinalRanking` 뷰를 정의합니다. 실제 DB 파일이나 레코드는 포함하지 않았습니다. `GetPlayerBestRanking` 메서드의 쿼리는 동명의 DB 뷰를 그대로 조회하는 구현이 아니므로 둘을 구별해서 읽어야 합니다.

## 버전과 외부 의존성

Unity **2022.3.33f1**, 리비전 **b2c853adf198**는 `ReviewContext/ProjectSettings/ProjectVersion.txt`의 값입니다. 아래 패키지 버전은 보존된 manifest와 lock의 최상위 resolved 항목을 기준으로 합니다. lock 안의 다른 패키지가 요청하는 하위 버전과 구별했습니다.

| 패키지 | 버전 | 근거 |
|---|---|---|
| Universal Render Pipeline | 14.0.11 | manifest / lock |
| Cinemachine | 2.10.3 | manifest / lock |
| AI Navigation | 1.1.6 | manifest / lock |
| Input System | 1.7.0 | manifest / lock |
| TextMesh Pro | 3.0.6 | manifest / lock |
| Test Framework | 1.1.33 | manifest / lock |
| Timeline | 1.7.6 | manifest / lock |
| Visual Scripting | 1.9.4 | manifest / lock |
| Burst | 1.8.15 | lock |
| Mathematics | 1.2.6 | lock |

전체 직접 의존성 45개와 lock의 resolved 항목 53개는 보존 설정에서 확인할 수 있습니다. 일부 입력 코드는 패키지 존재와 별개로 기존 `UnityEngine.Input` API를 사용합니다.

제외된 DLL에 대한 별도 원본 조사 근거의 DOTween 내장 버전 문자열은 **1.2.765**, Pro는 **1.0.381**입니다. 둘의 PE FileVersion **1.0.0.0**과 구별합니다. Mono.Data.Sqlite는 AssemblyVersion **4.0.0.0**, FileVersion **1.0.61.0**이며 sqlite3 엔진 버전은 미확인입니다. 이 사본은 DLL을 포함하지 않고, DLL 버전을 자체 바이트 검증한 사본도 아닙니다. 게임 코드는 `DG.Tweening`, `Mono.Data.Sqlite`, Unity 패키지 API와 원본 리소스에 의존합니다.

## 제외 원칙

모든 미디어·폰트·SDF·DB 바이너리·DLL·모델·셰이더·씬·프리팹·meta·SO 인스턴스·외부 패키지 구현을 제외했습니다. `Assets/Plugins`, `Assets/Synty`, `Assets/GUI Assets`, `Assets/VFX_Klaus`, `Assets/StreamingAssets`에서 보존한 파일은 **0개**입니다. 종류별 수량과 전체 제외 확장자 집계는 아래 표와 `VALIDATION.json`에서 확인할 수 있습니다. 수량은 폴더가 아닌 원본 트리의 파일 수입니다.

| 제외 종류 | 파일 수 |
|---|---:|
| meta | 7172 |
| 씬 / 프리팹 | 12 / 1322 |
| 이미지 | 4007 |
| 폰트 TTF | 7 |
| 모델 FBX·OBJ | 939 |
| DLL / DB 바이너리 | 8 / 3 |
| 셰이더·그래프·include | 29 |
| 직렬화 asset | 56 |
| C# | 36 |
| 위 분류 외 나머지 | 426 |

직렬화 asset 56개에는 파일명 기준 SDF 10개와 SO 인스턴스 5개가 포함됩니다. C# 제외 36개는 후보 제외 8개와 후보 경로 밖 28개입니다. 이 세부 수량은 위 분류와 중복되므로 합산하지 않습니다.

| 전체 제외한 리소스 폴더 | 원본 파일 수 | 보존 수 |
|---|---:|---:|
| `Assets/Plugins/` | 358 | 0 |
| `Assets/Synty/` | 1379 | 0 |
| `Assets/GUI Assets/` | 8465 | 0 |
| `Assets/VFX_Klaus/` | 201 | 0 |
| `Assets/StreamingAssets/` | 6 | 0 |

폴더별 수량은 확장자 분류와 중복됩니다. 원본 Git 이력은 원본 트리 파일 수에 포함되지 않으며 따로 취득·보존하지 않았습니다.

승인된 C# 제외 목록(모두 `Assets/Scripts/` 기준):

| 상대 경로 | 제외 이유 |
|---|---|
| `Weapon/Projectile/Projectile.cs` | 외부 VFX 구현과 공통 구간이 확인되어 파일 전체 제외 |
| `FSM/InputData.cs` | 승인된 선별 제외 |
| `Utils/RandomSkin.cs` | 승인된 선별 제외 |
| `etc/InputField.cs` | 승인된 선별 제외 |
| `Weapon/Effect/MoreDamageEffect.cs` | 승인된 선별 제외 |
| `Weapon/Effect/PoisonEffect.cs` | 승인된 선별 제외 |
| `Weapon/Effect/ProjectileEffect.cs` | 승인된 선별 제외 |
| `Utils/CoroutineSingleton.cs` | 승인된 선별 제외 |

`Projectile.cs`의 발사·충돌 VFX 구간은 초기 커밋 [`62cd4d1`](https://github.com/als79gur49/2025-DB-TeamProject/commit/62cd4d1a0fa975b8fe5a30da4658b87180880541)의 `Assets/VFX_Klaus/Scripts/ProjectileMove.cs`와 공통 구현이 확인됩니다. 복제 경위나 권리 침해를 단정하지 않으며, 파일에 있는 게임 통합 코드 전체가 외부 원작이라는 뜻도 아닙니다. 다른 파일에 외부 차용이 전혀 없다고 보증하지 않습니다.

## 정적 검토에서 확인한 한계

| 위치 | 확인 사항 |
|---|---|
| `DB/RankingManager.cs`, `FSM/State/AttackState.cs`, `ScoreBlockSpawner.cs`, `UI/GameOverUI.cs`, `etc/TitleSceneManager.cs` | `UNITY_EDITOR` 조건 없이 UnityEditor namespace를 import하는 5개 파일. 플레이어 빌드 제약을 검토해야 함 |
| `DB/GameSessionManager.cs` → `GameSessionRepository.cs` | `EndSession()`에서 플레이 시간 통계를 누적한 뒤 세션 종료 시각·PlayTimeSeconds를 갱신하므로 누적 순서 문제 존재 |
| `PlayerSpawner.cs` → `Entity/Player.cs` | 스킬 초기화가 있는 `Player.Setup(..., SkillIconManager)` 오버로드 대신 기본 Setup을 호출함 |
| `etc/TitlePlayerInfo.cs` | `highestScore != null || highestData != null` 조건 뒤 양쪽을 참조하여 null 접근 가능 |
| `DB/RankingManager.cs` | `GetPlayerBestRanking`은 완료된 Player 기록을 점수순으로 나열하며 계정별 최고 1건을 보장하지 않음 |
| `DB/RankingManager.cs` | `GetPlayerTotalPlayTime`, `GetCreatedAt`, `GetLastPlayedAt`는 특정 계정이 아닌 전체 시스템 범위 조회 |

이 목록은 실행 테스트 결과나 결함 전수 목록이 아닙니다. 검토용 사본의 원본 코드는 수정하지 않았습니다.

## 바이트 보존과 검증 읽기

82개 원본 파일에 대해 Git blob SHA-1(`blob <바이트수>\0` + 본문), 원본 트리 크기, 로컬 SHA-256를 확인했습니다. 텍스트 전송 중 변환된 26개는 원래 CP949 바이트로 복원했습니다. 이 중 6개는 전송 문자가 Windows-1252로 해석된 형태여서 역변환이 필요했고, 최종 원본 해시와 크기의 일치로 채택했습니다. UTF-8 56개(설정 3개 포함)와 CP949 26개를 원래 바이트 그대로 보존했습니다. 줄바꿈과 BOM도 원본 바이트의 일부로 검증됩니다.

`Entity.cs`의 주석에 U+FFFD **4개**, `Player.cs`의 주석에 **67개**가 원본 blob 자체에 존재합니다. 원본 해시 일치로 확인한 기존 문자 손상을 보존한 것이며, 임의 복원하거나 손실이 없는 텍스트라고 표시하지 않습니다. 새 전송 손상을 해시 불일치 상태로 채택한 파일은 없습니다.

원본 `.gitattributes`는 `* text=auto`로 줄바꿈 정규화를 유발할 수 있어 **미채택**했습니다. LFS filter 선언은 없습니다. 새로운 `.gitignore`는 모든 파일을 기본 제외하고 87개 경로와 필요한 부모 폴더만 명시적으로 허용합니다. 이 파일이 향후 Git의 전역 `core.autocrlf`나 외부 filter 설정까지 통제하지는 않습니다.

`MANIFEST.csv`는 코드 79개와 설정 3개의 정확한 경로·목적·인코딩·크기·원본 blob SHA·SHA-256를 담습니다. `VALIDATION.json`은 최종 87개 전체 파일 목록과 검증 범위를 담으며, 자기 파일의 해시는 순환 참조 때문에 파일 안에 넣지 않습니다. 별도로 전달한 최종 `VALIDATION.json` SHA-256로 해당 파일을 확인할 수 있습니다.

## 팀 기여와 출처의 한계

팀 역할은 권민혁: 게임플레이·DB 연동·UI 게임 로직 연결, 김지호: DB 스키마·SQL·관리, 전민균: UI/UX로 사용자 확인됐습니다. 개인 구현, 팀 기반 코드, 후속 공동 수정의 구분과 원본 커밋 링크는 `CONTRIBUTIONS.md`에 기록했습니다.

팀 코드와 이름 공개 동의는 **사용자 확인 사항**이며 독립적인 법적 검증이 아닙니다. 이 선별 사본의 지정 저장소 Public 게시는 별도로 사용자 승인됐습니다. 다른 대상의 재배포 권한이나 외부 구현의 권리까지 확정한 것은 아닙니다. 새 OSS 라이선스를 부여하지 않았습니다. 선별, 해시, 커밋 diff 검토는 전체 코드의 독창성이나 외부 리소스의 재배포 권한을 확정하지 않습니다.
