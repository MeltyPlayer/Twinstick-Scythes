using UnityEngine;

public interface IScytheMovement {
  bool IsEnabled { get; }
  Vector2 RelativeHeldVector { get; set; }
}