using UnityEngine;

/// <summary>
/// 플레이어의 상호작용 및 액티브
/// </summary>
public class PlayerCombat : MonoBehaviour
{
    [Header("Attack Settings")]
    [SerializeField] private float leftAttackCooldown = 0.5f;  // (좌클릭) 재사용 대기시간
    [SerializeField] private float rightAttackCooldown = 1.2f; // (우클릭) 재사용 대기시간

    private float nextAttackTime; // 다음 공격이 가능한 시점 (쿨타임 계산용)

    private void Update()
    {
        // 매 프레임 마우스 입력을 감지
        HandleAttackInput();
    }

    /// <summary>
    /// 마우스 클릭 입력에 따른 공격 분기 및 쿨타임 체크 처리
    /// </summary>
    private void HandleAttackInput()
    {
        // 현재 시간이 다음 공격 가능 시간보다 작으면(쿨타임 중이면) 입력을 무시
        if (Time.time < nextAttackTime) return;

        if (Input.GetMouseButtonDown(0)) // 마우스 좌클릭: 약공격
        {
            LeftAttack();
        }
        else if (Input.GetMouseButtonDown(1)) // 마우스 우클릭: 강공격
        {
            RightAttack();
        }
    }


    private void LeftAttack()
    {
        // 다음 공격 가능 시점을 약공격 쿨타임만큼 뒤로 설정
        nextAttackTime = Time.time + leftAttackCooldown;
        
        Debug.Log("약한공격 (속도: 빠름 / 데미지: 약함)");
        
        // TODO: 애니메이션 트리거 연동 (예: animator.SetTrigger("LightAttack"))
    }


    private void RightAttack()
    {
        // 다음 공격 가능 시점을 강공격 쿨타임만큼 뒤로 설정
        nextAttackTime = Time.time + rightAttackCooldown;

        Debug.Log("강한공격 (속도: 느림 / 데미지: 강함)");

        // TODO: 애니메이션 트리거 연동 (예: animator.SetTrigger("HeavyAttack"))
    }
}
