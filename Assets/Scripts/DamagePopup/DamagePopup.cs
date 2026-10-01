using TMPro;
using UnityEngine;
using DG.Tweening;
using System.Collections;

public class DamagePopup : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI damageText;

    private float duration;
    float timer;
    private DamagePopupManager damagePopupManager;

    private float floatDistance = 2f;
    private float fadeDuration = 0.5f;
    private Vector3 offset;

    public void Setup(Color color, float amount, Vector3 point, float duration, DamagePopupManager damagePopupManager)
    {
        damageText.color = color;
        damageText.text = amount.ToString();
        transform.position = point;
        offset = point;

        this.duration = duration;
        timer = 0;

        this.damagePopupManager = damagePopupManager;

        Animation();
    }


    private void Update()
    {
        timer += Time.deltaTime;

        if(timer > duration)
        {
            damagePopupManager.DeactiveSelf(this);
        }
    }

    private void Animation()
    {
        Sequence seq = DOTween.Sequence();

        // y축 이동
        seq.Join(transform.DOMoveY(transform.position.y + floatDistance, duration)
                          .SetEase(Ease.OutCubic));
        // x축 좌우 이동
        seq.Join(transform.DOMoveX(transform.position.x + 0.3f, 0.5f)
                          .SetLoops(4, LoopType.Yoyo)
                          .SetEase(Ease.InOutSine));
        // Fade
        seq.Join(damageText.DOFade(0, fadeDuration)
                           .SetDelay(duration - fadeDuration)
                           .SetEase(Ease.InOutQuad));
        // 시퀀스 Disable연결
        seq.SetLink(gameObject, LinkBehaviour.KillOnDisable);
    }
};
