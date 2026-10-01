# 개인 기여와 공동 기반

이 문서는 **원본 GitHub 커밋과 diff 검토 결과** 및 고정 커밋 `67ee6cdf472e2a3888d367b96d426ae3cf9443da`의 관련 최종 파일 내용을 바탕으로 기여를 구분합니다. GitHub 계정의 변경 기록은 전체 파일의 단독 저작권이나 기여 비율을 의미하지 않습니다.

## 팀 역할

| 팀원 | 사용자 확인 역할 |
|---|---|
| 권민혁 | 게임플레이, DB 연동, UI와 게임 로직 연결 |
| 김지호 | DB 스키마, SQL, DB 관리 |
| 전민균 | UI/UX |

이 역할과 팀 코드·이름 공개 동의는 사용자 확인 사항이며 독립 법적 검증이 아닙니다. `Runedia`, `miji1234` 계정과 실명 대응은 확정되지 않았으므로 임의 연결하지 않습니다. 아래 권민혁 기여 범위는 확인된 설명과 `als79gur49` 변경 이력을 대조한 범위입니다.

## 권민혁의 게임플레이·통합 변경

| 변경 | 원본 근거 | 최종 검토 대상과 범위 |
|---|---|---|
| AI 상태와 전이 분리 | [92b9ff0](https://github.com/als79gur49/2025-DB-TeamProject/commit/92b9ff0549f33408c0bdb673307254b9d441a2e7) | `FSM/FSM.cs`, `FSM/State`, `FSM/Transition`: 동작과 조건 검사 분리 |
| Entity와 생성 책임 분리 | [3e3ce26](https://github.com/als79gur49/2025-DB-TeamProject/commit/3e3ce2674ccd9a7d1fe3f4f435914290504b0d31) | `Entity/Entity.cs`, `Enemy.cs`, `Player.cs`, 각 Spawner: 공통 기능·적 행동·플레이어 입력·생성 분리 |
| 무기·투사체·SO 구조화 | [340a1d6](https://github.com/als79gur49/2025-DB-TeamProject/commit/340a1d65b7da711cf43dd2e5ac9490c013ef346a) | 무기 데이터와 발사/피해 연동 구조 개선. 외부 VFX 공통 구간 때문에 `Projectile.cs` 전체는 이 사본에서 제외 |
| 세션 이벤트와 UI·카메라 연동 | [bbfd93f](https://github.com/als79gur49/2025-DB-TeamProject/commit/bbfd93f54da416a43972bec0b13f40f89d12eb6f) | `SessionManager.cs`, Spawner, `UI/UIController.cs`: 생성·사망 이벤트, 세션 ID 전달과 UI 연결 |
| 풀링된 적의 DB 등록과 리스너 정리 | [529e687](https://github.com/als79gur49/2025-DB-TeamProject/commit/529e6870549a842fb039f8a88b3a3d70dce4fe5c) | `Entity/Enemy.cs`, `Entity.cs`: Start 등록을 Setup 등록으로 이동하고 풀 반환 전 사망 리스너 정리 |
| 적 사망의 DB 반영 시점 수정 | [1614d38](https://github.com/als79gur49/2025-DB-TeamProject/commit/1614d381a817c2385ff83c69aa461354468c705a) | `Enemy.ReturnToPoolAfterDelay`: 제거 대기 전에 DB 사망 처리 |

## 직접 추가하여 최종 코드에 남은 API 6개

| API | 도입 근거 | 최종 동작 및 공동 이력 |
|---|---|---|
| `RankingManager.GetSessionEndRanking(int sessionId, int limit)` | [0fd9308](https://github.com/als79gur49/2025-DB-TeamProject/commit/0fd93082dc25608ab355812b5d15a8c3caece02b) | 해당 세션의 비활성 `SessionRanking` 조회. UI에 sessionId 전달. 기본 limit는 10 |
| `GameSessionManager.GetSession(int sessionId)` | [b879463](https://github.com/als79gur49/2025-DB-TeamProject/commit/b879463fb40a25bdbd24ab8e3f829ff4acf8db85) | 기존 `GameSessionRepository.GetSessionById`를 상위 관리 계층에 노출. 저장소 조회 자체의 신규 구현은 아님 |
| `RankingManager.GetPlayerTotalPlayTime()` | [098cdb0](https://github.com/als79gur49/2025-DB-TeamProject/commit/098cdb044b13600618ac1d1fee68db684f34f3e7) | 전체 Player 완료 기록의 시간 합계. [f8ea12e](https://github.com/als79gur49/2025-DB-TeamProject/commit/f8ea12e3c95beb85fa2200658efe08c2078304d4)에서 원천을 `AllTimeRanking`으로 변경. 특정 계정의 누적 시간 아님 |
| `RankingManager.GetCreatedAt()` | [098cdb0](https://github.com/als79gur49/2025-DB-TeamProject/commit/098cdb044b13600618ac1d1fee68db684f34f3e7) | `GameSessions`의 최소 StartedAt. 계정 생성일 아님 |
| `RankingManager.GetLastPlayedAt()` | [098cdb0](https://github.com/als79gur49/2025-DB-TeamProject/commit/098cdb044b13600618ac1d1fee68db684f34f3e7) | `GameSessions`의 최대 EndedAt. 특정 계정의 최근 플레이 시각 아님 |
| `RankingManager.ClearAllSessionRanking()` | [b333d0a](https://github.com/als79gur49/2025-DB-TeamProject/commit/b333d0a7df96dab123b4ac94a73684ff06b5b75e) | 임시 `SessionRanking` 전체 DELETE. 팀원 삭제 이후 Runedia의 [89308ec](https://github.com/als79gur49/2025-DB-TeamProject/commit/89308ec92fcd0a32b138a534e179e529cc8d0103)에서 같은 본문을 복원한 공동 이력. 영구 기록 전체 삭제라는 뜻은 아님 |

`GetPlayerBestRanking(int limit)`은 신규 추가가 아니라 기존 조회 수정입니다. [3c10237](https://github.com/als79gur49/2025-DB-TeamProject/commit/3c1023752ca6293bc6f5114dd78755cc133861b9)에서 `SessionEntities` / `Entities` / `GameSessions` JOIN으로 완료된 Player 기록과 종료 시각을 매핑하도록 바꿨습니다. 최종 쿼리는 계정별 최고 1건을 보장하지 않습니다.

역사적 시도인 `GetPlayerLiveRanking`, `SessionEntityRepository`의 이름 기반 오버로드, `GetCurrentSessionEntities`의 추가 오버로드, `GetSessionPlayerEndRanking`은 최종 API 6개 목록에 포함하지 않았습니다.

## 공동 기반과 단독 구현으로 표시하지 않는 범위

DB Repository·Manager·model 기반은 Runedia가 [b01bbbd](https://github.com/als79gur49/2025-DB-TeamProject/commit/b01bbbddc85bab11850cd0e0911d02238a0dc8aa)와 [142a1eb](https://github.com/als79gur49/2025-DB-TeamProject/commit/142a1ebae56532962cfe2518abae2bfe45440261)에서 도입·개편했습니다. 최종 `Assets/Scripts/DB` 13개 파일을 권민혁 개인의 단독 저작으로 표시하지 않습니다. 개인의 DB 연결·조회 개선은 기존 팀 기반 위에 이루어진 변경입니다. 최종 점수와 세션 종료 흐름에는 팀원 후속 수정이 있어 전체 시스템을 단독 구현으로 표시하지 않습니다.

UI 브랜치에서 miji1234가 `UI_Form.unitypackage`를 추가한 [a23f590](https://github.com/als79gur49/2025-DB-TeamProject/commit/a23f590cd947fe34f111bead33092ff69455b6ae) 이력이 있습니다. UI 게임 로직 연결 기여를 UI 디자인 전체의 단독 제작으로 확대하지 않습니다. 해당 패키지는 이 사본에 포함하지 않습니다.

최종 114커밋 중 als79gur49 106 / Runedia 8이라는 조사 집계는 기여율·작업량·저작권 비율이 아닙니다. 이 문서의 판단은 원본 커밋과 diff, 관련 최종 구현에 한정하며 모든 브랜치와 외부 출처에 대한 전수 감사는 아닙니다.

## 외부 구간과 이용 범위

`Projectile.cs`의 VFX 공통 구간과 원본 `VFX_Klaus/Scripts/ProjectileMove.cs`의 관계는 README에 기록했습니다. 복제 경위·침해 여부를 단정하거나 게임 통합 코드까지 전부 외부 원작으로 분류하지 않습니다. 다른 외부 차용이 전혀 없다는 보증도 하지 않습니다.

코드 검토를 위해 선별한 사본이며, 사용자가 `als79gur49/2025-DB-TeamProject-code-portfolio`의 Public 게시를 별도로 승인했습니다. 새 OSS 라이선스를 부여하거나 외부 구현의 재배포 권한을 확정하지 않았습니다. 팀 동의에 대한 사용자 확인은 독립적인 법적 검증이 아닙니다.
