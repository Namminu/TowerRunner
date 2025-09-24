using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MapScrollManager : MonoBehaviour, IInitializable
{
	[SerializeField] private AssetReferenceGameObject[] mapScrollPrefs;

	private float _lastBornY;
	private float _middleHeight;

	private void Awake()
	{
		//// Temp
		//Init();
	}

	public void Init()
	{
		StartCoroutine(InitRoutine());
	}

	private IEnumerator InitRoutine()
	{
		yield return StartCoroutine(MapInitRoutine());
		GameBus.Publish(new SubsystemReady(SubsystemId.Map));
	}

	private IEnumerator MapInitRoutine()
	{
		// 이미지 생성 기준점 정립
		float newBottomY = ScreenBounds.LowerY;

		// 리스트 길이만큼 반복 시행
		for(int i = 0; i< mapScrollPrefs.Length; i++)
		{
			var handle = Addressables.InstantiateAsync(mapScrollPrefs[i]);
			yield return handle;

			if(handle.Status != AsyncOperationStatus.Succeeded)
			{
				Debug.LogError($"[Map Scroll Manager] Map Scroll Prefs Instantiate Failed : {i}th");
				yield break;
			}

			// 이미지 생성 및 위치 정립
			GameObject go = handle.Result;

			Renderer rd = go.GetComponent<Renderer>();
			if (!rd) Debug.LogError("Map Scroll Prefs has No Renderer");

			int height = Mathf.FloorToInt(rd.bounds.size.y);
			Debug.Log("Height : " + height);

			float newY = newBottomY + (height * 0.5f);
			Debug.Log("newY : " + newY);

			_middleHeight = (height * 0.5f);
			Vector3 newPos = new Vector3(0.05f, newY, 0f);
			go.transform.position = newPos;

			// On Out of Bound 이벤트 연결
			if(!go.TryGetComponent(out ObjectMover mover))
				mover = go.AddComponent<ObjectMover>();
			mover.OnOutofBounds += HandleOutofBound;

			// 생성 기준점 재정립
			if(i < mapScrollPrefs.Length -1)
				newBottomY += height;
		}

		// OutofBound의 고정 기준점
		_lastBornY = newBottomY;

		yield return null;
	}

	private void OnDestroy()
	{
		
	}

	private void HandleOutofBound(ObjectMover mover)
	{
		mover.gameObject.transform.position = new Vector3(0.05f, _lastBornY + _middleHeight, 0f);
	}
}
