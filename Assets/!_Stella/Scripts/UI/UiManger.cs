using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class UiManger : MonoBehaviour
{
    [Header("비활성화될 UI")]
    public Image childImage;
    public Image parentImage;

    [Header("이동할 버튼")]
    public RectTransform buttonA;
    public RectTransform buttonB;

    [Header("페이드 시간")]
    public float childFadeDuration;
    public float parentFadeDuration;

    [Header("버튼 이동 설정")]
    public float buttonMoveDistance = 100f;
    public float buttonMoveDuration;
    public float btn_up; //모드선택 버튼 올라오는 시간

    public void StartFadeOut()
    {
        StartCoroutine(FadeSequence());
    }

    IEnumerator FadeSequence()
    {
        // 버튼 이동 먼저 시작 (자식 Fade 시작과 동시에)
        float moveDuration = childFadeDuration + btn_up;

        StartCoroutine(MoveTop(buttonA, buttonMoveDistance, moveDuration));
        StartCoroutine(MoveTop(buttonB, buttonMoveDistance, moveDuration));

        // 자식 Fade
        yield return StartCoroutine(FadeImage(childImage, childFadeDuration));

        // 자식 Fade 끝난 뒤 부모 Fade
        yield return StartCoroutine(FadeImage(parentImage, parentFadeDuration));
    }

    IEnumerator FadeImage(Image target, float duration)
    {
        Color color = target.color;
        float startAlpha = color.a;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 0f, time / duration);
            target.color = color;
            yield return null;
        }

        color.a = 0f;
        target.color = color;

        // Fade 완료 후 비활성화
        target.gameObject.SetActive(false);
    }

    IEnumerator MoveTop(RectTransform target, float distance, float duration)
    {
        Vector2 startPos = target.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0, distance);

        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            target.anchoredPosition = Vector2.Lerp(startPos, endPos, time / duration);
            yield return null;
        }

        target.anchoredPosition = endPos;
    }
}