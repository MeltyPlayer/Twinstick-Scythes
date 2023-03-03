using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SkateMovementState {
  STILL,
  PUMPING,
  SKATING,
  GLIDING,
  IN_AIR,
  LANDING,
}

public static class SkateMovementStateExtensions {
  public static bool OnGround(this SkateMovementState state)
    => state is SkateMovementState.STILL
    or SkateMovementState.PUMPING
    or SkateMovementState.GLIDING
    or SkateMovementState.LANDING;

  public static bool InAir(this SkateMovementState state)
    => state is SkateMovementState.IN_AIR;
}

public interface ISkateMovement {
  SkateMovementState MovementState { get; }

  float HeldDirectionDegrees { get; set; }
  float CrouchAmount { get; set; }

  void Pump();
}

public class SkateMovementBehavior : MonoBehaviour, ISkateMovement {
  public SkateMovementState MovementState { get; private set; } = SkateMovementState.STILL;

  public float HeldDirectionDegrees { get; set; }
  public float CrouchAmount { get; set; }

  public void Pump() {

  }

  // Start is called before the first frame update
  void Start() {

  }

  // Update is called once per frame
  void Update() {

  }
}
