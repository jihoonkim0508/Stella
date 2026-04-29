using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThemeSwitch : MonoBehaviour
{
    [Header("비활성화될 Non-Clickable")]
    public List<GameObject> nonClickableLives;

    [Header("활성화될 Clickable")]
    public List<GameObject> clickableObjects;

    [Header("설정")]
    public float durationPerItem; // 한 개당 전환 시간
    public float delayBetween;    // 다음으로 넘어가는 간격

    private List<CanvasGroup> nonClickableGroups = new List<CanvasGroup>();
    private List<CanvasGroup> clickableGroups = new List<CanvasGroup>();

    void Start()
    {
        foreach (var obj in nonClickableLives)
        {
            CanvasGroup cg = obj.GetComponent<CanvasGroup>();
            if (cg == null) cg = obj.AddComponent<CanvasGroup>();
            cg.alpha = 1f;
            nonClickableGroups.Add(cg);
        }

        foreach (var obj in clickableObjects)
        {
            CanvasGroup cg = obj.GetComponent<CanvasGroup>();
            if (cg == null) cg = obj.AddComponent<CanvasGroup>();

            obj.SetActive(true);
            cg.alpha = 0f;

            clickableGroups.Add(cg);
        }
    }

    public void StartSwitch()
    {
        StartCoroutine(SwitchSequential());
    }

    IEnumerator SwitchSequential()
    {
        int count = Mathf.Min(nonClickableGroups.Count, clickableGroups.Count);

        for (int i = 0; i < count; i++)
        {
            yield return StartCoroutine(SwitchOne(
                nonClickableGroups[i],
                clickableGroups[i]
            ));

            yield return new WaitForSeconds(delayBetween);
        }
    }

    IEnumerator SwitchOne(CanvasGroup off, CanvasGroup on)
    {
        float t = 0f;

        while (t < durationPerItem)
        {
            t += Time.deltaTime;
            float lerp = t / durationPerItem;

            off.alpha = Mathf.Lerp(1f, 0f, lerp);
            on.alpha = Mathf.Lerp(0f, 1f, lerp);

            yield return null;
        }

        off.alpha = 0f;
        off.gameObject.SetActive(false);

        on.alpha = 1f;
    }
}