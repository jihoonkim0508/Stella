using System;
using UnityEngine;

[CreateAssetMenu(fileName = "Stage_1", menuName = "Stella/Stage Data")]
public class StageData : ScriptableObject
{
    public int stageNumber;
    public string stageName;
    public RoomData[] rooms;
}

[Serializable]
public class RoomData
{
    public string roomName;       // 표시용 (예: "1-1", "1-Boss")
    public RoomType roomType;
    public GameObject mapPrefab;
    public EnemySpawnGroup[] enemyGroups;
}

public enum RoomType { Normal, Event, Boss }

[Serializable]
public class EnemySpawnGroup
{
    public GameObject prefab;
    [Min(0)] public int count;
}
