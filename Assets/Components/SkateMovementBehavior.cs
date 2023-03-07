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
[RequireComponent(typeof(TrailRenderer))]
public class SkateMovementBehavior : MonoBehaviour, ISkateMovement {
  public GameObject body;

  private Rigidbody rigidbody_;
  private TrailRenderer trailRenderer_;

  public const float MAXIMUM_SPEED = 35f; // 25
  private const float PUMP_SPEED_MULTIPLIER = 2f; // 1.5
  private float pumpFraction_ = 0;
  private float heldAngleDegrees_ = 0;
  private readonly StateDebouncer<bool> emittingDebouncer_ = new(false, .5f);

  public SkateMovementState MovementState { get; private set; } =
    SkateMovementState.STILL;

  public Vector2 RelativeHeldVector { get; set; }
  public float CrouchAmount { get; set; }

  public void Pump() { }

  // Start is called before the first frame update
  void Start() {
    this.rigidbody_ = GetComponent<Rigidbody>();
    this.trailRenderer_ = GetComponent<TrailRenderer>();
  }

  // Update is called once per frame
  void FixedUpdate() {
    var bodyTransform = this.body.transform;

    var currentXzVelocity = this.rigidbody_.velocity.Xz();
    var currentSpeed = currentXzVelocity.magnitude;
    var currentSpeedFrac = Math.Min(1, currentSpeed / MAXIMUM_SPEED);

    //Debug.Log($"Current Speed: {currentSpeed} / {MAXIMUM_VELOCITY}");

    var heldMagnitude = this.RelativeHeldVector.magnitude;
    var isStickHeld = heldMagnitude > .3f;
    if (isStickHeld) {
      this.heldAngleDegrees_ = Mathf.Atan2(
          -this.RelativeHeldVector.y,
          this.RelativeHeldVector.x) * Mathf.Rad2Deg;
    }

    // Movement
    var enableEmitter = false;
    if (this.MovementState.OnGround()) {
      var normalizedVelocity = currentXzVelocity.normalized;
      var normalizedHeldAngle = this.RelativeHeldVector.normalized;

      var dot = Vector2.Dot(normalizedVelocity, normalizedHeldAngle);
      var facingTowardFraction = (1 + dot) / 2;

      var isFacingForward = facingTowardFraction > .75f;

      // Skating back and forth animation
      {
        var addAmount = Mathf.Lerp(.05f, 1, currentSpeedFrac) * facingTowardFraction * Time.fixedDeltaTime * PUMP_SPEED_MULTIPLIER;
        float activePumpingFraction;
        if (isStickHeld && isFacingForward) {
          this.pumpFraction_ += heldMagnitude * addAmount;
          activePumpingFraction = 1;
        } else if (!isStickHeld) {
          float target = this.pumpFraction_ switch {
              < .25f => 0,
              < .75f => .5f,
              _      => 1
          };
          this.pumpFraction_ += MathF.Sign(target - this.pumpFraction_) * addAmount;
          activePumpingFraction = MathF.Abs(target - this.pumpFraction_) / .25f;
        } else {
          this.pumpFraction_ = 0;
          activePumpingFraction = 0;
        }
        this.pumpFraction_ %= 1;

        var facingAngleDegrees = bodyTransform.localEulerAngles.y;
        var facingAngleRadians = facingAngleDegrees * Mathf.Deg2Rad;
        var facingAngleSin = MathF.Sin(facingAngleRadians);
        var facingAngleCos = MathF.Cos(facingAngleRadians);

        var pumpAngleRadians = 2 * this.pumpFraction_ * MathF.PI;

        var sideToSideMoveAmount = MathF.Sin(pumpAngleRadians) * .5f * currentSpeedFrac;
        var forwardAndBackMoveAmount = MathF.Cos(2 * pumpAngleRadians) * .25f * currentSpeedFrac;
        bodyTransform.localPosition =
            sideToSideMoveAmount *
            new Vector3(facingAngleSin, 0, facingAngleCos) +
            forwardAndBackMoveAmount *
            new Vector3(facingAngleCos, 0, facingAngleSin);

        // Updating angle
        var backAndForthFraction = activePumpingFraction * Mathf.Cos(pumpAngleRadians) * currentSpeedFrac;

        float yawDegrees, rollDegrees, pitchDegrees;
        Quaternion yaw, roll, pitch;
        if (isFacingForward || !isStickHeld) {
          yawDegrees = this.heldAngleDegrees_ - backAndForthFraction * 20;
          rollDegrees = backAndForthFraction * 20;
          pitchDegrees =
              -Mathf.Lerp(40, 60, this.CrouchAmount) * currentSpeedFrac +
              forwardAndBackMoveAmount * 15;
        } else {
          var velocityAngleDegrees =
              Mathf.Atan2(-normalizedVelocity.y, normalizedVelocity.x) *
              Mathf.Rad2Deg;
          var deltaAngle = Mathf.DeltaAngle(velocityAngleDegrees, this.heldAngleDegrees_);

          Debug.Log($"Velocity angle: {velocityAngleDegrees}, Held angle: {this.heldAngleDegrees_}");

          yawDegrees = this.heldAngleDegrees_;
          rollDegrees = -deltaAngle / 2 * currentSpeedFrac;
          pitchDegrees = -Mathf.Lerp(60, 75, this.CrouchAmount) * currentSpeedFrac;
        }
        yaw = Quaternion.AngleAxis(yawDegrees, Vector3.up);
        roll = Quaternion.AngleAxis(rollDegrees, Vector3.right);
        pitch = Quaternion.AngleAxis(pitchDegrees, Vector3.forward);
        var newRotation = yaw * roll * pitch;

        bodyTransform.localRotation = Quaternion.Lerp(bodyTransform.localRotation, newRotation, .5f);
      }

      // Movement force
      {
        var movementNormal =
            new Vector3(this.RelativeHeldVector.x, 0, this.RelativeHeldVector.y);

        var maxMovementForce = Mathf.Lerp(3, 5, this.CrouchAmount) *
            Physics.gravity.magnitude / 9.81f;
        var movementForce = maxMovementForce * movementNormal *
                            (1 - MathF.Pow(
                                facingTowardFraction * currentSpeedFrac,
                                .25f));

        this.rigidbody_.AddRelativeForce(
            movementForce * Time.fixedDeltaTime *
            Mathf.Lerp(10, 15, this.CrouchAmount),
            ForceMode.VelocityChange);
      }

      if (currentSpeedFrac > .25f && isStickHeld && !isFacingForward) {
        enableEmitter = true;
      }
    }

    var currentEmittingValue = this.emittingDebouncer_.Value;
    if (enableEmitter != currentEmittingValue) {
      this.emittingDebouncer_.Value = currentEmittingValue = enableEmitter;
    }
    this.trailRenderer_.emitting = currentEmittingValue;

    this.rigidbody_.velocity -= this.rigidbody_.velocity * .5f * Time.fixedDeltaTime;
  }
}