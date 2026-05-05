using UnityEngine;
using System.Collections.Generic;
using UnityEngine;

public enum ThemeType
{
    Theme1,
    Theme2,
    Theme3,
    Theme4,
    Theme5
}
[CreateAssetMenu(menuName = "Game/Theme Data")]
public class ThemeData : ScriptableObject
{
    public string themeName;
    public ThemeType themeType;
    public List<MapData> maps;
}