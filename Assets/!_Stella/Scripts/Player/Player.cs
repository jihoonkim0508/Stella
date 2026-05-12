using UnityEngine;

/// <summary>
/// 플레이어 관리 클래스 
/// </summary>
public class Player : MonoBehaviour
{
    #region Singleton
    public static Player Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    public void MoveToSpawnPoint(Vector3 position)
    {
        transform.position = position;
    }
}