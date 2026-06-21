using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// 일시정지 패널의 표시 상태와 시간 정지를 관리합니다.
/// </summary>
public class PauseMenuController : MonoBehaviour
{
    private static PauseMenuController instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button mainMenuButton;

    /// <summary>
    /// 씬의 일시정지 컨트롤러를 등록하고 버튼 이벤트를 연결합니다.
    /// </summary>
    private void Awake()
    {
        instance = this;

        if (continueButton != null)
        {
            continueButton.onClick.AddListener(TogglePause);
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(() =>
            {
                SetPaused(false);
                SceneManager.LoadScene("Start");
            });
        }

        SetPaused(false);
    }

    /// <summary>
    /// ESC 입력 또는 버튼에서 일시정지 상태를 토글합니다.
    /// </summary>
    public static void TogglePause()
    {
        if (instance == null)
        {
            return;
        }

        instance.SetPaused(instance.panel == null || !instance.panel.activeSelf);
    }

    /// <summary>
    /// 패널, 시간, 커서를 일시정지 상태에 맞게 갱신합니다.
    /// </summary>
    private void SetPaused(bool paused)
    {
        if (panel != null)
        {
            panel.SetActive(paused);
        }

        Time.timeScale = paused ? 0f : 1f;
        Cursor.lockState = paused ? CursorLockMode.None : CursorLockMode.Locked;
        Cursor.visible = paused;
    }
}
