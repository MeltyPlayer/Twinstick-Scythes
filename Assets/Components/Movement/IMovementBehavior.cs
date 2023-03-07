using UnityEngine;

public interface IMovementBehavior : IEnableableBehavior {
  Vector2 RelativeHeldVector { get; set; }
  float DashAmount { get; set; }
}
