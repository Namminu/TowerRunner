using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour, IInitializable
{
	public static DataManager Instance { get; private set; }

	private void Awake()
	{
		if(Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}
		Instance = this;
		DontDestroyOnLoad(Instance);
	}

	private async void OnApplicationQuit()
	{
		await SaveService.SaveAllAsync();
	}

	private async void OnApplicationPause(bool paused)
	{
		if (paused)
			await SaveService.SaveAllAsync();
	}

	public void Init()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}

	private void OnDestroy()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}

	private async void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		await SaveService.SaveAllAsync();
	}
}
