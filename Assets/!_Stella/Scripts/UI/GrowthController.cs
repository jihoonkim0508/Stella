using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 결과 씬의 성장 패널에서 공용 성장과 캐릭터 성장을 처리합니다.
/// </summary>
public class GrowthController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI commonLabel;
    [SerializeField] private TextMeshProUGUI characterLabel;
    [SerializeField] private Button commonUpgradeButton;
    [SerializeField] private Button characterUpgradeButton;
    [SerializeField] private Button backButton;
    [SerializeField] private ResultController resultController;

    /// <summary>
    /// 성장 버튼 이벤트를 연결합니다.
    /// </summary>
    private void Awake()
    {
        if (commonUpgradeButton != null)
        {
            commonUpgradeButton.onClick.AddListener(UpgradeCommon);
        }

        if (characterUpgradeButton != null)
        {
            characterUpgradeButton.onClick.AddListener(UpgradeCharacter);
        }

        if (backButton != null)
        {
            backButton.onClick.AddListener(() => resultController.ShowResult());
        }
    }

    /// <summary>
    /// 패널이 열릴 때 현재 성장 상태를 표시합니다.
    /// </summary>
    private void OnEnable()
    {
        Refresh();
    }

    /// <summary>
    /// 공용 성장 구매를 시도하고 저장합니다.
    /// </summary>
    private void UpgradeCommon()
    {
        GrowthService growth = new(GameSession.Instance.SaveData);
        growth.TryUpgradeCommon();
        GameSession.Instance.SaveService.Save();
        Refresh();
    }

    /// <summary>
    /// 선택 캐릭터 성장을 시도하고 저장합니다.
    /// </summary>
    private void UpgradeCharacter()
    {
        SaveData save = GameSession.Instance.SaveData;
        GrowthService growth = new(save);
        growth.TryUpgradeCharacter(save.selectedCharacter);
        GameSession.Instance.SaveService.Save();
        Refresh();
    }

    /// <summary>
    /// 성장 수치와 비용 텍스트를 갱신합니다.
    /// </summary>
    private void Refresh()
    {
        if (GameSession.Instance == null)
        {
            return;
        }

        SaveData save = GameSession.Instance.SaveData;
        CharacterProgress character = save.GetCharacterProgress(save.selectedCharacter);

        if (commonLabel != null)
        {
            commonLabel.text = $"Common Lv.{save.commonLevel} / Stars {save.commonStars}";
        }

        if (characterLabel != null)
        {
            characterLabel.text = $"{save.selectedCharacter} Lv.{character.level} / Boss Stars {character.bossStars}";
        }

        SetButtonText(commonUpgradeButton, $"Upgrade Common ({GrowthService.GetCommonCost(save.commonLevel)})");
        SetButtonText(characterUpgradeButton, $"Upgrade Character ({GrowthService.GetCharacterCost(character.level)})");
    }

    /// <summary>
    /// 버튼 안의 TMP 텍스트를 교체합니다.
    /// </summary>
    private static void SetButtonText(Button button, string text)
    {
        if (button == null)
        {
            return;
        }

        TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
        {
            label.text = text;
        }
    }
}
