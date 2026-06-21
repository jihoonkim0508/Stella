using UnityEngine;

/// <summary>
/// 플레이어가 들어오면 다음 스테이지로 이동하는 포탈입니다.
/// </summary>
public class StagePortal : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private bool activeOnStart;

    private bool isLoading;
    private Collider triggerCollider;

    public bool IsActive { get; private set; }

    /// <summary>
    /// 시작 활성화 여부에 따라 포탈 표시와 충돌을 맞춥니다.
    /// </summary>
    private void Awake()
    {
        triggerCollider = GetComponent<Collider>();
        SetActive(activeOnStart);
    }

    /// <summary>
    /// 전투 클리어 후 포탈을 활성화합니다.
    /// </summary>
    public void SetActive(bool isActive)
    {
        IsActive = isActive;
        if (triggerCollider != null)
        {
            triggerCollider.enabled = isActive;
        }

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.enabled = isActive;
        }
    }

    /// <summary>
    /// 3D 트리거에 들어온 오브젝트를 확인합니다.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        TryMove(other.gameObject);
    }

    /// <summary>
    /// 2D 트리거에 들어온 오브젝트를 확인합니다.
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        TryMove(other.gameObject);
    }

    /// <summary>
    /// 플레이어라면 상태를 저장하고 다음 스테이지로 이동합니다.
    /// </summary>
    private void TryMove(GameObject target)
    {
        if (!IsActive || isLoading || !target.CompareTag(playerTag))
        {
            return;
        }

        Player player = target.GetComponent<Player>();
        if (player != null)
        {
            player.SaveState();
        }

        isLoading = true;
        StageSceneLoader.Instance.LoadNextStage();
    }
}
