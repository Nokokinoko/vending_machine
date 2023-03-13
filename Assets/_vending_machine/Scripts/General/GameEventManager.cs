using System;
using System.Linq;
using UniRx;

public enum GameEvent
{
    GameStart,
    GameDead,
    GameTimeUp,
}

public static class GameEventManager
{
    public static void Notify(GameEvent gameEvent)
    {
        MessageBroker.Default.Publish(gameEvent);
    }
        
    public static void Notify<T>(GameEvent gameEvent, T value)
    {
        MessageBroker.Default.Publish((gameEvent, (object)value));
    }

    public static IObservable<Unit> OnReceivedAsObservable(GameEvent gameEvent)
    {
        return MessageBroker.Default.Receive<GameEvent>()
            .Where(receivedEvent => receivedEvent == gameEvent)
            .AsUnitObservable();
    }

    public static IObservable<Unit> OnReceivedAsObservable(params GameEvent[] gameEvents)
    {
        return MessageBroker.Default.Receive<GameEvent>()
            .Where(receivedEvent => gameEvents.Any(gameEvent => gameEvent == receivedEvent))
            .AsUnitObservable();
    }

    public static IObservable<T> OnReceivedAsObservable<T>(GameEvent gameEvent)
    {
        return MessageBroker.Default.Receive<(GameEvent type, object value)>()
            .Where(data => data.type == gameEvent)
            .Where(data => data.value is T)
            .Select(data => (T)data.value);
    }
}
