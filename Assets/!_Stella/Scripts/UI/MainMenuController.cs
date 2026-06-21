using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 시작 씬의 메인 메뉴 버튼과 보조 패널 전환을 담당합니다.
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [SerializeField] private Button ingameButton;
    [SerializeField] private Button characterSelectButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button settingsBackButton;
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject characterSelectPanel;
    [SerializeField] private GameObject settingsPanel;

    /// <summary>
    /// 씬에 배치된 버튼 이벤트를 연결합니다.
    /// </summary>
    private void Awake()
    {
        if (ingameButton != null)
        {
            ingameButton.onClick.AddListener(() => SceneManager.LoadScene("Lobby"));
        }

        if (characterSelectButton != null)
        {
            characterSelectButton.onClick.AddListener(() => ShowPanel(characterSelectPanel));
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(() => ShowPanel(settingsPanel));
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(Application.Quit);
        }

        if (settingsBackButton != null)
        {
            settingsBackButton.onClick.AddListener(ShowMain);
        }
    }

    /// <summary>
    /// 시작 시 메인 패널만 보이도록 정리합니다.
    /// </summary>
    private void Start()
    {
        ShowPanel(mainPanel);
    }

    /// <summary>
    /// 메인 패널로 돌아갑니다.
    /// </summary>
    public void ShowMain()
    {
        ShowPanel(mainPanel);
    }

    /// <summary>
    /// 지정한 패널 하나만 활성화합니다.
    /// </summary>
    private void ShowPanel(GameObject panel)
    {
        if (mainPanel != null)
        {
            mainPanel.SetActive(panel == mainPanel);
        }

        if (characterSelectPanel != null)
        {
            characterSelectPanel.SetActive(panel == characterSelectPanel);
        }

        if (settingsPanel != null)
        {
            settingsPanel.SetActive(panel == settingsPanel);
        }
    }
}
