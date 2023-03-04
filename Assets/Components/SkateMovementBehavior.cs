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

  private const float MAXIMUM_VELOCITY = 20f;
  private const float PUMP_SPEED_MULTIPLIER = 1.5f;
  private float pumpFraction_ = 0;
  private float heldAngleDegrees_ = 0;

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

    var currentXzVelocity = this.rigidbody_.velocity.Xz();
    var currentSpeed = currentXzVelocity.magnitude;
    var currentSpeedFrac = Math.Min(1, currentSpeed / MAXIMUM_VELOCITY);

    //Debug.Log($"Current Speed: {currentSpeed} / {MAXIMUM_VELOCITY}");

    var heldMagnitude = this.RelativeHeldVector.magnitude;
    var isStickHeld = heldMagnitude > .1f;
    if (isStickHeld) {
      this.heldAngleDegrees_ = Mathf.Atan2(
          -this.RelativeHeldVector.y,
          this.RelativeHeldVector.x) * Mathf.Rad2Deg;
    }

    // Movement
    if (this.MovementState.OnGround()) {
      var normalizedVelocity = currentXzVelocity.normalized;
      var normalizedHeldAngle = this.RelativeHeldVector.normalized;

      var dot = Vector2.Dot(normalizedVelocity, normalizedHeldAngle);
      var facingTowardFraction = (1 + dot) / 2;

      var isFacingForward = facingTowardFraction > .75f;

      // Skating back and forth animation
      {
        var addAmount = Mathf.Lerp(.2f, 1, currentSpeedFrac) * facingTowardFraction * Time.deltaTime * PUMP_SPEED_MULTIPLIER;
        if (heldMagnitude > .1f && isFacingForward) {
          this.pumpFraction_ += heldMagnitude * addAmount;
          this.heldAngleDegrees_ = Mathf.Atan2(
              -this.RelativeHeldVector.y,
              this.RelativeHeldVector.x) * Mathf.Rad2Deg;
        } else {
          float target = this.pumpFraction_ switch {
              < .25f => 0,
              < .75f => .5f,
              _      => 1
          };
          this.pumpFraction_ += MathF.Sign(target - this.pumpFraction_) * addAmount;
        }
        this.pumpFraction_ %= 1;

        var facingAngleRadians = bodyTransform.localEulerAngles.y * Mathf.Deg2Rad;
        var facingAngleSin = MathF.Sin(facingAngleRadians);
        var facingAngleCos = MathF.Cos(facingAngleRadians);

        var pumpAngleRadians = 2 * this.pumpFraction_ * MathF.PI;

        var moveAmount = MathF.Sin(pumpAngleRadians) * .5f * currentSpeedFrac;
        bodyTransform.localPosition =
            moveAmount * new Vector3(facingAngleSin,
                                     0,
                                     facingAngleCos);

        // Updating angle
        var backAndForthFraction = Mathf.Cos(pumpAngleRadians) * currentSpeedFrac;

        var yaw =
            Quaternion.AngleAxis(
                this.heldAngleDegrees_ - backAndForthFraction * 20,
                Vector3.up);
        var roll = Quaternion.AngleAxis(backAndForthFraction * 20, Vector3.right);
        var pitch = Quaternion.AngleAxis(-20 * currentSpeedFrac, Vector3.forward);
        bodyTransform.localRotation = yaw * roll * pitch;
      }

      // Movement force
      {
        var movementNormal =
            new Vector3(this.RelativeHeldVector.x, 0, this.RelativeHeldVector.y);
        var movementForce = 3 * movementNormal *
                            (1 - MathF.Pow(facingTowardFraction * currentSpeedFrac, .5f));

        this.rigidbody_.AddRelativeForce(movementForce);
      }
    }

    // Crouching
    this.body.transform.localScale =
        new Vector3(1, .5f + .5f * (1 - this.CrouchAmount), 1);
  }
}