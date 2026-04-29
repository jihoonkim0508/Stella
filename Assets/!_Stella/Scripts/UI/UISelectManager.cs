using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISelectManager : MonoBehaviour
{
    [Header("Mode")]
    public CanvasGroup modeGroup;
    public RectTransform modeRoot;

    [Header("Theme (부모)")]
    public List<CanvasGroup> themeItems;

    [Header("NonClickable 내부 아이템")]
    public List<CanvasGroup> nonClickableItems; // 자식들 따로 넣기

    [Header("설정")]
    public float moveY = -50f;
    public float modeDuration = 0.4f;

    public float itemFadeDuration = 0.25f;
    public float itemDelay = 0.1f;

    private bool isPlaying = false;

    public ThemeSwitch themeSwitch;

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

        modeGroup.gameObject.SetActive(false);

        // 부모 초기화
        foreach (var item in themeItems)
        {
            item.alpha = 0f;
            item.gameObject.SetActive(false);
        }

        // 자식 초기화
        foreach (var item in nonClickableItems)
        {
            item.alpha = 0f;
            item.gameObject.SetActive(false);
        }

        // 부모 순차 등장
        foreach (var item in themeItems)
        {
            item.gameObject.SetActive(true);
            yield return StartCoroutine(FadeIn(item, itemFadeDuration));

            // NonClickable_Theme일 때만 자식 실행
            if (item.name.Contains("Non-Clickable"))
            {
                foreach (var child in nonClickableItems)
                {
                    child.gameObject.SetActive(true);
                    yield return StartCoroutine(FadeIn(child, itemFadeDuration));
                    yield return new WaitForSeconds(itemDelay);
                }
            }

            yield return new WaitForSeconds(itemDelay);
        }

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