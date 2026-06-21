using UnityEngine;

/// <summary>
/// 씬 이동 중에도 유지할 플레이어 상태입니다.
/// </summary>
public class PlayerState : MonoBehaviour
{
    // 어디서든 접근할 플레이어 상태입니다.
    public static PlayerState Instance { get; private set; }

    // 플레이어 최대 체력입니다.
    public int maxHealth = 100;

    // 플레이어 현재 체력입니다.
    public int currentHealth = 100;

    // 플레이어가 가진 골드입니다.
    public int gold;

    /// <summary>
    /// 상태 오브젝트를 씬 이동 후에도 유지합니다.
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
