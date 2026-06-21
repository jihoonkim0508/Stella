using UnityEngine;

/// <summary>
/// 현재 테마와 방 진행 순서를 보관합니다.
/// </summary>
public class StageProgress : MonoBehaviour
{
    public static StageProgress Instance { get; private set; }

    [SerializeField] private StageTheme currentTheme = StageTheme.Theme1;
    [SerializeField] private int currentIndex;

    private readonly StageType[] theme1Route =
    {
        StageType.Battle,
        StageType.Event,
        StageType.Break,
        StageType.Boss
    };

    public StageTheme CurrentTheme => currentTheme;
    public int CurrentIndex => currentIndex;
    public int CurrentRoom => currentIndex + 1;

    /// <summary>
    /// 진행 상태 오브젝트를 씬 이동에도 유지합니다.
    /// </summary>
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// 테스트에서 오브젝트를 제거할 때 싱글턴 참조를 정리합니다.
    /// </summary>
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// 선택한 테마를 처음 방부터 시작합니다.
    /// </summary>
    public void StartTheme(StageTheme theme)
    {
        currentTheme = theme;
        currentIndex = 0;
        if (GameSession.Instance != null)
        {
            GameSession.Instance.RunState.reachedTheme = (int)theme + 1;
            GameSession.Instance.RunState.reachedRoom = 1;
        }
    }

    /// <summary>
    /// 현재 방 타입을 반환합니다.
    /// </summary>
    public StageType GetCurrentStageType()
    {
        return GetRoute(currentTheme)[currentIndex];
    }

    /// <summary>
    /// 다음 방으로 진행합니다.
    /// </summary>
    public void MoveNext()
    {
        currentIndex++;
        if (GameSession.Instance != null)
        {
            GameSession.Instance.RunState.reachedRoom = Mathf.Max(GameSession.Instance.RunState.reachedRoom, CurrentRoom);
        }
    }

    /// <summary>
    /// 현재 테마의 모든 방을 완료했는지 확인합니다.
    /// </summary>
    public bool IsFinished()
    {
        return currentIndex >= GetRoute(currentTheme).Length;
    }

    /// <summary>
    /// 테마 1은 Battle, Event, Break, Boss 고정 루트입니다.
    /// </summary>
    public StageType[] GetRoute(StageTheme theme)
    {
        return theme1Route;
    }
}
