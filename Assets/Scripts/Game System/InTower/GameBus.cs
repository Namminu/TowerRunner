using System;
using System.Collections.Generic;
using UnityEngine;

public interface IGameSignal
{ }

public readonly struct SubsystemReady : IGameSignal
{
    public readonly SubsystemId Id;
    public SubsystemReady(SubsystemId id) => Id = id;
}

public readonly struct SubsystemFailed : IGameSignal
{
    public readonly SubsystemId Id;
    public readonly string Reason;
    public SubsystemFailed(SubsystemId id, string reason)
    {
        Id = id;
        Reason = reason;
    }
}

public readonly struct RunSignal : IGameSignal
{ }

public readonly struct ActivateMovementNextFrame : IGameSignal
{ }

public struct StateUIReady : IGameSignal
{
	public TowerStateTextUI TextUI;
	public StateUIReady(TowerStateTextUI ui) { TextUI = ui; }
}

public static class GameBus
{
    public static event Action<IGameSignal> OnSignal;
    private static readonly Dictionary<Type, IGameSignal> _last = new();

    public static void Publish(IGameSignal sig)
    {
        OnSignal?.Invoke(sig);
        _last[sig.GetType()] = sig;
    }

    public static Action Subscribe<T>(Action<T> handler) where T : IGameSignal
    {
        Action<IGameSignal> scribeToken = (sig) => { if (sig is T t) handler(t); };
        OnSignal += scribeToken;
        return () => OnSignal -= scribeToken;
    }

	/// <summary>
	/// Subscription method for objects created after the game starts
	/// </summary>
	public static Action SubscribeSticky<T>(Action<T> handler) where T : IGameSignal
    {
		Action unsub = Subscribe(handler);
		if (_last.TryGetValue(typeof(T), out var sig))
			handler((T)sig);
		return unsub;
	}
}
