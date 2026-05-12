using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

/// <summary>
/// 씬 매니저
/// LoadScene(string s)를 통해서 씬 로드
/// </summary>
public class SceneController : MonoBehaviour
{
    #region Singleton
    public static SceneController Instance { get; private set; }

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
    #endregion
    
    private Stack<string> sceneHistory = new Stack<string>();

    public void LoadScene(string sceneName)
    {
        if (string.IsNullOrWhiteSpace(sceneName))
        {
            Debug.LogWarning("[SceneController] Scene name is null or empty. Cannot load scene.");
            return;
        }

        string currentScene = SceneManager.GetActiveScene().name;

        if (currentScene != sceneName)
        {
            sceneHistory.Push(currentScene);
            StartCoroutine(LoadSceneCoroutine(sceneName));
        }
    }

    private IEnumerator LoadSceneCoroutine(string sceneName)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(sceneName);

        while (!operation.isDone)
        {
            Debug.Log($"[SceneController] Loading Progress: {operation.progress}");
            yield return null;
        }
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void LoadPreviousScene()
    {
        if (sceneHistory.Count > 0)
        {
            string previousScene = sceneHistory.Pop();
            SceneManager.LoadScene(previousScene);
        }
        else
        {
            Debug.LogWarning("[SceneController] No previous scene in history to load.");
        }
    }

    public void QuitGame()
    {
        Debug.Log("[SceneController] Quitting game.");
        Application.Quit();
    }
}