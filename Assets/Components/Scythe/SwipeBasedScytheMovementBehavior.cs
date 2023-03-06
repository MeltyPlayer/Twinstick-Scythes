using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class SwipeBasedScytheMovementBehavior : MonoBehaviour, IScytheMovement {
  public GameObject body;
  public GameObject scythe;

  private IReadOnlyList<MeshRenderer> meshRenderers_;
  private float previousHeldAngleDegrees_;
  private float scytheVelocity_;
  private Color scytheColor_ = Color.white;

  private const int NUM_FRAMES_TO_AVERAGE = 5;
  private readonly LinkedList<float> pastRawHeldAngleDeltas_ = new();
  private readonly LinkedList<float> pastHeldAngleDeltas_ = new();

  private readonly StateDebouncer<bool> activelySwinging_ = new(false, .3f);

  public bool IsEnabled => base.enabled;
  public Vector2 RelativeHeldVector { get; set; }

  void Start() {
    this.meshRenderers_ = this.scythe.GetComponentsInChildren<MeshRenderer>();
  }

  // Update is called once per frame
  void FixedUpdate() {
    var scytheTransform = this.scythe.transform;
    var currentDegrees = scytheTransform.localRotation.eulerAngles.y;

    // Calculate force from stick
    var heldMagnitude = this.RelativeHeldVector.magnitude;
    var stickDeadzone = .5f;
    var isStickHeld = heldMagnitude > stickDeadzone;
    var hasVelocity = Math.Abs(this.scytheVelocity_) > .01f;

    var heldAngleDegrees = Mathf.Atan2(
        -this.RelativeHeldVector.y,
        this.RelativeHeldVector.x) * Mathf.Rad2Deg;

    var rawDeltaAngleDegrees = 0f;
    var deltaAngleDegrees = 0f;
    if (isStickHeld) {
      rawDeltaAngleDegrees =
          Mathf.DeltaAngle(this.previousHeldAngleDegrees_, heldAngleDegrees);
      deltaAngleDegrees = rawDeltaAngleDegrees / Time.fixedDeltaTime;
    }

    var totalArcDegrees =
        Mathf.Abs(rawDeltaAngleDegrees + this.pastRawHeldAngleDeltas_.Sum());
    var averageDeltaAngle =
        (deltaAngleDegrees + this.pastHeldAngleDeltas_.Sum()) /
        (1 + NUM_FRAMES_TO_AVERAGE);
    var maxDeltaAngle = 500f;
    var deltaAngleFraction = Mathf.Abs(averageDeltaAngle / maxDeltaAngle);

    if (isStickHeld && deltaAngleFraction >= 1f && totalArcDegrees > 45) {
      this.activelySwinging_.SetForced(true);
    } else {
      this.activelySwinging_.SetDebounced(false);
    }
    var isSwinging = this.activelySwinging_.Value;

    if (isStickHeld && isSwinging) {
      this.scytheVelocity_ = MaxMagnitude_(this.scytheVelocity_, rawDeltaAngleDegrees);
    }

    float scytheAngleDegrees;
    if (!hasVelocity) {
      scytheAngleDegrees = isStickHeld ? heldAngleDegrees : currentDegrees;
    } else {
      scytheAngleDegrees = currentDegrees + this.scytheVelocity_;
    }
    this.scytheVelocity_ *= .7f;
    scytheTransform.localRotation =
        Quaternion.AngleAxis(scytheAngleDegrees, Vector3.up);

    if (isSwinging || hasVelocity) {
      var isSwingingPositively = this.scytheVelocity_ > 0;
      this.scytheColor_ = isSwingingPositively ? Color.red : Color.green;
     
      scytheTransform.localScale =
          new Vector3(1, 1, isSwingingPositively ? 1 : -1);
    } else {
      this.scytheColor_ = Color.white;

      // Update facing angle based on side of body
      var bodyTransform = this.body.transform;
      var deltaAngle =
          Mathf.DeltaAngle(scytheTransform.eulerAngles.y,
                           bodyTransform.eulerAngles.y);
      scytheTransform.localScale =
          new Vector3(1, 1, Mathf.Sign(deltaAngle) > 0 ? 1 : -1);
    }

    foreach (var meshRenderer in this.meshRenderers_) {
      meshRenderer.material.SetColor("_Color", this.scytheColor_);
    }

    this.previousHeldAngleDegrees_ = heldAngleDegrees;

    this.pastRawHeldAngleDeltas_.AddFirst(rawDeltaAngleDegrees);
    this.pastHeldAngleDeltas_.AddFirst(deltaAngleDegrees);
    if (this.pastHeldAngleDeltas_.Count >= NUM_FRAMES_TO_AVERAGE) {
      this.pastRawHeldAngleDeltas_.RemoveLast();
      this.pastHeldAngleDeltas_.RemoveLast();
    }
  }

  private float MaxMagnitude_(params float[] values)
    => values.Aggregate(this.MaxMagnitude_);

  private float MaxMagnitude_(float a, float b)
    => Math.Abs(a) > Math.Abs(b) ? a : b;
}