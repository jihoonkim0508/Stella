using UnityEngine;

/// <summary>
/// 몬스터가 엘리트인지 표시하기 위한 시각 효과 스크립트입니다.
/// 이 컴포넌트를 붙이면 기본적으로 엘리트 몬스터로 취급하며,
/// 붉은 오라 프리팹을 자식으로 붙이고 몬스터 크기를 지정 배율만큼 키울 수 있습니다.
/// </summary>
[DisallowMultipleComponent]
[ExecuteAlways]
public class EnemyEliteVisual : MonoBehaviour
{
    [Header("Elite Settings")]
    [Tooltip("체크되어 있으면 이 몬스터를 엘리트 몬스터로 취급합니다.")]
    [SerializeField] private bool isElite = true;

    [Header("Elite Scale")]
    [Tooltip("엘리트 몬스터일 때 Transform Scale을 키울지 여부입니다.")]
    [SerializeField] private bool applyEliteScale = true;

    [Tooltip("원래 스케일에 곱할 엘리트 크기 배율입니다. 예: 1,1,1 스케일에 1.5를 적용하면 1.5,1.5,1.5가 됩니다.")]
    [SerializeField] private float eliteScaleMultiplier = 1.5f;

    [Tooltip("에디터에서 값을 바꿀 때도 즉시 스케일을 반영할지 여부입니다.")]
    [SerializeField] private bool applyScaleInEditMode = true;

    [Tooltip("Is Elite를 끄거나 Apply Elite Scale을 끄면 저장해둔 원래 스케일로 되돌릴지 여부입니다.")]
    [SerializeField] private bool restoreOriginalScaleWhenDisabled = true;

    [Header("Visual Prefab")]
    [Tooltip("엘리트 몬스터 발밑에 생성할 붉은 원형 오라 프리팹입니다.")]
    [SerializeField] private GameObject eliteAuraPrefab;

    [Header("Visual Transform")]
    [SerializeField] private Vector3 localPosition = new Vector3(0f, 0.03f, 0f);
    [SerializeField] private Vector3 localRotation = new Vector3(90f, 0f, 0f);
    [SerializeField] private Vector3 localScale = new Vector3(3f, 3f, 3f);

    [Header("Debug")]
    [Tooltip("저장된 원래 스케일입니다. 보통 직접 수정하지 않아도 됩니다.")]
    [SerializeField] private Vector3 originalLocalScale = Vector3.one;

    [SerializeField, HideInInspector] private bool hasCachedOriginalScale;

    private const string AuraInstanceName = "EliteAuraVisual_Instance";
    private GameObject auraInstance;

    private void Reset()
    {
        // 컴포넌트를 처음 붙이는 순간의 스케일을 원본으로 저장합니다.
        CacheOriginalScale(true);
        isElite = true;
        ApplyEliteScale();
    }

    private void Awake()
    {
        CacheOriginalScale(false);
    }

    private void Start()
    {
        // 실제 게임 실행 중에는 오라까지 생성합니다.
        if (Application.isPlaying)
        {
            Refresh();
        }
    }

    private void OnValidate()
    {
        eliteScaleMultiplier = Mathf.Max(0.1f, eliteScaleMultiplier);

        // 에디터에서 컴포넌트를 붙이거나 값을 바꿀 때 바로 크기가 보이게 합니다.
        // 단, 오라 Instantiate는 Start/Refresh에서 처리해서 에디터 중복 생성을 줄입니다.
        if (!Application.isPlaying && applyScaleInEditMode)
        {
            CacheOriginalScale(false);
            ApplyEliteScale();
        }
    }

    /// <summary>
    /// 엘리트 상태, 스케일, 오라 프리팹을 다시 적용합니다.
    /// 런타임에서 상태를 바꿀 때 호출해도 됩니다.
    /// </summary>
    public void Refresh()
    {
        CacheOriginalScale(false);
        ApplyEliteScale();
        RefreshAura();
    }

    /// <summary>
    /// 런타임에서 일반 몬스터/엘리트 몬스터 상태를 전환하고 싶을 때 사용합니다.
    /// </summary>
    public void SetElite(bool value)
    {
        isElite = value;
        Refresh();
    }

    private void CacheOriginalScale(bool force)
    {
        if (!force && hasCachedOriginalScale) return;

        originalLocalScale = transform.localScale;
        hasCachedOriginalScale = true;
    }

    private void ApplyEliteScale()
    {
        if (!hasCachedOriginalScale)
        {
            CacheOriginalScale(false);
        }

        if (!isElite || !applyEliteScale)
        {
            if (restoreOriginalScaleWhenDisabled)
            {
                transform.localScale = originalLocalScale;
            }

            return;
        }

        transform.localScale = originalLocalScale * eliteScaleMultiplier;
    }

    private void RefreshAura()
    {
        ClearAura();

        if (!isElite || eliteAuraPrefab == null) return;

        auraInstance = Instantiate(eliteAuraPrefab, transform);
        auraInstance.name = AuraInstanceName;
        auraInstance.transform.localPosition = localPosition;
        auraInstance.transform.localEulerAngles = localRotation;
        auraInstance.transform.localScale = localScale;
    }

    private void ClearAura()
    {
        if (auraInstance != null)
        {
            DestroyAuraObject(auraInstance);
            auraInstance = null;
        }

        // 혹시 이전에 생성된 오라가 남아있으면 이름 기준으로 정리합니다.
        Transform oldAura = transform.Find(AuraInstanceName);
        if (oldAura != null)
        {
            DestroyAuraObject(oldAura.gameObject);
        }
    }

    private void DestroyAuraObject(GameObject target)
    {
        if (target == null) return;

        if (Application.isPlaying)
        {
            Destroy(target);
        }
        else
        {
            DestroyImmediate(target);
        }
    }

    [ContextMenu("Refresh Elite Visual")]
    private void RefreshFromContextMenu()
    {
        Refresh();
    }

    [ContextMenu("Cache Current Scale As Original")]
    private void CacheCurrentScaleAsOriginal()
    {
        // 현재 Transform Scale을 새 기준 스케일로 저장합니다.
        // 저장만 하고 바로 1.5배를 다시 적용하지는 않습니다.
        originalLocalScale = transform.localScale;
        hasCachedOriginalScale = true;
    }

    [ContextMenu("Apply Elite Scale")]
    private void ApplyEliteScaleFromContextMenu()
    {
        ApplyEliteScale();
    }

    [ContextMenu("Restore Original Scale")]
    private void RestoreOriginalScale()
    {
        if (!hasCachedOriginalScale) return;
        transform.localScale = originalLocalScale;
    }
}
