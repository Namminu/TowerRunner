using System;
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
    public static void Publish(IGameSignal sig) => OnSignal?.Invoke(sig);

    public static void Subscribe<T>(Action<T> handler) where T : IGameSignal
    {
        OnSignal += (sig) => { if (sig is T t) handler(t); };
    }

	public static void Unsubscribe<T>(Action<T> handler) where T : IGameSignal
	{
		OnSignal -= (sig) => { if (sig is T t) handler(t); };
	}
}
