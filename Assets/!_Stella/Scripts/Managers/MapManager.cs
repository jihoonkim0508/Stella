using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance { get; private set; }

    [Header("Stage Data")]
    [SerializeField] StageData[] stages;

    [Header("Spawn Settings")]
    [SerializeField] float spawnRadius = 8f;
    [SerializeField] string spawnPointTag = "SpawnPoint";

    [Header("Theme")]
    [SerializeField] string themeName;

    // 방 클리어(적 전멸) 시 발생 — 외부에서 NextRoom() 호출로 진행
    public event Action OnRoomClear;
    // 스테이지의 마지막 방까지 클리어 완료 시 발생
    public event Action OnStageComplete;

    StageData _currentStageData;
    int _currentRoomIndex;

    public int CurrentStage => _currentStageData != null ? _currentStageData.stageNumber : -1;
    public int CurrentRoomIndex => _currentRoomIndex;

    public RoomData CurrentRoom =>
        _currentStageData != null && _currentRoomIndex < _currentStageData.rooms.Length
            ? _currentStageData.rooms[_currentRoomIndex]
            : null;

    public bool IsLastRoom =>
        _currentStageData != null &&
        _currentRoomIndex >= _currentStageData.rooms.Length - 1;

    GameObject _spawnedMap;
    readonly List<GameObject> _spawnedEnemies = new();
    Coroutine _roomClearRoutine;

    void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    // ─── 스테이지 로드 ────────────────────────────────────────────

    // 스테이지 번호로 로드 (첫 번째 방부터 시작)
    public void LoadStage(int stageNumber)
    {
        StageData data = FindStageData(stageNumber);
        if (data == null)
        {
            Debug.LogWarning($"[MapManager] Stage {stageNumber} 데이터를 찾을 수 없습니다.");
            return;
        }

        _currentStageData = data;
        _currentRoomIndex = 0;
        LoadCurrentRoom();
    }

    StageData FindStageData(int stageNumber)
    {
        if (stages == null) return null;
        foreach (var s in stages)
            if (s != null && s.stageNumber == stageNumber)
                return s;
        return null;
    }

    // ─── 방 진행 ─────────────────────────────────────────────────

    // OnRoomClear 이벤트 수신 후 외부에서 호출
    public void NextRoom()
    {
        if (_currentStageData == null) return;

        _currentRoomIndex++;

        if (_currentRoomIndex >= _currentStageData.rooms.Length)
        {
            ClearRoom();
            OnStageComplete?.Invoke();
            return;
        }

        LoadCurrentRoom();
    }

    void LoadCurrentRoom()
    {
        RoomData room = CurrentRoom;
        if (room == null) return;

        ClearRoom();
        SpawnMap(room);
        SpawnEnemies(room);

        if (_spawnedEnemies.Count > 0)
        {
            if (_roomClearRoutine != null) StopCoroutine(_roomClearRoutine);
            _roomClearRoutine = StartCoroutine(TrackRoomClear());
        }
        else
        {
            // 적이 없는 방(이벤트 등)은 즉시 클리어 이벤트 발생
            OnRoomClear?.Invoke();
        }
    }

    // ─── 맵 / 적 스폰 ────────────────────────────────────────────

    void SpawnMap(RoomData room)
    {
        if (room.mapPrefab == null) return;
        _spawnedMap = Instantiate(room.mapPrefab, Vector3.zero, Quaternion.identity);
    }

    void SpawnEnemies(RoomData room)
    {
        if (room.enemyGroups == null) return;

        List<Transform> spawnPoints = GetSpawnPoints();
        int spawnIndex = 0;

        foreach (var group in room.enemyGroups)
        {
            if (group.prefab == null) continue;

            for (int i = 0; i < group.count; i++)
            {
                Vector3 pos = spawnPoints.Count > 0
                    ? spawnPoints[spawnIndex++ % spawnPoints.Count].position
                    : GetRandomSpawnPosition();

                _spawnedEnemies.Add(Instantiate(group.prefab, pos, Quaternion.identity));
            }
        }
    }

    List<Transform> GetSpawnPoints()
    {
        var points = new List<Transform>();
        if (_spawnedMap == null) return points;

        foreach (Transform t in _spawnedMap.GetComponentsInChildren<Transform>(true))
            if (t.CompareTag(spawnPointTag))
                points.Add(t);

        return points;
    }

    Vector3 GetRandomSpawnPosition()
    {
        Vector2 circle = UnityEngine.Random.insideUnitCircle * spawnRadius;
        return new Vector3(circle.x, 0f, circle.y);
    }

    // ─── 클리어 감지 / 정리 ──────────────────────────────────────

    IEnumerator TrackRoomClear()
    {
        yield return new WaitForSeconds(1f);

        var wait = new WaitForSeconds(0.5f);
        while (true)
        {
            bool allDead = true;
            foreach (var e in _spawnedEnemies)
                if (e != null) { allDead = false; break; }

            if (allDead)
            {
                OnRoomClear?.Invoke();
                yield break;
            }

            yield return wait;
        }
    }

    public void ClearRoom()
    {
        if (_roomClearRoutine != null)
        {
            StopCoroutine(_roomClearRoutine);
            _roomClearRoutine = null;
        }

        if (_spawnedMap != null)
        {
            Destroy(_spawnedMap);
            _spawnedMap = null;
        }

        foreach (var e in _spawnedEnemies)
            if (e != null) Destroy(e);
        _spawnedEnemies.Clear();
    }
}
