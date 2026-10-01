using UnityEngine;
using DG.Tweening;

public class ScoreBlock : MonoBehaviour
{
    private int score = 10;

    private Transform targetTransform;

    private MemoryPool<ScoreBlock> memoryPool;

    private ScoreBlockSpawner spawner;
    private bool canRespawn;
    private Vector3 originPosition;

    // 흡수 애니메이션 설정들
    private float pullBackDistance = 0.5f; // 반대 방향으로 당겨지는 거리
    private float pullBackDuration = 0.5f; // 뒤로 당겨지는 시간
    private float absorbDuration = 0.4f; // 플레이어에게 흡수되는 시간
    private float rotationSpeed = 720f; // 회전 속도

    private bool isAbsorbing = false;

    private float fullDuration => pullBackDuration + absorbDuration;
    [SerializeField]
    private GameObject childMesh; // 실제 작아질 오브젝트

    public void Setup(int score, Color c, float size,
        MemoryPool<ScoreBlock> memoryPool,
        bool canRespawn,
            ScoreBlockSpawner spawner = null)
    {
        this.score = score;

        isAbsorbing = false;

        // 색 변경
        MeshRenderer renderer = childMesh.GetComponent<MeshRenderer>();
        MaterialPropertyBlock block = new MaterialPropertyBlock();

        renderer.GetPropertyBlock(block);
        block.SetColor("_BaseColor", c);
        renderer.SetPropertyBlock(block);

        if (childMesh != null)
        {
            childMesh.transform.localScale = Vector3.one * size;
        }

        this.memoryPool = memoryPool;

        this.canRespawn = canRespawn;
        this.spawner = spawner;
        this.originPosition = transform.position;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 플레이어만 
        if (other.gameObject.TryGetComponent<Player>(out Player player))
        {
            AbsorbScoreObject(other.gameObject, player);
        }
    }

    // Entity 죽여서 쏟아져 나오는 모션
    public void LaunchUpwards()
    {
        // 포물선 형태 이동 이후 둥둥 떠다니는 트윈
        Sequence launchSeq = DOTween.Sequence();

        // x, z축 무작위 방향 이동값
        float xOffset = 3f * Random.Range(-1f, 1f);
        float zOffset = 3f * Random.Range(-1f, 1f);
        float upHeight = 2f;
        float duration = 1.2f;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(xOffset, 0f, zOffset);
        // 각 Offset에 곱해진 값 바꾸면, 포물선의 각도 변경
        Vector3 midPos = startPos + new Vector3(xOffset * 0.3f, upHeight, zOffset * 0.3f);

        Vector3[] path = new Vector3[] { startPos, midPos, endPos };

        launchSeq.Append(transform.DOPath(path, duration, PathType.CatmullRom)
            .SetEase(Ease.OutSine))
            .AppendCallback(() =>
            {
                // 도착 후 둥둥 떠다니기 시작
                transform.DOMoveY(0.5f, 1f)
                    .SetRelative(true)
                    .SetEase(Ease.InOutQuad)
                    .SetLoops(-1, LoopType.Yoyo);
            });

        /*
        Sequence launchSeq = DOTween.Sequence();
        Tween yoyoTween = transform.DOMoveY(0.5f, 1f).SetRelative(true).SetEase(Ease.InOutQuad).SetLoops(-1, LoopType.Yoyo).Pause();

        // launchSeq.Append(transform.DOMoveY(upHeight, upDuration) //y축 이동
        //             .SetRelative(true)
        //             .SetEase(Ease.OutQuad))
        //         .Join(transform.DOMoveX(0.5f * Random.Range(-1f, 1), upDuration * 0.8f) // x, z축 이동
        //             .SetRelative(true)
        //             .SetEase(Ease.OutQuad))
        //         .Join(transform.DOMoveZ(0.5f * Random.Range(-1f, 1), upDuration * 0.8f)
        //             .SetRelative(true)
        //             .SetEase(Ease.OutQuad))
        //         .Insert(upDuration * 0.7f, yoyoTween); //이후 둥둥 떠다니는 애니메이션

        launchSeq.Append(transform.DOMoveY(upHeight, upDuration) //y축 이동
                    .SetRelative(true)
                    .SetEase(Ease.OutQuad))
                .Join(transform.DOMoveX(5f * Random.Range(-1f, 1), upDuration * 0.8f) // x, z축 이동
                    .SetRelative(true)
                    .SetEase(Ease.OutQuad))
                .Join(transform.DOMoveZ(5f * Random.Range(-1f, 1), upDuration * 0.8f)
                    .SetRelative(true)
                    .SetEase(Ease.OutQuad))
                .AppendCallback(() =>
                {
                    //yoyoTween.Goto(Random.Range(0, 1f), true); // 해당 위치로 바로 가서 실행해서, 순간이동하는 문제점
                    yoyoTween.Play();
                });
        */
    }

    public void YoYoMoving()
    {
        Tween yoyoTween = transform.DOMoveY(0.5f, 1f)
            .SetRelative(true)
            .SetEase(Ease.InOutQuad)
            .SetLoops(-1, LoopType.Yoyo)
            .SetLink(gameObject);
        yoyoTween.Goto(Random.Range(0, 1f), true);
    }

    public void AbsorbScoreObject(GameObject target, Player player)
    {
        if (isAbsorbing)
        {
            return;
        }

        isAbsorbing = true;

        Vector3 startPos = transform.position;

        // 목표 반대 방향으로 일정 거리 계산
        Vector3 directionToPlayer = (target.transform.position - transform.position).normalized;
        Vector3 pullBackPos = startPos - directionToPlayer * pullBackDistance;

        Sequence absorbSequence = DOTween.Sequence();

        // 반대 방향으로 당기기
        absorbSequence.Append(transform.DOMove(pullBackPos, pullBackDuration)
            .SetEase(Ease.OutQuad));
        // 회전 트윈
        absorbSequence.Join(childMesh.transform.DORotate(new Vector3(0, rotationSpeed, 0), fullDuration, RotateMode.FastBeyond360));
        // 스케일 
        absorbSequence.Join(childMesh.transform.DOScale(0, fullDuration)
            .SetEase(Ease.InElastic));

        // 실시간 목표 방향으로 다가가기
        float t = 0f;
        //                                시작값, 진행 중인 값 ,목표값, 걸리는 시간
        absorbSequence.Insert(pullBackDuration, DOTween.To(() => t, x => t = x, 1, absorbDuration)
            .SetEase(Ease.InQuad)
            .OnUpdate(() =>
            {
                if (target != null)
                {
                    // 실시간으로 플레이어 위치 추적
                    float progress = t;
                    transform.position = Vector3.Lerp(pullBackPos, target.transform.position, progress);
                }
            }))
            .OnComplete(() =>
            {
                AddScoreTo(player);
            });
    }

    private void AddScoreTo(Player player)
    {
        if (player == null)
        {
            return;
        }

        player.AddScore(score, player.Info.EntityName);
        Destroy(gameObject);
    }
}