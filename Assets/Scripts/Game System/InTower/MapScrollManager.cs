using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class MapScrollManager : MonoBehaviour, IInitializable
{
	[SerializeField] private AssetReferenceGameObject[] mapScrollPrefs;

	//private float _lastBornY;
	//private float _middleHeight;

	private float _totalLoopHeight;

	public void Init()
	{
		StartCoroutine(InitRoutine());
	}

	private IEnumerator InitRoutine()
	{
		yield return StartCoroutine(MapInitRoutine());
		// 준비 완료 시그널 전송
		GameBus.Publish(new SubsystemReady(SubsystemId.Map));
	}

	private IEnumerator MapInitRoutine()
	{
		// 이미지 생성 기준점 정립
		float newBottomY = ScreenBounds.LowerY;

		float totalHeight = 0f;
		// 리스트 길이만큼 반복 시행
		for (int i = 0; i< mapScrollPrefs.Length; i++)
		{
			var handle = Addressables.InstantiateAsync(mapScrollPrefs[i]);
			yield return handle;

			if(handle.Status != AsyncOperationStatus.Succeeded)
			{
				Debug.LogError($"[Map Scroll Manager] Map Scroll Prefs Instantiate Failed : {i}th");
				GameBus.Publish(new SubsystemFailed(SubsystemId.Map, $"{handle.Status}"));
				yield break;
			}

			// 이미지 생성 및 위치 정립
			GameObject go = handle.Result;

			Renderer rd = go.GetComponent<Renderer>();
			if (!rd) Debug.LogError("Map Scroll Prefs has No Renderer");

			//int height = Mathf.FloorToInt(rd.bounds.size.y);
			float height = rd.bounds.size.y;
			totalHeight += height;
			float newY = newBottomY + (height * 0.5f);

			//_middleHeight = (height * 0.5f);
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

		_totalLoopHeight = totalHeight;
		// OutofBound의 고정 기준점
		//_lastBornY = newBottomY;

		yield return null;
	}

	private void HandleOutofBound(ObjectMover mover)
	{
		Vector3 currentPos = mover.gameObject.transform.position;
		currentPos.y += _totalLoopHeight;
		//mover.gameObject.transform.position = new Vector3(0.05f, (int)(_lastBornY + _middleHeight), 0f);
		mover.gameObject.transform.position = new Vector3(0.05f, currentPos.y, 0f);
	}
}
