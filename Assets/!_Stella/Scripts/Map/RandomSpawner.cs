using System.Collections.Generic;
using UnityEngine;

public class RandomSpawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject lobbyPrefab;
    public GameObject[] normalPrefabs;
    public GameObject eventPrefab;
    public GameObject bossPrefab;

    [Header("Settings")]
    public float offsetX = 10f;

    private Dictionary<string, Queue<GameObject>> pool = new Dictionary<string, Queue<GameObject>>();
    private List<GameObject> spawnedFlow = new List<GameObject>();

    private Transform poolParent;
    private Transform mapParent;

    private List<GameObject> flowData = new List<GameObject>();
    private int currentIndex = 0;

    void Awake()
    {
        poolParent = new GameObject("Pool").transform;
        mapParent = new GameObject("MapRoot").transform;
    }

    void Start()
    {
        GenerateFlow();
        SpawnNext(); // 첫 맵만 생성
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ClearAll();
            GenerateFlow();
            SpawnNext();
        }

        // 테스트용: N키로 다음 맵 생성
        if (Input.GetKeyDown(KeyCode.N))
        {
            SpawnNext();
        }
    }

    void GenerateFlow()
    {
        flowData.Clear();
        currentIndex = 0;

        // 로비
        flowData.Add(lobbyPrefab);

        // 잡몹 4개
        int prev = -1;
        for (int i = 0; i < 4; i++)
        {
            int idx = GetRandomIndexExclude(prev, normalPrefabs.Length);
            flowData.Add(normalPrefabs[idx]);
            prev = idx;
        }

        // 이벤트 (첫 방 제외)
        int insert = Random.Range(2, flowData.Count);
        flowData.Insert(insert, eventPrefab);

        // 보스
        flowData.Add(bossPrefab);
    }

    public void SpawnNext()
    {
        if (currentIndex >= flowData.Count) return;

        Vector3 pos = transform.position + new Vector3(currentIndex * offsetX, 0, 0);

        GameObject obj = GetFromPool(flowData[currentIndex]);
        obj.transform.SetParent(mapParent);
        obj.transform.position = pos;

        spawnedFlow.Add(obj);
        currentIndex++;
    }

    // 클리어 시 이 함수 호출
    public void OnMapClear()
    {
        SpawnNext();
    }

    void ClearAll()
    {
        foreach (var obj in spawnedFlow)
        {
            ReturnToPool(obj);
        }

        spawnedFlow.Clear();
    }

    GameObject GetFromPool(GameObject prefab)
    {
        string key = prefab.name;

        if (!pool.ContainsKey(key))
            pool[key] = new Queue<GameObject>();

        var q = pool[key];

        if (q.Count > 0)
        {
            GameObject obj = q.Dequeue();
            obj.SetActive(true);
            return obj;
        }

        return Instantiate(prefab);
    }

    void ReturnToPool(GameObject obj)
    {
        string key = obj.name.Replace("(Clone)", "");

        obj.SetActive(false);
        obj.transform.SetParent(poolParent);

        if (!pool.ContainsKey(key))
            pool[key] = new Queue<GameObject>();

        pool[key].Enqueue(obj);
    }

    int GetRandomIndexExclude(int exclude, int max)
    {
        int rand = Random.Range(0, max - 1);
        if (rand >= exclude) rand++;
        return rand;
    }
}