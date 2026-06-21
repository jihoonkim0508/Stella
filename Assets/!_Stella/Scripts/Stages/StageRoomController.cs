using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 현재 방의 적 처치 상태와 포탈 활성 조건을 관리합니다.
/// </summary>
public class StageRoomController : MonoBehaviour
{
    [SerializeField] private StagePortal portal;
    [SerializeField] private List<EnemyController> enemies = new();
    [SerializeField] private bool clearOnStart;

    public StagePortal Portal => portal;
    public int RemainingEnemies => enemies.Count;

    /// <summary>
    /// 씬에 배치된 적 목록을 정리하고 즉시 클리어 방이면 포탈을 엽니다.
    /// </summary>
    private void Start()
    {
        enemies.RemoveAll(enemy => enemy == null);

        foreach (EnemyController enemy in enemies)
        {
            enemy.SetRoom(this);
        }

        if (clearOnStart || enemies.Count == 0)
        {
            ActivatePortal();
        }
    }

    /// <summary>
    /// 스폰되거나 배치된 적을 이 방의 클리어 조건으로 등록합니다.
    /// </summary>
    public void RegisterEnemy(EnemyController enemy)
    {
        if (enemy == null || enemies.Contains(enemy))
        {
            return;
        }

        enemies.Add(enemy);
        enemy.SetRoom(this);
    }

    /// <summary>
    /// 적이 처치되면 남은 수를 갱신하고 보스 또는 방 클리어 처리를 합니다.
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
    /// 씬 또는 프리팹에서 포탈 참조를 연결합니다.
    /// </summary>
    public void SetPortal(StagePortal stagePortal)
    {
        portal = stagePortal;
    }
}
