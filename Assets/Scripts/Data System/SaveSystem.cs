using System;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;


public static class SaveSystem
{
    private const int CURRENT_VERSION = 1;
    private static string FilePath => Path.Combine(Application.persistentDataPath, "Game_SaveFile.json");

    public static async Task<GameData> LoadAsync()
    {
        if (!File.Exists(FilePath))
            return new GameData();

        Debug.Log(File.ReadAllText(FilePath));

        string cipher = await File.ReadAllTextAsync(FilePath);
        string json = Decrypt(cipher);

        try
        {
            GameData data = JsonUtility.FromJson<GameData>(json);
            if (data == null)
            {
                Debug.LogWarning("SaveSystem.LoadAsync: parsed GameData is null. Using default data.");
                return new GameData();
            }

            if (data.version < CURRENT_VERSION)
                data = Migrate(data);

            return data;
        }
        catch (Exception ex)
        {
            Debug.LogError($"SaveSystem.LoadAsync failed: {ex}");
            return new GameData();
        }
    }

    public static async Task SaveAsync(GameData data) 
    {
        if (data == null)
        {
            Debug.LogWarning("SaveSystem.SaveAsync skipped: GameData is null.");
            return;
        }

        data.version = CURRENT_VERSION;
        string json = JsonUtility.ToJson(data, prettyPrint: true);
        string cipher = Encrypt(json);
        await File.WriteAllTextAsync(FilePath, cipher);
    }

    public static void Delete() => File.Delete(FilePath);

    private static string Encrypt(string plain) => plain;
    private static string Decrypt(string cipher) => cipher;
    private static GameData Migrate(GameData oldData)
    {
        oldData.version = CURRENT_VERSION;
        return oldData;
    }
}
