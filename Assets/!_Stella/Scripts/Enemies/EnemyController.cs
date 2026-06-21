using UnityEngine;

/// <summary>
/// 회색박스 적의 추적, 공격, 사망 보상 처리를 담당합니다.
/// </summary>
[RequireComponent(typeof(Health))]
public class EnemyController : MonoBehaviour
{
    [SerializeField] private int contactDamage = 10;
    [SerializeField] private int commonStarReward = 10;
    [SerializeField] private float moveSpeed = 2.5f;
    [SerializeField] private float attackRange = 1.6f;
    [SerializeField] private float attackCooldown = 1f;
    [SerializeField] private bool isBoss;
    [SerializeField] private CharacterId bossId = CharacterId.Leo;

    private Transform player;
    private Health health;
    private StageRoomController room;
    private float nextAttackTime;

    public bool IsBoss => isBoss;
    public CharacterId BossId => bossId;

    /// <summary>
    /// 씬에 배치된 방 컨트롤러를 직접 연결해 런타임 검색을 줄입니다.
    /// </summary>
    public void SetRoom(StageRoomController stageRoom)
    {
        room = stageRoom;
    }

    /// <summary>
    /// 체력 이벤트와 플레이어 대상을 준비합니다.
    /// </summary>
    private void Awake()
    {
        health = GetComponent<Health>();
        health.Died += OnDied;
    }

    /// <summary>
    /// 플레이어를 찾아 단순 추적과 근접 공격을 수행합니다.
    /// </summary>
    private void Update()
    {
        if (health.IsDead)
        {
            return;
        }

        if (player == null)
        {
            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            player = playerObject != null ? playerObject.transform : null;
        }

        if (player == null)
        {
            return;
        }

        Vector3 delta = player.position - transform.position;
        delta.y = 0f;
        if (delta.magnitude > attackRange)
        {
            transform.position += delta.normalized * moveSpeed * Time.deltaTime;
            return;
        }

        if (Time.time >= nextAttackTime && player.TryGetComponent(out Health playerHealth))
        {
            nextAttackTime = Time.time + attackCooldown;
            playerHealth.TakeDamage(new DamageInfo(gameObject, contactDamage));
        }
    }

    /// <summary>
    /// 적 타입별 기본값을 설정합니다.
    /// </summary>
    public void Configure(EnemyKind kind)
    {
        EnsureHealth();

        switch (kind)
        {
            case EnemyKind.Melee:
                health.Configure(30, 0);
                contactDamage = 10;
                commonStarReward = 10;
                moveSpeed = 2.8f;
                break;
            case EnemyKind.Tanker:
                health.Configure(80, 2);
                contactDamage = 15;
                commonStarReward = 20;
                moveSpeed = 1.6f;
                break;
            case EnemyKind.Ranged:
                health.Configure(25, 0);
                contactDamage = 8;
                commonStarReward = 15;
                moveSpeed = 2.2f;
                attackRange = 5f;
                break;
        }
    }

    /// <summary>
    /// 보스 기본값과 별자리 ID를 설정합니다.
    /// </summary>
    public void ConfigureBoss(CharacterId id)
    {
        EnsureHealth();

        isBoss = true;
        bossId = id;
        health.Configure(180, 3);
        contactDamage = id switch
        {
            CharacterId.Capricorn => 18,
            CharacterId.Sagittarius => 14,
            _ => 16
        };
        moveSpeed = id == CharacterId.Capricorn ? 1.8f : 2.4f;
        commonStarReward = 0;
    }

    /// <summary>
    /// 사망 시 런 재화를 누적하고 방 관리자에게 알립니다.
    /// </summary>
    private void OnDied(Health deadHealth)
    {
        if (GameSession.Instance != null)
        {
            if (isBoss)
            {
                GameSession.Instance.AddBossClear(bossId);
            }
            else
            {
                GameSession.Instance.RunState.commonStars += commonStarReward;
            }
        }

        if (room != null)
        {
            room.NotifyEnemyDefeated(this);
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// 에디터 프리팹 생성처럼 Awake 전 설정되는 경우에도 Health 참조를 보장합니다.
    /// </summary>
    private void EnsureHealth()
    {
        if (health == null)
        {
            health = GetComponent<Health>();
        }
    }
}

/// <summary>
/// 초기 전투방에 배치되는 일반 몬스터 종류입니다.
/// </summary>
public enum EnemyKind
{
    Melee,
    Tanker,
    Ranged
}
