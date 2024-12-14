using UnityEngine;

namespace Assets.Scripts.EventBus
{
  public static class Events
  {
    #region Character
    public struct PlayerInitialized : IEvent
    {
      public int ID;
      public Transform Transform;
      public Transform CameraTarget;
    }
    #endregion
  }
}