using System;

using UnityEngine;


public class BasicCameraController : MonoBehaviour {
  public GameObject focus;
  public float distance = 10;
  private Vector3 previousFocusPosition_;

  void FixedUpdate() {
    var cameraTransform = this.transform;

    var cameraAngleDegrees = this.transform.eulerAngles.y;

    var focusTransform = this.focus.transform;
    var focusPosition = focusTransform.position;
    if (this.previousFocusPosition_ != null) {
      var velocity = (focusPosition - this.previousFocusPosition_).Xz();
      var velocityDegrees =
          Mathf.Atan2(-velocity.y, velocity.x) * Mathf.Rad2Deg;

      var deltaAngle = Mathf.DeltaAngle(cameraAngleDegrees, velocityDegrees);
    }

    cameraAngleDegrees += .1f;

    var cameraAngleRadians = cameraAngleDegrees * Mathf.Deg2Rad;
    var cameraAngleVector = new Vector3(MathF.Sin(cameraAngleRadians),
                                        0,
                                        MathF.Cos(cameraAngleRadians));
    var cameraOffsetVector = new Vector3(0, 6,0);
    
    cameraTransform.eulerAngles = new Vector3(30, cameraAngleDegrees, 0);
    cameraTransform.position = focusTransform.position -
        this.distance * cameraAngleVector + cameraOffsetVector;
    
    this.previousFocusPosition_ = focusPosition;
  }
}