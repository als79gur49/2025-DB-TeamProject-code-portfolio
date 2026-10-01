using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

[System.Serializable]
public class Pair
{
    public string name;
    public Sprite sprite;
}


public class KillLogManager : MonoBehaviour
{
    [Header("UI 참조")]
    [SerializeField] 
    private KillLogPanel killLogPrefab;
    [SerializeField] 
    private Transform killLogParent;

    [Header("무기 스프라이트")]
    [SerializeField] 
    private List<Pair> weaponSprites;

    [Header("로그 설정")]
    [SerializeField] 
    private float removeLogTime = 5f;
    [SerializeField] 
    private int maxLogCount = 5;

    [Header("애니메이션 설정")]
    [SerializeField] 
    private float animationDuration = 0.5f;
    [SerializeField] 
    private Ease addEase = Ease.OutBack;
    [SerializeField] 
    private Ease removeEase = Ease.InBack;
    [SerializeField] 
    private bool useSlideAnimation = true;
    [SerializeField] 
    private Vector2 slideOffset = new Vector2(300f, 0f);

    // 현재 활성 로그들을 관리하는 큐
    private Queue<KillLogPanel> activeLogs = new Queue<KillLogPanel>();
    private RectTransform parentRect;

    void Start()
    {
        parentRect = killLogParent as RectTransform;

        if (parentRect == null)
        {
            Debug.LogError("killLogParent가 RectTransform이 아닙니다!");
        }
    }

    /// <summary>
    /// 킬 로그 추가 (애니메이션 포함)
    /// </summary>
    public void AddLog(KillLog log)
    {
        // 최대 로그 수 초과 시 가장 오래된 로그 제거
        if (activeLogs.Count >= maxLogCount)
        {
            RemoveOldestLogAnimated();
        }

        // 무기 스프라이트 찾기
        Pair resultPair = weaponSprites.FirstOrDefault();

        foreach (Pair pair in weaponSprites)
        {
            if (log.Weapon.Contains(pair.name))
            {
                resultPair = pair;
                break;
            }
        }

        // 로그 패널 생성
        KillLogPanel clone = Instantiate(killLogPrefab, killLogParent);
        clone.Setup(log.Enemy, resultPair.sprite, log.Self);

        // 큐에 추가
        activeLogs.Enqueue(clone);

        // 애니메이션과 함께 추가
        AddLogAnimated(clone);

        // 일정 시간 후 자동 제거
        StartCoroutine(RemoveLogAfterDelay(clone, removeLogTime));
    }

    /// <summary>
    /// 로그 추가 애니메이션
    /// </summary>
    private void AddLogAnimated(KillLogPanel logPanel)
    {
        RectTransform logRect = logPanel.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = logPanel.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = logPanel.gameObject.AddComponent<CanvasGroup>();
        }

        if (useSlideAnimation)
        {

            // 먼저 레이아웃 업데이트하여 정확한 위치 계산
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);

            // Layout이 적용된 후의 정확한 위치 저장
            Vector2 layoutPosition = logRect.anchoredPosition;

            // 시작 위치를 오프셋만큼 이동
            Vector2 startPosition = layoutPosition + slideOffset;
            logRect.anchoredPosition = startPosition;
            canvasGroup.alpha = 0f;

            // 정확한 Layout 위치로 애니메이션
            Sequence addSequence = DOTween.Sequence();
            addSequence.Append(logRect.DOAnchorPos(layoutPosition, animationDuration).SetEase(addEase))
                      .Join(canvasGroup.DOFade(1f, animationDuration * 0.7f))
                      .OnComplete(() => {
                      });
        }
        else
        {
            // 스케일 애니메이션은 위치 변경이 없으므로 문제없음
            logRect.localScale = Vector3.zero;
            canvasGroup.alpha = 0f;

            // 레이아웃 업데이트
            Canvas.ForceUpdateCanvases();
            LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);

            // 스케일 + 페이드 애니메이션
            Sequence addSequence = DOTween.Sequence();
            addSequence.Append(logRect.DOScale(Vector3.one, animationDuration).SetEase(addEase))
                      .Join(canvasGroup.DOFade(1f, animationDuration * 0.7f))
                      .OnComplete(() => {
                          Debug.Log($"킬 로그 추가 완료: {logPanel.name}");
                      });
        }
    }

    /// <summary>
    /// 로그 제거 애니메이션
    /// </summary>
    private void RemoveLogAnimated(KillLogPanel logPanel, System.Action onComplete = null)
    {
        if (logPanel == null)
        {
            onComplete?.Invoke();
            return;
        }

        RectTransform logRect = logPanel.GetComponent<RectTransform>();
        CanvasGroup canvasGroup = logPanel.GetComponent<CanvasGroup>();

        if (canvasGroup == null)
        {
            canvasGroup = logPanel.gameObject.AddComponent<CanvasGroup>();
        }

        if (useSlideAnimation)
        {
            // 슬라이드 아웃 애니메이션: 왼쪽으로 사라짐
            Vector2 exitPos = logRect.anchoredPosition - slideOffset;

            Sequence removeSequence = DOTween.Sequence();
            removeSequence.Append(logRect.DOAnchorPos(exitPos, animationDuration).SetEase(removeEase))
                         .Join(canvasGroup.DOFade(0f, animationDuration * 0.5f))
                         .OnComplete(() => {
                             if (logPanel != null)
                             {
                                 Destroy(logPanel.gameObject);
                             }

                             // 레이아웃 업데이트
                             if (parentRect != null)
                             {
                                 Canvas.ForceUpdateCanvases();
                                 LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
                             }

                             onComplete?.Invoke();
                         });
        }
        else
        {
            // 스케일 축소 애니메이션
            Sequence removeSequence = DOTween.Sequence();
            removeSequence.Append(logRect.DOScale(Vector3.zero, animationDuration).SetEase(removeEase))
                         .Join(canvasGroup.DOFade(0f, animationDuration * 0.5f))
                         .OnComplete(() => {
                             if (logPanel != null)
                             {
                                 Destroy(logPanel.gameObject);
                             }

                             // 레이아웃 업데이트
                             if (parentRect != null)
                             {
                                 Canvas.ForceUpdateCanvases();
                                 LayoutRebuilder.ForceRebuildLayoutImmediate(parentRect);
                             }

                             onComplete?.Invoke();
                             Debug.Log("킬 로그 제거 완료");
                         });
        }
    }

    /// <summary>
    /// 가장 오래된 로그 제거 (애니메이션 포함)
    /// </summary>
    private void RemoveOldestLogAnimated()
    {
        if (activeLogs.Count > 0)
        {
            KillLogPanel oldestLog = activeLogs.Dequeue();
            RemoveLogAnimated(oldestLog);
        }
    }

    /// <summary>
    /// 일정 시간 후 로그 제거
    /// </summary>
    private IEnumerator RemoveLogAfterDelay(KillLogPanel logPanel, float delay)
    {
        yield return new WaitForSeconds(delay);

        // 아직 큐에 있고 유효한 로그인지 확인
        if (logPanel != null && activeLogs.Contains(logPanel))
        {
            // 큐에서 제거
            var tempList = activeLogs.ToList();
            tempList.Remove(logPanel);
            activeLogs.Clear();
            foreach (var log in tempList)
            {
                if (log != null)
                {
                    activeLogs.Enqueue(log);
                }
            }

            // 애니메이션으로 제거
            RemoveLogAnimated(logPanel);
        }
    }
}

/*
 using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class Pair
{
    public string name;
    public Sprite sprite;
}

public class KillLogManager : MonoBehaviour
{
    [SerializeField]
    private KillLogPanel killLogPrefab;

    [SerializeField]
    private Transform killLogParent;

    // 무기 이름, 스프라이트 저장
    [SerializeField]
    private List<Pair> weaponSprites;

    private float removeLogTime = 5f;

    public void AddLog(KillLog log)
    {
        Pair resultPair = weaponSprites.FirstOrDefault();
     
        //log.Weapon에서 무기 이름을 추출하고 해당 스프라이트 반환
        foreach (Pair pair in weaponSprites)
        {
            if (log.Weapon.Contains(pair.name))
            {
                resultPair = pair;
                break;
            }
        }

        KillLogPanel clone = Instantiate(killLogPrefab);
        clone.Setup(log.Enemy, resultPair.sprite, log.Self);
        clone.transform.SetParent(killLogParent);
        Destroy(clone.gameObject, removeLogTime);
    }
}

 
 */