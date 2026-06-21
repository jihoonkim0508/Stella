using UnityEngine;

/// <summary>
/// 개별 스테이지에서 사용할 표시 이름과 맵 프리팹 정보를 담는 데이터 에셋입니다.
/// </summary>
[CreateAssetMenu(menuName = "Stella/Stage/Stage Data")]
public class StageData : ScriptableObject
{
    // 저장, 참조, 디버깅에 사용할 고유 스테이지 ID입니다.
    public string stageId;

    // UI에 표시할 스테이지 이름입니다.
    public string displayName;

    // 스테이지 진입 시 배치할 맵 프리팹입니다.
    public GameObject mapPrefab;
}
