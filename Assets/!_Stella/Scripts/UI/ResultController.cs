using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 결과 씬에서 런 보상을 저장하고 요약 정보를 표시합니다.
/// </summary>
public class ResultController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI commonStarLabel;
    [SerializeField] private TextMeshProUGUI bossStarLabel;
    [SerializeField] private TextMeshProUGUI defeatedBossesLabel;
    [SerializeField] private TextMeshProUGUI unlockedCharactersLabel;
    [SerializeField] private TextMeshProUGUI reachedStageLabel;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private GameObject growthPanel;
    [SerializeField] private Button growthButton;
    [SerializeField] private Button replayButton;

    private bool committed;

    /// <summary>
    /// 결과 버튼 이벤트를 연결합니다.
    /// </summary>
    private void Awake()
    {
        if (growthButton != null)
        {
            growthButton.onClick.AddListener(ShowGrowth);
        }

        if (replayButton != null)
        {
            replayButton.onClick.AddListener(() => SceneManager.LoadScene("Start"));
        }
    }

    /// <summary>
    /// 결과 씬 진입 시 런 보상을 한 번만 정산합니다.
    /// </summary>
    private void Start()
    {
        RunSummary summary = committed ? GameSession.Instance.RunState.ToSummary() : GameSession.Instance.CommitRun();
        committed = true;

        if (commonStarLabel != null)
        {
            commonStarLabel.text = $"Common Star +{summary.commonStars}";
        }

        if (bossStarLabel != null)
        {
            bossStarLabel.text = $"Boss Star +{summary.bossStars}";
        }

        if (defeatedBossesLabel != null)
        {
            defeatedBossesLabel.text = $"Defeated Bosses: {(summary.defeatedBosses.Count == 0 ? "None" : string.Join(", ", summary.defeatedBosses))}";
        }

        if (unlockedCharactersLabel != null)
        {
            unlockedCharactersLabel.text = $"Unlocked: {(summary.newlyUnlockedCharacters.Count == 0 ? "None" : string.Join(", ", summary.newlyUnlockedCharacters))}";
        }

        if (reachedStageLabel != null)
        {
            reachedStageLabel.text = $"Reached Theme {summary.reachedTheme}, Room {summary.reachedRoom}";
        }

        ShowResult();
    }

    /// <summary>
    /// 결과 패널을 다시 표시합니다.
    /// </summary>
    public void ShowResult()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(true);
        }

        if (growthPanel != null)
        {
            growthPanel.SetActive(false);
        }
    }

    /// <summary>
    /// 성장 패널로 전환합니다.
    /// </summary>
    private void ShowGrowth()
    {
        if (resultPanel != null)
        {
            resultPanel.SetActive(false);
        }

        if (growthPanel != null)
        {
            growthPanel.SetActive(true);
        }
    }
}
