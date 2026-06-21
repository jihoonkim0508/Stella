using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 캐릭터 선택 패널에서 해금된 캐릭터만 선택 가능하도록 버튼을 구성합니다.
/// </summary>
public class CharacterSelectController : MonoBehaviour
{
    [SerializeField] private Transform buttonRoot;
    [SerializeField] private Button buttonTemplate;
    [SerializeField] private Button backButton;
    [SerializeField] private MainMenuController mainMenu;

    /// <summary>
    /// 저장 데이터를 읽어 캐릭터 버튼을 준비합니다.
    /// </summary>
    private void OnEnable()
    {
        Rebuild();
    }

    /// <summary>
    /// 뒤로가기 버튼을 메인 메뉴로 연결합니다.
    /// </summary>
    private void Awake()
    {
        if (backButton != null)
        {
            backButton.onClick.AddListener(() => mainMenu.ShowMain());
        }
    }

    /// <summary>
    /// 캐릭터 목록을 현재 해금 상태에 맞게 다시 만듭니다.
    /// </summary>
    private void Rebuild()
    {
        if (buttonRoot == null || buttonTemplate == null)
        {
            return;
        }

        for (int i = buttonRoot.childCount - 1; i >= 0; i--)
        {
            Transform child = buttonRoot.GetChild(i);
            if (child != buttonTemplate.transform)
            {
                Destroy(child.gameObject);
            }
        }

        SaveData save = GameSession.Instance != null ? GameSession.Instance.SaveData : SaveService.CreateDefault();
        buttonTemplate.gameObject.SetActive(false);

        foreach (CharacterId id in CharacterCatalog.AllCharacters)
        {
            Button button = Instantiate(buttonTemplate, buttonRoot);
            bool unlocked = save.unlockedCharacters.Contains(id);
            button.gameObject.SetActive(true);
            button.interactable = unlocked;
            button.name = $"{id}Button";

            TextMeshProUGUI label = button.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = unlocked ? CharacterCatalog.GetDisplayName(id) : $"{CharacterCatalog.GetDisplayName(id)} (Locked)";
            }

            button.onClick.AddListener(() =>
            {
                save.selectedCharacter = id;
                GameSession.Instance.SaveService.Save();
                mainMenu.ShowMain();
            });
        }
    }
}
