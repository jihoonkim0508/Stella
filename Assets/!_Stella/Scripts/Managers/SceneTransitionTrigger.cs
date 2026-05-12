using UnityEngine;

/// <summary>
/// 씬 이동 트리거
/// nextrSceneName으로 이동할 씬 설정
/// triggerOn/Off를 통해서 트리거 활성화 여부 설정
/// </summary>
public class SceneTransitionTrigger : MonoBehaviour
{
    [SerializeField] private string nextSceneName = "";
    [SerializeField] private bool triggerOnTagEnter = false; // Trigger when an object with the specified tag enters
    [SerializeField] private string Tag = "Player";
    
    public void TransitionScene()
    {
        if (string.IsNullOrWhiteSpace(nextSceneName))
        {
            Debug.LogWarning("[SceneTransitionTrigger] Next scene name is not set.");
            return;
        }
        SceneController.Instance.LoadScene(nextSceneName);
    }

    public void TriggerOn()
    {
        triggerOnTagEnter = true;
    }

    public void TriggerOff()
    {
        triggerOnTagEnter = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!triggerOnTagEnter) return;
        if (!other.CompareTag(Tag)) return;

        TransitionScene();
    }

    #region Editor Gizmos
    [SerializeField] private const float radius = 0.5f; // Gizmos 반지름
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
    #endregion
}