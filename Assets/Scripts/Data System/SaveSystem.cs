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

        string cipher = await File.ReadAllTextAsync(FilePath);
        string json = Decrypt(cipher);
        GameData data = JsonUtility.FromJson<GameData>(json);

        if (data.version < CURRENT_VERSION)
            data = Migrate(data);

        return data;
	}

    public static async Task SaveAsync(GameData data) 
    {
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
