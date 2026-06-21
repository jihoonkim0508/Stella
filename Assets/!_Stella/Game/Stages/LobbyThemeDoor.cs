using UnityEngine;

/// <summary>
/// 로비에서 해금된 테마 문에 들어가면 해당 테마 런을 시작합니다.
/// </summary>
public class LobbyThemeDoor : MonoBehaviour
{
    [SerializeField] private int themeNumber = 1;
    [SerializeField] private bool unlocked;

    /// <summary>
    /// 문이 연결할 테마 번호와 해금 상태를 설정합니다.
    /// </summary>
    public void Configure(int themeNumber, bool unlocked)
    {
        this.themeNumber = Mathf.Clamp(themeNumber, 1, 5);
        this.unlocked = unlocked;
    }

    /// <summary>
    /// 플레이어가 해금된 문에 들어오면 저장된 선택 캐릭터로 런을 시작합니다.
    /// </summary>
    private void OnTriggerEnter(Collider other)
    {
        if (!unlocked || !other.CompareTag("Player"))
        {
            return;
        }

        SaveData save = GameSession.Instance.SaveData;
        GameSession.Instance.StartRun(save.selectedCharacter);
        StageProgress.Instance.StartTheme((StageTheme)(themeNumber - 1));
        StageSceneLoader.Instance.LoadCurrentStage();
    }
}
