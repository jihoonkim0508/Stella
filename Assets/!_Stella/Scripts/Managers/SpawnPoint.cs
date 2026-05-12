using UnityEngine;

/// <summary>
/// 스폰포인트
/// 씬 로드되면 플레이어 스폰포인트로 이동
/// </summary>
public class SpawnPoint : MonoBehaviour
{
    [SerializeField] private const float radius = 0.5f; // Gizmos 반지름
    private void Start()
    {
        Player.Instance.MoveToSpawnPoint(transform.position);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}