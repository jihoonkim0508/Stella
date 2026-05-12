using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MenuIntroTransition : MonoBehaviour
{
    [Header("Fade Targets")]
    [SerializeField] private Image childImage;
    [SerializeField] private Image parentImage;

    [Header("Moving Button")]
    [SerializeField] private RectTransform button;

    [Header("Fade Timing")]
    [SerializeField] private float childFadeDuration = 1f;
    [SerializeField] private float parentFadeDuration = 1f;

    [Header("Button Motion")]
    [SerializeField] private float buttonMoveDistance = 100f;
    [SerializeField] private float buttonMoveDuration = 1f;
    [SerializeField] private float buttonDelay = 0f;

    private bool isPlaying;

    public void PlayTransition()
    {
        if (!isPlaying)
        {
            StartCoroutine(FadeSequence());
        }
    }

    public void StartFadeOut()
    {
        PlayTransition();
    }

    private IEnumerator FadeSequence()
    {
        isPlaying = true;

        if (button != null)
        {
            float moveDuration = Mathf.Max(0.01f, buttonMoveDuration + buttonDelay);
            StartCoroutine(MoveTop(button, buttonMoveDistance, moveDuration));
        }

        if (childImage != null)
        {
            yield return FadeImage(childImage, childFadeDuration);
        }

        if (parentImage != null)
        {
            yield return FadeImage(parentImage, parentFadeDuration);
        }

        isPlaying = false;
    }

    private IEnumerator FadeImage(Image target, float duration)
    {
        Color color = target.color;
        float startAlpha = color.a;
        float time = 0f;
        duration = Mathf.Max(0.01f, duration);

        while (time < duration)
        {
            time += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, 0f, time / duration);
            target.color = color;
            yield return null;
        }

        color.a = 0f;
        target.color = color;
        target.gameObject.SetActive(false);
    }

    private IEnumerator MoveTop(RectTransform target, float distance, float duration)
    {
        Vector2 startPos = target.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0f, distance);
        float time = 0f;
        duration = Mathf.Max(0.01f, duration);

        while (time < duration)
        {
            time += Time.deltaTime;
            target.anchoredPosition = Vector2.Lerp(startPos, endPos, time / duration);
            yield return null;
        }

        target.anchoredPosition = endPos;
    }
}
