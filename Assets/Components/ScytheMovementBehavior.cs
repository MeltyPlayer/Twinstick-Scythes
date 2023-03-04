using UnityEngine;

public interface IScytheMovement {
  Vector2 RelativeHeldVector { get; set; }
}

public class ScytheMovementBehavior : MonoBehaviour, IScytheMovement {
  public GameObject body;

  public Vector2 RelativeHeldVector { get; set; }

  // Update is called once per frame
  void Update() {
    var bodyTransform = this.body.transform;

    var heldMagnitude = this.RelativeHeldVector.magnitude;
    var isStickHeld = heldMagnitude > .1f;
    if (isStickHeld) {
      var heldAngleDegrees = Mathf.Atan2(
          -this.RelativeHeldVector.y,
          this.RelativeHeldVector.x) * Mathf.Rad2Deg;
      bodyTransform.localRotation =
          Quaternion.AngleAxis(heldAngleDegrees, Vector3.up);
    }
  }
}