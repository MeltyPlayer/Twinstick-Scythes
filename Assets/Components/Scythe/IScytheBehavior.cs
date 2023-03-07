using UnityEngine;

public interface IScytheBehavior : IEnableableBehavior {
  Vector2 RelativeHeldVector { get; set; }
}