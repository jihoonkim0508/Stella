using System.Collections;
using UnityEngine;

public class BattleSceneFlow : MonoBehaviour
{
    [SerializeField] private float startDelay = 1f;
    [SerializeField] private float checkInterval = 0.5f;
    [SerializeField] private bool completeWhenNoEnemies;

    private bool hasSeenEnemy;
    private bool completed;

    private void Start()
    {
        GameFlowManager.EnsureExists();
        StageProgressManager.EnsureExists();
        StartCoroutine(TrackBattleClear());
    }

    public void CompleteBattleForDebug()
    {
        CompleteBattle();
    }

    private IEnumerator TrackBattleClear()
    {
        yield return new WaitForSeconds(Mathf.Max(0f, startDelay));

        var wait = new WaitForSeconds(Mathf.Max(0.1f, checkInterval));

        while (!completed)
        {
            int aliveCount = CountAliveEnemies();

            if (aliveCount > 0)
            {
                hasSeenEnemy = true;
            }
            else if (hasSeenEnemy || completeWhenNoEnemies)
            {
                CompleteBattle();
                yield break;
            }

            yield return wait;
        }
    }

    private int CountAliveEnemies()
    {
        int aliveCount = 0;

        foreach (EnemyBase enemy in FindObjectsByType<EnemyBase>(FindObjectsSortMode.None))
        {
            if (enemy != null && !enemy.IsDead)
            {
                aliveCount++;
            }
        }

        foreach (EnemyAI enemy in FindObjectsByType<EnemyAI>(FindObjectsSortMode.None))
        {
            if (enemy != null && !enemy.IsDead)
            {
                aliveCount++;
            }
        }

        return aliveCount;
    }

    private void CompleteBattle()
    {
        if (completed) return;

        completed = true;
        GameFlowManager.EnsureExists().CompleteBattle();
    }
}
