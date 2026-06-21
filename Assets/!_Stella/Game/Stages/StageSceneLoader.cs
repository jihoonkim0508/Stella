using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 스테이지 타입에 맞는 씬을 로드합니다.
/// </summary>
public class StageSceneLoader : MonoBehaviour
{
    public static StageSceneLoader Instance { get; private set; }

    [SerializeField] private string battleSceneName = "BattleStage";
    [SerializeField] private string eventSceneName = "EventStage";
    [SerializeField] private string breakSceneName = "BreakRoom";
    [SerializeField] private string bossSceneName = "BossStage";
    [SerializeField] private string resultSceneName = "Result";

    /// <summary>
    /// 씬 이동 전역 로더를 유지합니다.
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
    /// 현재 스테이지 씬을 로드합니다.
    /// </summary>
    public void LoadCurrentStage()
    {
        EnsureProgress();
        if (StageProgress.Instance.IsFinished())
        {
            SceneManager.LoadScene(resultSceneName);
            return;
        }

        Load(StageProgress.Instance.GetCurrentStageType());
    }

    /// <summary>
    /// 다음 스테이지로 진행하고 씬을 로드합니다.
    /// </summary>
    public void LoadNextStage()
    {
        EnsureProgress();
        StageProgress.Instance.MoveNext();
        LoadCurrentStage();
    }

    /// <summary>
    /// 스테이지 타입에 맞는 씬을 로드합니다.
    /// </summary>
    private void Load(StageType type)
    {
        switch (type)
        {
            case StageType.Battle:
                SceneManager.LoadScene(battleSceneName);
                break;
            case StageType.Event:
                SceneManager.LoadScene(eventSceneName);
                break;
            case StageType.Break:
                SceneManager.LoadScene(breakSceneName);
                break;
            case StageType.Boss:
                SceneManager.LoadScene(bossSceneName);
                break;
        }
    }

    /// <summary>
    /// 테스트나 빈 씬에서 진행 오브젝트가 없으면 즉시 생성합니다.
    /// </summary>
    private static void EnsureProgress()
    {
        if (StageProgress.Instance != null)
        {
            return;
        }

        GameObject progressObject = new("StageProgress");
        progressObject.AddComponent<StageProgress>().StartTheme(StageTheme.Theme1);
    }
}
