using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    #region Singleton
    public static SceneController Instance { get; private set; }
    #endregion
    
    // 이전 씬 기록용
    private Stack<string> sceneHistory = new Stack<string>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
    /// <summary>
    /// 비동기 씬 로드
    /// </summary>
    public void LoadScene(string sceneName)
    {
        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene != sceneName)
        {
            sceneHistory.Push(currentScene);
            StartCoroutine(LoadSceneCoroutine(sceneName));
        }
    }

    private System.Collections.IEnumerator LoadSceneCoroutine(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            // 필요하면 여기서 로딩 UI 업데이트 가능
            Debug.Log($"로딩 진행률: {operation.progress}");
            yield return null;
        }
    }

    /// <summary>
    /// 현재 씬 다시 로드
    /// </summary>
    public void ReloadCurrentScene()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(currentScene);
    }

    /// <summary>
    /// 이전 씬으로 복귀
    /// </summary>
    public void LoadPreviousScene()
    {
        if (sceneHistory.Count > 0)
        {
            string previousScene = sceneHistory.Pop();
            SceneManager.LoadScene(previousScene);
        }
        else
        {
            Debug.LogWarning("이전 씬 기록이 없습니다.");
        }
    }

    /// <summary>
    /// 게임 종료
    /// </summary>
    public void QuitGame()
    {
        Debug.Log("게임 종료");
        Application.Quit();
    }
}