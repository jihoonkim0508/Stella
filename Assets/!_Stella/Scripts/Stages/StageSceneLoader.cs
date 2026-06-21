using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 현재 스테이지 타입에 맞는 씬을 로드합니다.
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
    /// 씬에 배치된 GameRoot의 로더를 싱글턴으로 유지합니다.
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
    /// 삭제되는 로더가 현재 싱글턴이면 참조를 정리합니다.
    /// </summary>
    private void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }

    /// <summary>
    /// 현재 진행 상태의 방 타입에 맞는 씬을 로드합니다.
    /// </summary>
    public void LoadCurrentStage()
    {
        if (!HasProgress())
        {
            return;
        }

        if (StageProgress.Instance.IsFinished())
        {
            SceneManager.LoadScene(resultSceneName);
            return;
        }

        Load(StageProgress.Instance.GetCurrentStageType());
    }

    /// <summary>
    /// 다음 방으로 진행한 뒤 해당 씬을 로드합니다.
    /// </summary>
    public void LoadNextStage()
    {
        if (!HasProgress())
        {
            return;
        }

        StageProgress.Instance.MoveNext();
        LoadCurrentStage();
    }

    /// <summary>
    /// 스테이지 타입별 씬 이름으로 이동합니다.
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
    /// 씬에 배치되어야 하는 진행 싱글턴이 준비됐는지 확인합니다.
    /// </summary>
    private static bool HasProgress()
    {
        if (StageProgress.Instance != null)
        {
            return true;
        }

        Debug.LogError("StageProgress가 없습니다. 각 씬의 _Systems 아래 GameRoot 프리팹을 배치해야 합니다.");
        return false;
    }
}
