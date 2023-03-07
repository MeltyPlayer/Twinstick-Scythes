using System;

using UnityEngine;


public class BasicCameraController : MonoBehaviour {
  public GameObject focus;
  public float distance = 10;

  private IMovementBehavior movementBehavior_;

  void Start() {
    this.movementBehavior_ =
        this.focus.GetFirstEnabledComponent<IMovementBehavior>();
  }

  void FixedUpdate() {
    var cameraTransform = this.transform;
    var cameraAngleDegrees = this.transform.eulerAngles.y;

    var focusTransform = this.focus.transform;
    var relativeHeldVector = this.movementBehavior_.RelativeHeldVector;
    if (relativeHeldVector.magnitude > Constants.DEAD_ZONE) {
      var velocityDegrees =
          Mathf.Atan2(relativeHeldVector.x, relativeHeldVector.y) * Mathf.Rad2Deg;

      var deltaAngle = Mathf.DeltaAngle(cameraAngleDegrees, velocityDegrees);
      var absDeltaAngle = Mathf.Abs(deltaAngle);

      float angleChange;
      if (absDeltaAngle < 90) {
        angleChange = deltaAngle;
      } else {
        angleChange = Math.Sign(deltaAngle) * (90 - (Math.Abs(deltaAngle) - 90));
      }

      cameraAngleDegrees += angleChange * Time.fixedDeltaTime;
    }

    var cameraAngleRadians = cameraAngleDegrees * Mathf.Deg2Rad;
    var cameraAngleVector = new Vector3(MathF.Sin(cameraAngleRadians),
                                        0,
                                        MathF.Cos(cameraAngleRadians));
    var cameraOffsetVector = new Vector3(0, 6, 0);

    cameraTransform.eulerAngles = new Vector3(30, cameraAngleDegrees, 0);
    cameraTransform.position = focusTransform.position -
        this.distance * cameraAngleVector + cameraOffsetVector;
  }
}