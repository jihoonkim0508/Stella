using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIButtonSelectEffect : MonoBehaviour
{
    public List<RectTransform> clickableButtons;
    public List<CanvasGroup> clickableGroups;

    public List<RectTransform> nonClickableRects;
    public List<CanvasGroup> nonClickableGroups;

    [Header("설정")]
    public float moveY = -150f;
    public float selectedMoveY = 10f;
    public float duration = 0.8f;
    public AnimationCurve easeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [Header("씬 이동")]
    public string nextSceneName;
    public float sceneLoadDelay = 0.3f;

    private bool isPlaying = false;

    public void OnClick(int index)
    {
        if (!isPlaying)
            StartCoroutine(Effect(index));
    }

    IEnumerator Effect(int selectedIndex)
    {
        isPlaying = true;

        float t = 0f;

        List<Vector2> startPosClickable = new List<Vector2>();
        List<Vector2> startPosNonClickable = new List<Vector2>();

        foreach (var r in clickableButtons)
            startPosClickable.Add(r.anchoredPosition);

        foreach (var r in nonClickableRects)
            startPosNonClickable.Add(r.anchoredPosition);

        while (t < duration)
        {
            t += Time.deltaTime;

            float lerp = t / duration;
            float curved = easeCurve.Evaluate(lerp);

            // 클릭 버튼
            for (int i = 0; i < clickableButtons.Count; i++)
            {
                if (i == selectedIndex)
                {
                    // 선택된 버튼 → 위로 이동
                    clickableButtons[i].anchoredPosition =
                        Vector2.Lerp(
                            startPosClickable[i],
                            startPosClickable[i] + new Vector2(0, selectedMoveY),
                            curved
                        );
                }
                else
                {
                    // 나머지 → 아래로 이동 + 사라짐
                    clickableButtons[i].anchoredPosition =
                        Vector2.Lerp(
                            startPosClickable[i],
                            startPosClickable[i] + new Vector2(0, moveY),
                            curved
                        );

                    clickableGroups[i].alpha =
                        Mathf.Lerp(1f, 0f, curved);
                }
            }

            // NonClickable
            for (int i = 0; i < nonClickableRects.Count; i++)
            {
                if (i == selectedIndex) continue;

                nonClickableRects[i].anchoredPosition =
                    Vector2.Lerp(
                        startPosNonClickable[i],
                        startPosNonClickable[i] + new Vector2(0, moveY),
                        curved
                    );

                nonClickableGroups[i].alpha =
                    Mathf.Lerp(1f, 0f, curved);
            }

            yield return null;
        }

        // 비활성화
        for (int i = 0; i < clickableButtons.Count; i++)
        {
            if (i == selectedIndex) continue;

            clickableGroups[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < nonClickableRects.Count; i++)
        {
            if (i == selectedIndex) continue;

            nonClickableGroups[i].gameObject.SetActive(false);
        }

        yield return new WaitForSeconds(sceneLoadDelay);

        // 씬 이동
        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }

        isPlaying = false;
    }
}