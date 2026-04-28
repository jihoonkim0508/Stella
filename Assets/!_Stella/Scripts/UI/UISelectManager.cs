using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISelectManager : MonoBehaviour
{
    [Header("Mode (현재 UI 전체)")]
    public CanvasGroup modeGroup;          // CanvasGroup 필수
    public RectTransform modeRoot;         // 이동 대상

    [Header("Theme (다음 UI)")]
    public List<CanvasGroup> themeItems;   // 순차 등장할 UI들

    [Header("설정")]
    public float moveY = -50f;
    public float modeDuration = 0.4f;

    public float itemFadeDuration = 0.25f;
    public float itemDelay = 0.1f;

    private bool isPlaying = false;

    public ThemeSwitch themeSwitch; // 추가


    public void OnClick()
    {
        if (!isPlaying)
            StartCoroutine(Transition());
    }

    IEnumerator Transition()
    {
        isPlaying = true;

        Vector2 startPos = modeRoot.anchoredPosition;
        Vector2 endPos = startPos + new Vector2(0, moveY);

        float t = 0f;

        while (t < modeDuration)
        {
            t += Time.deltaTime;
            float lerp = t / modeDuration;

            modeRoot.anchoredPosition = Vector2.Lerp(startPos, endPos, lerp);
            modeGroup.alpha = Mathf.Lerp(1f, 0f, lerp);

            yield return null;
        }

        modeRoot.anchoredPosition = endPos;
        modeGroup.alpha = 0f;
        modeGroup.gameObject.SetActive(false);

        // Theme 초기화
        foreach (var item in themeItems)
        {
            item.alpha = 0f;
            item.gameObject.SetActive(true);
        }

        // Theme 순차 등장
        foreach (var item in themeItems)
        {
            yield return StartCoroutine(FadeIn(item, itemFadeDuration));
            yield return new WaitForSeconds(itemDelay);
        }

        // ⭐ 여기서 연결
        if (themeSwitch != null)
            themeSwitch.StartSwitch();

        isPlaying = false;
    }

    IEnumerator FadeIn(CanvasGroup cg, float duration)
    {
        float t = 0f;

        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(0f, 1f, t / duration);
            yield return null;
        }

        cg.alpha = 1f;
    }
}