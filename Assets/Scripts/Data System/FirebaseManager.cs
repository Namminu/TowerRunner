using System;
using System.Collections.Generic;
using Firebase;
using Firebase.Analytics;
using Firebase.Crashlytics;
using UnityEngine;

public class FirebaseManager : MonoBehaviour
{
	public static FirebaseManager Instance;

	private bool isInitialized;
	private bool initializationFailed;
	private static readonly Queue<FirebaseEvent> pendingEvents = new();

	private readonly struct FirebaseEvent
	{
		public readonly string EventName;
		public readonly Parameter[] Parameters;
		public FirebaseEvent(string eventName, Parameter[] parameters)
		{
			EventName = eventName;
			Parameters = parameters;
		}
	}

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

	private void Start()
	{
		Debug.Log($"Firebase Init : {isInitialized}");
	}

	private void InitFirebase()
	{
		FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
		{
			if (task.Result == DependencyStatus.Available)
			{
				isInitialized = true;
				initializationFailed = false;

				Crashlytics.ReportUncaughtExceptionsAsFatal = true;

				Debug.Log("Firebase Init Success");
				FlushPendingEvents();
			}
			else
			{
				initializationFailed = true;
				Debug.LogError($"Firebase Init Failed : {task.Result}");
				pendingEvents.Clear();
			}
		});
	}

	private void FlushPendingEvents()
	{
		while (pendingEvents.Count > 0)
		{
			var e = pendingEvents.Dequeue();
			try
			{
				FirebaseAnalytics.LogEvent(e.EventName, e.Parameters);
			}
			catch (Exception ex)
			{
				Debug.LogError($"FirebaseManager.FlushPendingEvents failed: {ex}");
			}
		}
	}

	public static void LogEvent(string eventName)
	{
		if (Instance == null)
		{
			EnqueueEvent(eventName, Array.Empty<Parameter>());
			return;
		}

		if (Instance.initializationFailed)
			return;

		if (Instance.isInitialized)
		{
			FirebaseAnalytics.LogEvent(eventName);
			return;
		}

		EnqueueEvent(eventName, Array.Empty<Parameter>());
	}

	public static void LogEvent(string eventName, params Parameter[] parameters)
	{
		if (Instance == null)
		{
			EnqueueEvent(eventName, parameters ?? Array.Empty<Parameter>());
			return;
		}

		if (Instance.initializationFailed)
			return;

		if (Instance.isInitialized)
		{
			FirebaseAnalytics.LogEvent(eventName, parameters);
			return;
		}

		EnqueueEvent(eventName, parameters ?? Array.Empty<Parameter>());
	}

	private static void EnqueueEvent(string eventName, Parameter[] parameters)
	{
		if (pendingEvents.Count >= 50)
		{
			pendingEvents.Dequeue();
		}

		pendingEvents.Enqueue(new FirebaseEvent(eventName, parameters));
	}

	public static void LogCrash(string message)
	{
		if (Instance == null || !Instance.isInitialized)
			return;

		Crashlytics.Log(message);
	}
}