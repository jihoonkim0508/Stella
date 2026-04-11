using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 플레이어의 스킬 입력(예: 대쉬 등)
/// </summary>
public class SkillInput : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image imgFill;         // 쿨타임 진행도를 보여줄 이미지 (1 -> 0으로 줄어듦)
    [SerializeField] private TMP_Text textCoolTime; // 화면에 표시될 남은 쿨타임 숫자 텍스트

    [Header("Settings")]
    [SerializeField] private float coolTime = 5f;   // 스킬 재사용 대기시간
    [SerializeField] private KeyCode skillKey = KeyCode.LeftShift; // 스킬 발동 키 (기본: 왼쪽 Shift)

    private bool isReady = true;  // 스킬 사용 가능 상태 여부
    private Coroutine coolTimeRoutine;
    private PlayerController player;

    private void Start()
    {
        // 씬 내의 PlayerController를 찾아 할당
        player = FindFirstObjectByType<PlayerController>();
        Init();
    }

    private void Update()
    {
        // 지정된 스킬 키가 눌렸을 때 실행 시도
        if (Input.GetKeyDown(skillKey))
        {
            UseSkill();
        }
    }

    /// <summary>
    /// 초기 UI 상태 설정
    /// </summary>
    public void Init()
    {
        if (textCoolTime != null) textCoolTime.gameObject.SetActive(false);
        if (imgFill != null) imgFill.fillAmount = 0;

        isReady = true;
    }

    /// <summary>
    /// 스킬 사용 조건(쿨타임, 움직임 여부)을 체크하고 스킬을 발동
    /// </summary>
    public void UseSkill()
    {
        // 1. 스킬 사용 가능(isReady) 상태이고
        // 2. 플레이어가 움직이고 있는 상태일 때만 발동 (대쉬 목적)
        if (isReady && player != null && player.IsMoving)
        {
            if (coolTimeRoutine != null) StopCoroutine(coolTimeRoutine);
            coolTimeRoutine = StartCoroutine(CoolTimeRoutine());

            // 플레이어의 대쉬 기능 실행
            player.Dash();

            Debug.Log($"{skillKey} Skill Activated!");
        }
        else if (!isReady)
        {
            Debug.Log("스킬이 아직 쿨타임 중입니다.");
        }
    }

    /// <summary>
    /// 쿨타임 동안 UI를 업데이트하고 사용 가능 상태로 복구하는 코루틴
    /// </summary>
    private IEnumerator CoolTimeRoutine()
    {
        isReady = false;
        if (textCoolTime != null) textCoolTime.gameObject.SetActive(true);

        float time = coolTime;

        while (time > 0)
        {
            time -= Time.deltaTime;

            // 남은 시간 텍스트 업데이트 (소수점 첫째 자리까지 표시)
            if (textCoolTime != null)
                textCoolTime.text = Mathf.Max(0, time).ToString("F1");

            // 이미지 채우기 정도 업데이트 (남은 시간에 비례)
            if (imgFill != null)
                imgFill.fillAmount = (time / coolTime);

            yield return null;
        }

        // 쿨타임 종료 후 UI 복구
        if (imgFill != null) imgFill.fillAmount = 0;
        if (textCoolTime != null) textCoolTime.gameObject.SetActive(false);

        isReady = true;
        coolTimeRoutine = null;
    }
}
