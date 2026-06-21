using TMPro;
using UnityEngine;

/// <summary>
/// 로비 HUD에 선택 캐릭터와 해금된 테마 정보를 표시합니다.
/// </summary>
public class LobbyHudController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI characterLabel;
    [SerializeField] private TextMeshProUGUI unlockedThemesLabel;

    /// <summary>
    /// 로비 진입 시 저장 데이터를 HUD에 반영합니다.
    /// </summary>
    private void Start()
    {
        SaveData save = GameSession.Instance != null ? GameSession.Instance.SaveData : SaveService.CreateDefault();

        if (characterLabel != null)
        {
            characterLabel.text = $"Character: {CharacterCatalog.GetDisplayName(save.selectedCharacter)}";
        }

        if (unlockedThemesLabel != null)
        {
            unlockedThemesLabel.text = $"Unlocked Themes: {string.Join(", ", save.unlockedThemes)}";
        }
    }
}
