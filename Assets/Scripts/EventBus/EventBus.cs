using System.Collections.Generic;

namespace Assets.Scripts.EventBus
{
  public static class EventBus<T> where T : IEvent
  {
    private static readonly HashSet<IEventBinding<T>> bindings = new();

    public static void Register(EventBinding<T> binding) => bindings.Add(binding);
    public static void Unregister(EventBinding<T> binding) => bindings.Remove(binding);

    public static void Raise(T @event) {
      var currentBindings = new List<IEventBinding<T>>(bindings);

      foreach (var binding in currentBindings) {
        binding.OnEvent.Invoke(@event);
        binding.OnEventNoArgs.Invoke();
      }
    }

    public static void Clear() {
      bindings.Clear();
    }
  }
}