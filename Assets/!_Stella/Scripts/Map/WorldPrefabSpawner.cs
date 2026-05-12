using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// world 프리팹을 생성
/// </summary>
public class WorldPrefabSpawner : MonoBehaviour
{
    #region prefabs
    // 2차원을 위해서 만든 클래스
    [System.Serializable]
    private class worldPrefabRow
    {
        [SerializeField] public List<GameObject> column = new List<GameObject>();
    }

    [SerializeField] private List<worldPrefabRow> worldPrefabs;

    #endregion

    private bool isSpawnedWorld; // 월드가 이미 생성되었는지 여부
    UnityEvent onWorldSpawned = new UnityEvent();

    public void SpawnWorld()
    {
        if (isSpawnedWorld) return;

        Stage stage = StageProgressManager.EnsureExists().CurrentStage;
        Instantiate(worldPrefabs[stage.X - 1].column[stage.Y - 1], Vector3.zero, Quaternion.identity);

        isSpawnedWorld = true;
        onWorldSpawned.Invoke();
    }

    private void Awake()
    {
        SpawnWorld();
    }
}
