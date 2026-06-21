using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 테마별 스테이지 진행 순서를 담는 데이터입니다.
/// </summary>
[CreateAssetMenu(menuName = "Stella/Stage/Stage Route")]
public class StageRoute : ScriptableObject
{
    // 5개 테마의 진행 순서 목록입니다.
    public List<StageThemeRoute> themes = new();

    /// <summary>
    /// 선택한 테마의 진행 순서를 반환합니다.
    /// </summary>
    public List<StageType> GetSteps(StageTheme theme)
    {
        foreach (StageThemeRoute route in themes)
        {
            if (route.theme == theme)
            {
                return route.steps;
            }
        }

        Debug.LogError($"등록되지 않은 테마입니다: {theme}");
        return null;
    }
}

/// <summary>
/// 하나의 테마와 해당 테마의 스테이지 타입 목록입니다.
/// </summary>
[System.Serializable]
public class StageThemeRoute
{
    // 적용할 테마입니다.
    public StageTheme theme;

    // Battle, Event, Break, Boss가 등장하는 순서입니다.
    public List<StageType> steps = new();
}

/// <summary>
/// 스테이지 테마 종류입니다.
/// </summary>
public enum StageTheme
{
    Theme1,
    Theme2,
    Theme3,
    Theme4,
    Theme5
}

/// <summary>
/// 스테이지 진행 타입입니다.
/// </summary>
public enum StageType
{
    Battle,
    Event,
    Break,
    Boss
}
