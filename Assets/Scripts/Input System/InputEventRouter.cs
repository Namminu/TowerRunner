using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(GraphicRaycaster))]
public class InputEventRouter : MonoBehaviour
{
	private GraphicRaycaster uiRaycaster;
	private PointerEventData pointerData;
	private EventSystem		 eventSystem;

	private void Awake()
	{
		uiRaycaster = GetComponent<GraphicRaycaster>();
		eventSystem = EventSystem.current;
		pointerData = new PointerEventData(eventSystem);

		TouchProcessor.Instance.OnTap += HandleTap;
		TouchProcessor.Instance.OnDrag += HandleDrag;
		TouchProcessor.Instance.OnDragEnd += HandleDragEnd;
	}

	private bool IsPointerOverUI(Vector2 screenPos)
	{
		pointerData.position = screenPos;
		var results = new List<RaycastResult>();
		uiRaycaster.Raycast(pointerData, results);		
		return results.Count > 0;
	}

	private void HandleTap(int id, Vector2 pos)
	{
		if (IsPointerOverUI(pos)) return;
		if (Player.Instance == null) return;

		// UI 터치 외 다른 터치 로직 - 플레이어 공격
		Player.Instance.OnTap(pos);
	}

	private void HandleDrag(int id, Vector2 delta, Vector2 pos)
	{
		if (IsPointerOverUI(pos)) return;
		if (Player.Instance == null) return;

		Player.Instance.Mover.OnDrag(delta);
	}
	private void HandleDragEnd(int id, Vector2 pos)
	{
		if (IsPointerOverUI(pos)) return;
		if (Player.Instance == null) return;

		Player.Instance.Mover.OnDragEnd(pos);
	}
}
