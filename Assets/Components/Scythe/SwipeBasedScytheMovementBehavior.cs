using System.Collections.Generic;

using UnityEngine;

public class SwipeBasedScytheMovementBehavior : MonoBehaviour, IScytheMovement {
  public GameObject scythe;
  private IReadOnlyList<MeshRenderer> meshRenderers_;
  private float previousHeldAngleDegrees_;

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
    var isStickHeld = heldMagnitude > .1f;

    var heldAngleDegrees = Mathf.Atan2(
        -this.RelativeHeldVector.y,
        this.RelativeHeldVector.x) * Mathf.Rad2Deg;

    Color scytheColor;
    if (isStickHeld) {
      scytheTransform.localRotation = Quaternion.AngleAxis(heldAngleDegrees, Vector3.up);

      var rawDeltaAngleDegrees =
          Mathf.DeltaAngle(this.previousHeldAngleDegrees_, heldAngleDegrees);

      var deltaAngleDegrees = rawDeltaAngleDegrees / Time.fixedDeltaTime;

      Debug.Log(deltaAngleDegrees);

      var maxDeltaAngle = 1000f;
      var deltaAngleFraction = Mathf.Abs(deltaAngleDegrees / maxDeltaAngle);
      if (deltaAngleDegrees > 0) {
        scytheColor = Color.red * deltaAngleFraction;
      } else {
        scytheColor = Color.green * deltaAngleFraction;
      }
    } else {
      scytheColor = Color.white;
    }

    foreach (var meshRenderer in this.meshRenderers_) {
      meshRenderer.material.SetColor("_Color", scytheColor);
    }

    this.previousHeldAngleDegrees_ = heldAngleDegrees;
  }
}