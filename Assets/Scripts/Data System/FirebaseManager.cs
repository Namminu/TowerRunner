using Firebase;
using Firebase.Analytics;
using Firebase.Crashlytics;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
	public static FirebaseManager Instance;

	private bool isInitialized;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);

			InitFirebase();
		}
		else
		{
			Destroy(gameObject);
		}
	}

	private void InitFirebase()
	{
		FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
		{
			if (task.Result == DependencyStatus.Available)
			{
				isInitialized = true;

				// Crashlytics
				Crashlytics.ReportUncaughtExceptionsAsFatal = true;

				Debug.Log("Firebase Init Success");
			}
			else
			{
				Debug.LogError($"Firebase Init Failed : {task.Result}");
			}
		});
	}

	public static void LogEvent(string eventName)
	{
		if (Instance == null || !Instance.isInitialized)
			return;

		FirebaseAnalytics.LogEvent(eventName);
	}

	public static void LogException(System.Exception e)
	{
		if (Instance == null || !Instance.isInitialized)
			return;

		Crashlytics.LogException(e);
	}

	public static void LogCrash(string message)
	{
		if (Instance == null || !Instance.isInitialized)
			return;

		Crashlytics.Log(message);
	}
}