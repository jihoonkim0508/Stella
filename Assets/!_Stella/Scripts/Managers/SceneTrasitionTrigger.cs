using UnityEngine;

public class SceneTransitionTrigger : MonoBehaviour
{
    [SerializeField]
    private string nextSceneName = "None";

    public void TransitionScene()
    {
        if (nextSceneName == "None" || string.IsNullOrEmpty(nextSceneName))
        {
            Debug.LogWarning("이동할 씬 이름이 설정되지 않았습니다.");
            return;
        }

        SceneController.Instance.LoadScene(nextSceneName);
    }
}