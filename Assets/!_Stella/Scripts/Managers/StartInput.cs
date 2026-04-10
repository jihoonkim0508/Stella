using UnityEngine;

public class StartInput : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "Lobby";
    private void Update()
    {
        if(Input.anyKeyDown)
        {
            SceneController.Instance.LoadScene(nextSceneName);
        }
    }
}
