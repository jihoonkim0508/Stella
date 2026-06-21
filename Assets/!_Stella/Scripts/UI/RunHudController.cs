using TMPro;
using UnityEngine;

/// <summary>
/// 전투 진행 중 방 정보와 현재 획득 재화를 표시합니다.
/// </summary>
public class RunHudController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomLabel;
    [SerializeField] private TextMeshProUGUI commonStarLabel;
    [SerializeField] private TextMeshProUGUI bossStarLabel;

    /// <summary>
    /// HUD 텍스트를 최신 런 상태로 갱신합니다.
    /// </summary>
    private void Update()
    {
        if (GameSession.Instance == null || StageProgress.Instance == null)
        {
            return;
        }

        RunState runState = GameSession.Instance.RunState;
        if (roomLabel != null)
        {
            roomLabel.text = $"Room: {StageProgress.Instance.GetCurrentStageType()} {StageProgress.Instance.CurrentRoom}/4";
        }

        if (commonStarLabel != null)
        {
            commonStarLabel.text = $"Common Star: {runState.commonStars}";
        }

        if (bossStarLabel != null)
        {
            bossStarLabel.text = $"Boss Star: {runState.bossStars}";
        }
    }
}
