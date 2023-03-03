using System;

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

  Vector2 RelativeHeldVector { get; set; }
  float CrouchAmount { get; set; }

  void Pump();
}

[RequireComponent(typeof(Rigidbody))]
public class SkateMovementBehavior : MonoBehaviour, ISkateMovement {
  public GameObject body;

  private Rigidbody rigidbody_;

  private const float MAXIMUM_VELOCITY = 5f;
  private float pumpFraction_ = 0;

  public SkateMovementState MovementState { get; private set; } =
    SkateMovementState.STILL;

  public Vector2 RelativeHeldVector { get; set; }
  public float CrouchAmount { get; set; }

  public void Pump() { }

  // Start is called before the first frame update
  void Start() {
    this.rigidbody_ = GetComponent<Rigidbody>();
  }

  // Update is called once per frame
  void Update() {
    var bodyTransform = this.body.transform;

    var currentVelocity = this.rigidbody_.velocity;
    var currentSpeed =
        new Vector2(currentVelocity.x, currentVelocity.z).magnitude;
    var currentSpeedFrac = Math.Min(1, currentSpeed / MAXIMUM_VELOCITY);

    // Skating back and forth
    var addAmount = Time.deltaTime;
    var heldMagnitude = this.RelativeHeldVector.magnitude;
    if (heldMagnitude > .1f) {
      this.pumpFraction_ += heldMagnitude * addAmount;

      var heldAngle = Mathf.Atan2(
          -this.RelativeHeldVector.y,
          this.RelativeHeldVector.x);
      bodyTransform.localRotation = Quaternion.AngleAxis(Mathf.Rad2Deg * heldAngle, Vector3.up);
    } else {
      float target = this.pumpFraction_ switch {
          < .25f => 0,
          < .75f => .5f,
          _      => 1
      };
      this.pumpFraction_ += MathF.Sign(target - this.pumpFraction_) * addAmount;
    }
    this.pumpFraction_ %= 1;

    var facingAngle = bodyTransform.localEulerAngles.y * Mathf.Deg2Rad;
    var moveAmount = MathF.Sin(2 * this.pumpFraction_ * MathF.PI) * .5f * currentSpeedFrac;
    bodyTransform.localPosition =
        moveAmount * new Vector3(MathF.Sin(facingAngle), 0, MathF.Cos(facingAngle));

    // Movement
    if (this.MovementState.OnGround()) {
      var movementNormal =
          new Vector3(this.RelativeHeldVector.x, 0, this.RelativeHeldVector.y);
      var movementForce = 2 * movementNormal;

      this.rigidbody_.AddRelativeForce(movementForce);
    }

    // Crouching
    this.body.transform.localScale =
        new Vector3(1, .5f + .5f * (1 - this.CrouchAmount), 1);
  }
}