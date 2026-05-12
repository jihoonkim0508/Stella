using UnityEngine;

public class StartInput : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "Lobby";
    private void Update()
    {
        if(Input.anyKeyDown)
        {
            GameFlowManager.EnsureExists().ResetRun();

            if (SceneController.Instance != null)
            {
                SceneController.Instance.LoadScene(nextSceneName);
            }
            else
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene(nextSceneName);
            }
        }
    }
}
