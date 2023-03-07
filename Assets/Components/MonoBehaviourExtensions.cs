using System.Linq;

using UnityEngine;


public static class MonoBehaviourExtensions {
  public static T GetFirstEnabledComponent<T>(this MonoBehaviour behavior)
      where T : IEnableableBehavior
    => behavior.GetComponents<T>()
               .First(component => component.IsEnabled);
}