using UnityEngine;

/// <summary>
/// 선택한 테마의 스테이지 진행을 시작합니다.
/// </summary>
public class StageManager : MonoBehaviour
{
    // 시작할 테마입니다.
    [SerializeField] private StageTheme startingTheme = StageTheme.Theme1;

    // 시작하자마자 첫 스테이지로 이동할지 결정합니다.
    [SerializeField] private bool loadFirstStage;

    /// <summary>
    /// 테마를 초기화하고 필요하면 첫 스테이지를 로드합니다.
    /// </summary>
    private void Start()
    {
        StageProgress.Instance.StartTheme(startingTheme);

        if (loadFirstStage)
        {
            StageSceneLoader.Instance.LoadCurrentStage();
        }
    }
}
