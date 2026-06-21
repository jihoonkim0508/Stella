using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

/// <summary>
/// CharacterController 기반 1인칭 이동, 공격, 사망 처리를 담당합니다.
/// </summary>
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Health))]
public class Player : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth = 100;
    public int gold;

    [SerializeField] private Camera playerCamera;
    [SerializeField] private float lookSensitivity = 0.1f;

    private CharacterController controller;
    private Health health;
    private StatBlock stats;
    private float verticalVelocity;
    private float cameraPitch;
    private int airJumpsUsed;
    private float nextDashTime;
    private float dashInvulnerableUntil;
    private float nextAttackTime;

    /// <summary>
    /// 플레이어 입력과 체력 상태를 초기화합니다.
    /// </summary>
    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        health = GetComponent<Health>();
        health.Died += OnDied;
    }

    /// <summary>
    /// 저장/성장 상태를 기반으로 스탯을 적용합니다.
    /// </summary>
    private void Start()
    {
        gameObject.tag = "Player";
        if (playerCamera == null)
        {
            playerCamera = GetComponentInChildren<Camera>();
        }

        SaveData saveData = GameSession.Instance != null ? GameSession.Instance.SaveData : SaveService.CreateDefault();
        stats = GrowthService.ApplyCommonBonuses(StatBlock.DefaultPlayer(), saveData.commonLevel);
        maxHealth = stats.maxHealth;
        currentHealth = stats.maxHealth;
        health.Configure(stats.maxHealth, stats.defense);

        if (saveData.settings != null)
        {
            lookSensitivity = saveData.settings.mouseSensitivity;
        }
    }

    /// <summary>
    /// 이동, 시점, 점프, 대쉬, 공격 입력을 처리합니다.
    /// </summary>
    private void Update()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        ReadLook();
        ReadMovement();
        ReadActions();

        health.IsInvulnerable = Time.time < dashInvulnerableUntil;
        currentHealth = health.CurrentHealth;
    }

    /// <summary>
    /// 마우스 델타로 1인칭 시점을 회전합니다.
    /// </summary>
    private void ReadLook()
    {
        if (Mouse.current == null || playerCamera == null)
        {
            return;
        }

        Vector2 look = Mouse.current.delta.ReadValue() * lookSensitivity;
        transform.Rotate(Vector3.up * look.x);
        cameraPitch = Mathf.Clamp(cameraPitch - look.y, -80f, 80f);
        playerCamera.transform.localRotation = Quaternion.Euler(cameraPitch, 0f, 0f);
    }

    /// <summary>
    /// WASD 이동과 중력, 점프를 CharacterController로 적용합니다.
    /// </summary>
    private void ReadMovement()
    {
        Keyboard keyboard = Keyboard.current;
        Vector2 moveInput = Vector2.zero;
        if (keyboard != null)
        {
            moveInput.x = (keyboard.dKey.isPressed ? 1f : 0f) - (keyboard.aKey.isPressed ? 1f : 0f);
            moveInput.y = (keyboard.wKey.isPressed ? 1f : 0f) - (keyboard.sKey.isPressed ? 1f : 0f);
        }

        if (controller.isGrounded)
        {
            airJumpsUsed = 0;
            verticalVelocity = -1f;
        }

        Vector3 move = transform.right * moveInput.x + transform.forward * moveInput.y;
        if (move.sqrMagnitude > 1f)
        {
            move.Normalize();
        }

        verticalVelocity += Physics.gravity.y * Time.deltaTime;
        move.y = verticalVelocity;
        controller.Move(move * stats.moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 점프, 대쉬, 공격, 일시정지 입력을 처리합니다.
    /// </summary>
    private void ReadActions()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard != null)
        {
            if (keyboard.spaceKey.wasPressedThisFrame)
            {
                TryJump();
            }

            if ((keyboard.leftShiftKey.wasPressedThisFrame || keyboard.rightShiftKey.wasPressedThisFrame) && Time.time >= nextDashTime)
            {
                Dash();
            }

            if (keyboard.escapeKey.wasPressedThisFrame)
            {
                PauseMenuController.TogglePause();
            }
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && Time.time >= nextAttackTime)
        {
            Attack();
        }
    }

    /// <summary>
    /// 성장 해금 상태에 따라 1회 공중 점프를 허용합니다.
    /// </summary>
    private void TryJump()
    {
        int commonLevel = GameSession.Instance != null ? GameSession.Instance.SaveData.commonLevel : 1;
        bool canAirJump = GrowthService.HasDoubleJump(commonLevel) && airJumpsUsed == 0;
        if (!controller.isGrounded && !canAirJump)
        {
            return;
        }

        if (!controller.isGrounded)
        {
            airJumpsUsed++;
        }

        verticalVelocity = stats.jumpForce;
    }

    /// <summary>
    /// 바라보는 방향으로 짧게 이동하고 해금 시 대쉬 무적을 적용합니다.
    /// </summary>
    private void Dash()
    {
        nextDashTime = Time.time + stats.dashCooldown;
        controller.Move(transform.forward * stats.dashDistance);

        int commonLevel = GameSession.Instance != null ? GameSession.Instance.SaveData.commonLevel : 1;
        if (GrowthService.HasDashInvulnerability(commonLevel))
        {
            dashInvulnerableUntil = Time.time + 0.35f;
        }
    }

    /// <summary>
    /// 카메라 정면의 피해 가능 대상에게 기본 공격 피해를 줍니다.
    /// </summary>
    private void Attack()
    {
        nextAttackTime = Time.time + AttackDefinition.Default().cooldown;
        Ray ray = playerCamera != null
            ? new Ray(playerCamera.transform.position, playerCamera.transform.forward)
            : new Ray(transform.position + Vector3.up, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, AttackDefinition.Default().range)
            && hit.collider.TryGetComponent(out IDamageable damageable))
        {
            damageable.TakeDamage(new DamageInfo(gameObject, stats.attackPower));
        }
    }

    /// <summary>
    /// 첫 사망 시 성장 해금 상태에 따라 자동 부활하고, 불가능하면 결과 화면으로 이동합니다.
    /// </summary>
    private void OnDied(Health deadHealth)
    {
        int commonLevel = GameSession.Instance != null ? GameSession.Instance.SaveData.commonLevel : 1;
        RunState runState = GameSession.Instance != null ? GameSession.Instance.RunState : null;
        if (GrowthService.HasRevival(commonLevel) && runState != null && !runState.revivalUsed)
        {
            runState.revivalUsed = true;
            health.Revive(0.5f);
            return;
        }

        SaveState();
        SceneManager.LoadScene("Result");
    }

    /// <summary>
    /// 기존 씬 전환 코드와 호환되도록 플레이어 상태를 저장합니다.
    /// </summary>
    public void SaveState()
    {
        if (PlayerState.Instance == null)
        {
            return;
        }

        PlayerState.Instance.maxHealth = maxHealth;
        PlayerState.Instance.currentHealth = currentHealth;
        PlayerState.Instance.gold = gold;
    }
}
