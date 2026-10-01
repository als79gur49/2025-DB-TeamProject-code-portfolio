using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonController : MonoBehaviour
{
    [Header("패널 연결")]
    [SerializeField] 
    private GameObject settingsPanel;
    [SerializeField] 
    private CanvasGroup settingsCanvasGroup; // 페이드 효과용

    [Header("버튼 연결")]
    [SerializeField] 
    private Button openSettingsButton;
    [SerializeField] 
    private Button closeSettingsButton;

    [Header("애니메이션 설정")]
    [SerializeField] 
    private float animationDuration = 0.3f;
    [SerializeField] 
    private Ease easeType = Ease.OutBack;
    [SerializeField] 
    private AnimationType animationType = AnimationType.ScaleAndFade;

    private RectTransform panelRectTransform;
    private Vector3 originalScale;
    private bool isAnimating = false;

    public enum AnimationType
    {
        Scale,           // 크기 변화
        Fade,           // 페이드 인/아웃
        ScaleAndFade,   // 크기 + 페이드
        SlideFromTop,   // 위에서 슬라이드
        SlideFromBottom, // 아래서 슬라이드
        SlideFromLeft,  // 왼쪽에서 슬라이드
        SlideFromRight  // 오른쪽에서 슬라이드
    }

    private void Awake()
    {
        // 컴포넌트 설정
        panelRectTransform = settingsPanel.GetComponent<RectTransform>();
        originalScale = panelRectTransform.localScale;

        // CanvasGroup이 없으면 자동 추가
        if (settingsCanvasGroup == null)
        {
            settingsCanvasGroup = settingsPanel.GetComponent<CanvasGroup>();
            if (settingsCanvasGroup == null)
            {
                settingsCanvasGroup = settingsPanel.AddComponent<CanvasGroup>();
            }
        }

        // 버튼 이벤트 연결
        openSettingsButton.onClick.AddListener(OpenPanel);
        closeSettingsButton.onClick.AddListener(ClosePanel);

        // 초기 상태 설정
        settingsPanel.SetActive(false);
    }

    public void OpenPanel()
    {
        if (isAnimating || settingsPanel.activeInHierarchy)
        {
            return;
        }

        isAnimating = true;
        settingsPanel.SetActive(true);

        // 버튼 비활성화 (애니메이션 중 중복 클릭 방지)
        openSettingsButton.interactable = false;

        switch (animationType)
        {
            case AnimationType.Scale:
                OpenWithScale();
                break;
            case AnimationType.Fade:
                OpenWithFade();
                break;
            case AnimationType.ScaleAndFade:
                OpenWithScaleAndFade();
                break;
            case AnimationType.SlideFromTop:
                OpenWithSlide(Vector2.up);
                break;
            case AnimationType.SlideFromBottom:
                OpenWithSlide(Vector2.down);
                break;
            case AnimationType.SlideFromLeft:
                OpenWithSlide(Vector2.left);
                break;
            case AnimationType.SlideFromRight:
                OpenWithSlide(Vector2.right);
                break;
        }
    }

    public void ClosePanel()
    {
        if (isAnimating || !settingsPanel.activeInHierarchy)
        {
            return;
        }

        isAnimating = true;

        // 버튼 비활성화
        closeSettingsButton.interactable = false;

        switch (animationType)
        {
            case AnimationType.Scale:
                CloseWithScale();
                break;
            case AnimationType.Fade:
                CloseWithFade();
                break;
            case AnimationType.ScaleAndFade:
                CloseWithScaleAndFade();
                break;
            case AnimationType.SlideFromTop:
                CloseWithSlide(Vector2.up);
                break;
            case AnimationType.SlideFromBottom:
                CloseWithSlide(Vector2.down);
                break;
            case AnimationType.SlideFromLeft:
                CloseWithSlide(Vector2.left);
                break;
            case AnimationType.SlideFromRight:
                CloseWithSlide(Vector2.right);
                break;
        }
    }

    private void OpenWithScale()
    {
        panelRectTransform.localScale = Vector3.zero;

        panelRectTransform.DOScale(originalScale, animationDuration)
            .SetEase(easeType)
            .OnComplete(OnOpenComplete);
    }

    private void OpenWithFade()
    {
        settingsCanvasGroup.alpha = 0f;

        settingsCanvasGroup.DOFade(1f, animationDuration)
            .SetEase(easeType)
            .OnComplete(OnOpenComplete);
    }

    private void OpenWithScaleAndFade()
    {
        panelRectTransform.localScale = Vector3.zero;
        settingsCanvasGroup.alpha = 0f;

        var sequence = DOTween.Sequence();
        sequence.Append(panelRectTransform.DOScale(originalScale, animationDuration).SetEase(easeType));
        sequence.Join(settingsCanvasGroup.DOFade(1f, animationDuration).SetEase(Ease.OutQuart));
        sequence.OnComplete(OnOpenComplete);
    }

    private void OpenWithSlide(Vector2 direction)
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 startPosition = panelRectTransform.anchoredPosition + direction * screenSize;
        Vector2 targetPosition = panelRectTransform.anchoredPosition;

        panelRectTransform.anchoredPosition = startPosition;
        settingsCanvasGroup.alpha = 0f;

        var sequence = DOTween.Sequence();
        sequence.Append(panelRectTransform.DOAnchorPos(targetPosition, animationDuration).SetEase(easeType));
        sequence.Join(settingsCanvasGroup.DOFade(1f, animationDuration * 0.5f).SetEase(Ease.OutQuart));
        sequence.OnComplete(OnOpenComplete);
    }



    private void CloseWithScale()
    {
        panelRectTransform.DOScale(Vector3.zero, animationDuration)
            .SetEase(Ease.InBack)
            .OnComplete(OnCloseComplete);
    }

    private void CloseWithFade()
    {
        settingsCanvasGroup.DOFade(0f, animationDuration)
            .SetEase(Ease.InQuart)
            .OnComplete(OnCloseComplete);
    }

    private void CloseWithScaleAndFade()
    {
        var sequence = DOTween.Sequence();
        sequence.Append(panelRectTransform.DOScale(Vector3.zero, animationDuration).SetEase(Ease.InBack));
        sequence.Join(settingsCanvasGroup.DOFade(0f, animationDuration).SetEase(Ease.InQuart));
        sequence.OnComplete(OnCloseComplete);
    }

    private void CloseWithSlide(Vector2 direction)
    {
        Vector2 screenSize = new Vector2(Screen.width, Screen.height);
        Vector2 targetPosition = panelRectTransform.anchoredPosition + direction * screenSize;

        var sequence = DOTween.Sequence();
        sequence.Append(settingsCanvasGroup.DOFade(0f, animationDuration * 0.5f).SetEase(Ease.InQuart));
        sequence.Join(panelRectTransform.DOAnchorPos(targetPosition, animationDuration).SetEase(Ease.InBack));
        sequence.OnComplete(OnCloseComplete);
    }



    private void OnOpenComplete()
    {
        isAnimating = false;
        openSettingsButton.interactable = true;
        closeSettingsButton.interactable = true;

        // 접근성을 위한 포커스 설정
        closeSettingsButton.Select();
    }

    private void OnCloseComplete()
    {
        isAnimating = false;
        settingsPanel.SetActive(false);
        openSettingsButton.interactable = true;

        // 원래 상태로 복원
        panelRectTransform.localScale = originalScale;
        settingsCanvasGroup.alpha = 1f;

        // 포커스를 열기 버튼으로 복원
        openSettingsButton.Select();
    }



    /// <summary>
    /// 애니메이션 타입을 런타임에서 변경
    /// </summary>
    public void SetAnimationType(AnimationType newType)
    {
        animationType = newType;
    }

    /// <summary>
    /// 애니메이션 지속시간 변경
    /// </summary>
    public void SetAnimationDuration(float duration)
    {
        animationDuration = Mathf.Max(0.1f, duration);
    }

    /// <summary>
    /// 이징 타입 변경
    /// </summary>
    public void SetEaseType(Ease ease)
    {
        easeType = ease;
    }

    /// <summary>
    /// 즉시 패널 열기 (애니메이션 없음)
    /// </summary>
    public void OpenPanelInstant()
    {
        if (isAnimating) return;

        settingsPanel.SetActive(true);
        panelRectTransform.localScale = originalScale;
        settingsCanvasGroup.alpha = 1f;
        closeSettingsButton.Select();
    }

    /// <summary>
    /// 즉시 패널 닫기 (애니메이션 없음)
    /// </summary>
    public void ClosePanelInstant()
    {
        if (isAnimating) return;

        settingsPanel.SetActive(false);
        panelRectTransform.localScale = originalScale;
        settingsCanvasGroup.alpha = 1f;
        openSettingsButton.Select();
    }


    private void OnDestroy()
    {
        // DOTween 정리
        panelRectTransform?.DOKill();
        settingsCanvasGroup?.DOKill();
    }
}


/*
 using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    // 모든 참조를 컨트롤러에서 관리. 

    [Header("패널 연결")]
    [SerializeField] private GameObject settingsPanel;

    [Header("버튼 연결")]
    [SerializeField] private Button openSettingsButton;
    [SerializeField] private Button closeSettingsButton;

    private void Awake()
    {
        openSettingsButton.onClick.AddListener(OpenPanel);
        closeSettingsButton.onClick.AddListener(ClosePanel);
    }

    public void OpenPanel()
    {
        settingsPanel.SetActive(true);
    }

    public void ClosePanel()
    {
        settingsPanel.SetActive(false);
    }
}

 */