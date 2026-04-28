using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class ButtonMoveDown : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("내려갈 버튼")]
    public RectTransform targetButton;

    [Header("이동 설정")]
    public float moveY = -15f;
    public float moveDuration = 0.4f;

    [Header("자기 자신 이동")]
    public float selfMoveY = 10f;

    [Header("선택 표시 오브젝트")]
    public RectTransform selectObject;
    public float shakeRange = 10f;     // -10 ~ +10
    public float shakeSpeed = 5f;      // 흔들리는 속도

    private RectTransform selfRect;
    private Coroutine moveCoroutine;
    private Coroutine shakeCoroutine;

    private Vector2 originTargetPos;
    private Vector2 originSelfPos;
    private Vector2 selectOriginPos;

    void Start()
    {
        selfRect = GetComponent<RectTransform>();

        originTargetPos = targetButton.anchoredPosition;
        originSelfPos = selfRect.anchoredPosition;

        if (selectObject != null)
        {
            selectOriginPos = selectObject.anchoredPosition;
            selectObject.gameObject.SetActive(false);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        MoveTo(originTargetPos + new Vector2(0, moveY),
               originSelfPos + new Vector2(0, selfMoveY));

        if (selectObject != null)
        {
            selectObject.gameObject.SetActive(true);

            if (shakeCoroutine != null)
                StopCoroutine(shakeCoroutine);

            shakeCoroutine = StartCoroutine(ShakeX());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        MoveTo(originTargetPos, originSelfPos);

        if (selectObject != null)
        {
            if (shakeCoroutine != null)
                StopCoroutine(shakeCoroutine);

            selectObject.anchoredPosition = selectOriginPos;
            selectObject.gameObject.SetActive(false);
        }
    }

    void MoveTo(Vector2 targetPos, Vector2 selfPos)
    {
        if (moveCoroutine != null)
            StopCoroutine(moveCoroutine);

        moveCoroutine = StartCoroutine(MoveCoroutine(targetPos, selfPos));
    }

    IEnumerator MoveCoroutine(Vector2 targetPos, Vector2 selfPos)
    {
        Vector2 tStart = targetButton.anchoredPosition;
        Vector2 sStart = selfRect.anchoredPosition;

        float elapsed = 0f;

        while (elapsed < moveDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / moveDuration;

            targetButton.anchoredPosition = Vector2.Lerp(tStart, targetPos, t);
            selfRect.anchoredPosition = Vector2.Lerp(sStart, selfPos, t);

            yield return null;
        }

        targetButton.anchoredPosition = targetPos;
        selfRect.anchoredPosition = selfPos;
    }

    IEnumerator ShakeX()
    {
        float time = 0f;

        while (true)
        {
            time += Time.deltaTime * shakeSpeed;

            float offsetX = Mathf.Sin(time) * shakeRange;
            selectObject.anchoredPosition = selectOriginPos + new Vector2(offsetX, 0);

            yield return null;
        }
    }
}