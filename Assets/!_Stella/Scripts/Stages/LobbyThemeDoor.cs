using UnityEngine;

/// <summary>
/// 로비의 테마 문 잠금 상태를 표시하고, 해금된 문으로 입장하면 런을 시작합니다.
/// </summary>
public class LobbyThemeDoor : MonoBehaviour
{
    [SerializeField] private int themeNumber = 1;
    [SerializeField] private GameObject unlockedVisual;
    [SerializeField] private GameObject lockedVisual;
    [SerializeField] private Collider triggerCollider;

    private bool unlocked;

    /// <summary>
    /// 저장 데이터를 기준으로 문 상태를 초기화합니다.
    /// </summary>
    private void Start()
    {
        SaveData save = GameSession.Instance != null ? GameSession.Instance.SaveData : SaveService.CreateDefault();
        ApplyUnlocked(save.unlockedThemes.Contains(themeNumber));
    }

    /// <summary>
    /// 프리팹 또는 테스트에서 테마 번호와 해금 상태를 직접 지정합니다.
    /// </summary>
    public void Configure(int themeNumber, bool unlocked)
    {
        this.themeNumber = Mathf.Clamp(themeNumber, 1, 5);
        ApplyUnlocked(unlocked);
    }

    /// <summary>
    /// 문 시각 상태와 트리거 사용 여부를 함께 갱신합니다.
    /// </summary>
    public void ApplyUnlocked(bool isUnlocked)
    {
        unlocked = isUnlocked;

        if (unlockedVisual != null)
        {
            unlockedVisual.SetActive(unlocked);
        }

        if (lockedVisual != null)
        {
            lockedVisual.SetActive(!unlocked);
        }

        Collider activeCollider = triggerCollider != null ? triggerCollider : GetComponent<Collider>();
        if (activeCollider != null)
        {
            activeCollider.isTrigger = true;
            activeCollider.enabled = unlocked;
        }
    }

    /// <summary>
    /// 플레이어가 해금된 문에 들어오면 선택 캐릭터로 해당 테마 런을 시작합니다.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (!unlocked || !other.CompareTag("Player"))
        {
            return;
        }

        SaveData save = GameSession.Instance.SaveData;
        GameSession.Instance.StartRun(save.selectedCharacter);
        StageProgress.Instance.StartTheme((StageTheme)(themeNumber - 1));
        StageSceneLoader.Instance.LoadCurrentStage();
    }
}
