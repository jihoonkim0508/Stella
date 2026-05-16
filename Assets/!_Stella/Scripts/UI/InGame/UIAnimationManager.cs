using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIAnimationManager : MonoBehaviour
{
    #region Player HP

    [Header("Player HP")]
    [SerializeField] private Image hpFillImage;
    [SerializeField] private float hpLerpSpeed = 5f;

    [Range(0, 1)]
    [SerializeField] private float targetPlayerHp = 1f;

    #endregion

    #region Boss HP

    [Header("Boss HP")]
    [SerializeField] private Image bossHpFillImage;
    [SerializeField] private float bossHpLerpSpeed = 4f;

    [Range(0, 1)]
    [SerializeField] private float targetBossHp = 1f;

    #endregion

    #region Narration

    [Header("Narration")]
    [SerializeField] private CanvasGroup narrationCanvasGroup;
    [SerializeField] private RectTransform narrationRect;
    [SerializeField] private TMP_Text narrationText;

    [SerializeField] private float narrationFadeDuration = 0.4f;
    [SerializeField] private float narrationStayDuration = 2f;
    [SerializeField] private float narrationMoveOffsetY = 20f;

    private Vector2 narrationDefaultPosition;

    #endregion

    #region Skill Cooldown

    [Header("Skill Cooldown")]
    [SerializeField] private Image[] skillCooldownImages;

    [SerializeField]
    private KeyCode[] skillKeys =
    {
        KeyCode.Q,
        KeyCode.E,
        KeyCode.Z,
        KeyCode.X
    };

    [SerializeField]
    private float[] skillCooldownTimes =
    {
        3f,
        5f,
        7f,
        10f
    };

    private bool[] isSkillOnCooldown;

    #endregion

    #region Unity Event

    private void Start()
    {
        InitializeNarration();
        InitializeSkillCooldown();
    }

    private void Update()
    {
        UpdatePlayerHpUI();
        UpdateBossHpUI();
        HandleSkillInput();
        HandleDebugInput();
    }

    #endregion

    #region Initialize

    private void InitializeNarration()
    {
        narrationDefaultPosition = narrationRect.anchoredPosition;
    }

    private void InitializeSkillCooldown()
    {
        isSkillOnCooldown = new bool[skillCooldownImages.Length];

        for (int i = 0; i < skillCooldownImages.Length; i++)
        {
            skillCooldownImages[i].fillAmount = 0f;
        }
    }

    #endregion

    #region Update UI

    private void UpdatePlayerHpUI()
    {
        hpFillImage.fillAmount = Mathf.Lerp(
            hpFillImage.fillAmount,
            targetPlayerHp,
            Time.deltaTime * hpLerpSpeed
        );
    }

    private void UpdateBossHpUI()
    {
        bossHpFillImage.fillAmount = Mathf.Lerp(
            bossHpFillImage.fillAmount,
            targetBossHp,
            Time.deltaTime * bossHpLerpSpeed
        );
    }

    #endregion

    #region 스킬쿨

    private void HandleSkillInput()
    {
        for (int i = 0; i < skillKeys.Length; i++)
        {
            if (Input.GetKeyDown(skillKeys[i]))
            {
                UseSkill(i);
            }
        }
    }

    public void UseSkill(int skillIndex)
    {
        if (isSkillOnCooldown[skillIndex])
            return;

        StartCoroutine(SkillCooldownRoutine(skillIndex));
    }

    private IEnumerator SkillCooldownRoutine(int skillIndex)
    {
        isSkillOnCooldown[skillIndex] = true;

        float elapsedTime = 0f;
        float cooldownDuration = skillCooldownTimes[skillIndex];

        skillCooldownImages[skillIndex].fillAmount = 1f;

        while (elapsedTime < cooldownDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = elapsedTime / cooldownDuration;

            skillCooldownImages[skillIndex].fillAmount =
                Mathf.Lerp(1f, 0f, progress);

            yield return null;
        }

        skillCooldownImages[skillIndex].fillAmount = 0f;

        isSkillOnCooldown[skillIndex] = false;
    }

    #endregion

    #region 나레이션

    public void ShowNarration(string message)
    {
        StopCoroutine(nameof(NarrationRoutine));
        StartCoroutine(NarrationRoutine(message));
    }

    private IEnumerator NarrationRoutine(string message)
    {
        narrationText.text = message;

        float elapsedTime = 0f;

        Vector2 startPosition =
            narrationDefaultPosition +
            new Vector2(0, narrationMoveOffsetY);

        Vector2 targetPosition = narrationDefaultPosition;

        narrationCanvasGroup.alpha = 0f;
        narrationRect.anchoredPosition = startPosition;

        while (elapsedTime < narrationFadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = elapsedTime / narrationFadeDuration;

            narrationCanvasGroup.alpha =
                Mathf.Lerp(0f, 1f, progress);

            narrationRect.anchoredPosition =
                Vector2.Lerp(startPosition, targetPosition, progress);

            yield return null;
        }

        narrationCanvasGroup.alpha = 1f;

        yield return new WaitForSeconds(narrationStayDuration);

        elapsedTime = 0f;

        while (elapsedTime < narrationFadeDuration)
        {
            elapsedTime += Time.deltaTime;

            float progress = elapsedTime / narrationFadeDuration;

            narrationCanvasGroup.alpha =
                Mathf.Lerp(1f, 0f, progress);

            yield return null;
        }

        narrationCanvasGroup.alpha = 0f;
    }

    #endregion

    #region 체력관리 UI

    public void DamagePlayer(float damageAmount)
    {
        targetPlayerHp -= damageAmount;
        targetPlayerHp = Mathf.Clamp01(targetPlayerHp);
    }

    public void HealPlayer(float healAmount)
    {
        targetPlayerHp += healAmount;
        targetPlayerHp = Mathf.Clamp01(targetPlayerHp);
    }

    public void DamageBoss(float damageAmount)
    {
        targetBossHp -= damageAmount;
        targetBossHp = Mathf.Clamp01(targetBossHp);
    }

    public void HealBoss(float healAmount)
    {
        targetBossHp += healAmount;
        targetBossHp = Mathf.Clamp01(targetBossHp);
    }

    #endregion

    #region 테스트

    private void HandleDebugInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            DamagePlayer(0.1f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            HealPlayer(0.1f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            DamageBoss(0.1f);
        }

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            HealBoss(0.1f);
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            ShowNarration("BOSS PHASE");
        }
    }

    #endregion
}