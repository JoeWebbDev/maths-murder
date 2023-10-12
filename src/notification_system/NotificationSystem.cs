using Godot;
using System;

public partial class NotificationSystem : Node
{
    [Signal] public delegate void DamageTakenEventHandler(Fighter fighter, int amount);
    [Signal] public delegate void DamageDealtEventHandler(Fighter fighter, int amount);

    public void Notify(StringName signalName, params Variant[] args)
    {
        EmitSignal(signalName, args);
    }
    // private readonly Dictionary<GameEvent, Action<Dictionary<string, object>>> _eventDictionary 
    //     = new Dictionary<GameEvent, Action<Dictionary<string, object>>>();
    //
    // public void AddListener(GameEvent e, Action<Dictionary<string, object>> listener)
    // {
    //     if (_eventDictionary.TryGetValue(e, out Action<Dictionary<string, object>> thisEvent))
    //     {
    //         thisEvent += listener;
    //         _eventDictionary[e] = thisEvent;
    //     }
    //     else
    //     {
    //         thisEvent += listener;
    //         _eventDictionary.Add(e, thisEvent);
    //     }
    // }
    //
    // public void RemoveListener(GameEvent e, Action<Dictionary<string, object>> listener)
    // {
    //     if (_eventDictionary.TryGetValue(e, out Action<Dictionary<string, object>> thisEvent))
    //     {
    //         thisEvent -= listener;
    //         _eventDictionary[e] = thisEvent;
    //     }
    //     else
    //     {
    //         Debug.LogWarning($"Attempting to remove listener from a null event. Event: {e} - Listener: {listener.Method}");
    //     }
    // }
    //
    // public void TriggerEvent(GameEvent e, Dictionary<string, object> message)
    // {
    //     if (_eventDictionary.TryGetValue(e, out Action<Dictionary<string, object>> thisEvent))
    //     {
    //         thisEvent?.Invoke(message);
    //     }
    // }
}
