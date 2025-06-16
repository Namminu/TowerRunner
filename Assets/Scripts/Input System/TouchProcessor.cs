using System;
using System.Collections.Generic;
using UnityEngine;

public class TouchProcessor
{
    public static TouchProcessor Instance { get; } = new TouchProcessor();

    private class TouchData
    {
        public Vector2 startPos;
        public float startTime;
        public bool isDragging;
    }
    private Dictionary<int, TouchData> touches = new Dictionary<int, TouchData>();

    public event Action<int, Vector2> OnTap;                // fingerId, pos
    public event Action<int, Vector2, Vector2> OnDrag;      // fingerId, delta, pos
	public event Action<int, Vector2> OnDragEnd;            // fingerId, pos

    private const float tapMaxTime = 0.2f;
    private const float dragThreshold = 20f;

    public void ProcessTouchBegan(int id, Vector2 pos)
    {
        touches[id] = new TouchData { startPos = pos, startTime = Time.time, isDragging = false };
    }

    public void ProcessTouchMoved(int id, Vector2 pos)
    {
        if(!touches.ContainsKey(id)) return;
        var data = touches[id];
        var dist = Vector2.Distance(data.startPos, pos);

        if (!data.isDragging && dist > dragThreshold)
            data.isDragging = true;

        if(data.isDragging)
        {
            Vector2 delta = pos - data.startPos;
            OnDrag?.Invoke(id, delta, pos);
            data.startPos = pos;
        }
    }

    public void ProcessTouchEnded(int id, Vector2 pos)
    {
        if (!touches.ContainsKey(id)) return;
        var data = touches[id];
        float duration = Time.time - data.startTime;
        var dist = Vector2.Distance(data.startPos, pos);

        if (!data.isDragging && duration <= tapMaxTime && dist <= dragThreshold)
            OnTap?.Invoke(id, pos);             // 탭 판정
        else if (data.isDragging)
            OnDragEnd?.Invoke(id, pos);         // 드래그 판정

        touches.Remove(id);
	}
}
