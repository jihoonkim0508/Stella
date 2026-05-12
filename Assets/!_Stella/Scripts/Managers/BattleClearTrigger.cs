using UnityEngine;

public class BattleClearTrigger : MonoBehaviour
{
    [SerializeField] private string playerTag = "Player";

    private bool triggered;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;
        if (!other.CompareTag(playerTag)) return;

        triggered = true;
        GameFlowManager.EnsureExists().CompleteBattle();
    }
}
