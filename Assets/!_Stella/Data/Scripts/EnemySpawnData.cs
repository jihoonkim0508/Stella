using UnityEngine;

public enum EnemyType
{
    Chaser,
    Tank,
    Shooter
}

[System.Serializable]
public class EnemySpawnData
{
    public EnemyType enemyType;
    public Vector3 pos;
}