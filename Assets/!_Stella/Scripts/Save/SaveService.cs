using System.IO;
using UnityEngine;

/// <summary>
/// Application.persistentDataPath의 단일 JSON 세이브 파일을 관리합니다.
/// </summary>
public class SaveService
{
    public const string FileName = "stella_save.json";

    private readonly string savePath;

    public SaveData Current { get; private set; }

    /// <summary>
    /// 기본 저장 경로를 사용하는 서비스입니다.
    /// </summary>
    public SaveService() : this(Application.persistentDataPath)
    {
    }

    /// <summary>
    /// 테스트에서 임시 경로를 주입하기 위한 생성자입니다.
    /// </summary>
    public SaveService(string rootPath)
    {
        savePath = Path.Combine(rootPath, FileName);
    }

    /// <summary>
    /// 저장 파일을 로드하거나 기본 데이터를 생성합니다.
    /// </summary>
    public SaveData Load()
    {
        if (!File.Exists(savePath))
        {
            Current = CreateDefault();
            return Current;
        }

        string json = File.ReadAllText(savePath);
        Current = JsonUtility.FromJson<SaveData>(json) ?? CreateDefault();
        Current.EnsureDefaults();
        return Current;
    }

    /// <summary>
    /// 현재 데이터를 JSON 파일로 저장합니다.
    /// </summary>
    public void Save()
    {
        if (Current == null)
        {
            Current = CreateDefault();
        }

        Directory.CreateDirectory(Path.GetDirectoryName(savePath));
        string json = JsonUtility.ToJson(Current, true);
        File.WriteAllText(savePath, json);
    }

    /// <summary>
    /// 기본 해금과 시작 레벨을 가진 새 세이브를 만듭니다.
    /// </summary>
    public static SaveData CreateDefault()
    {
        SaveData data = new();
        data.EnsureDefaults();
        return data;
    }
}
