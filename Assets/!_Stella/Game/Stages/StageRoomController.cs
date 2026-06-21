using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 현재 방의 적 수와 포탈 활성화 조건을 관리합니다.
/// </summary>
public class StageRoomController : MonoBehaviour
{
    [SerializeField] private StagePortal portal;

    private readonly List<EnemyController> enemies = new();

    public StagePortal Portal => portal;
    public int RemainingEnemies => enemies.Count;

    /// <summary>
    /// 씬 타입에 맞춰 즉시 클리어 방인지 전투 방인지 결정합니다.
    /// </summary>
    private void Start()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        if (sceneName == "EventStage" || sceneName == "BreakRoom")
        {
            ActivatePortal();
        }
    }

    /// <summary>
    /// 스폰된 적을 클리어 조건에 등록합니다.
    /// </summary>
    public void RegisterEnemy(EnemyController enemy)
    {
        enemies.Add(enemy);
    }

    /// <summary>
    /// 적 처치 후 남은 적이 없으면 포탈을 엽니다.
    /// </summary>
    public void NotifyEnemyDefeated(EnemyController enemy)
    {
        enemies.Remove(enemy);
        if (enemy.IsBoss)
        {
            StageSceneLoader.Instance.LoadNextStage();
            return;
        }

        if (enemies.Count == 0)
        {
            ActivatePortal();
        }
    }

    /// <summary>
    /// 다음 방으로 이동 가능한 포탈을 활성화합니다.
    /// </summary>
    public void ActivatePortal()
    {
        if (portal != null)
        {
            portal.SetActive(true);
        }
    }

    /// <summary>
    /// 런타임 생성 포탈을 방 컨트롤러에 연결합니다.
    /// </summary>
    public void SetPortal(StagePortal stagePortal)
    {
        portal = stagePortal;
    }
}
