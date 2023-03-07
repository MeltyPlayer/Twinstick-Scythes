using UnityEngine;

public class MomentumBasedScytheBehavior : MonoBehaviour, IScytheBehavior {
  public GameObject body;
  public GameObject scythe;

  private Rigidbody rigidbody_;

  public bool IsEnabled => base.enabled;
  public Vector2 RelativeHeldVector { get; set; }
  public float AngularVelocity { get; set; }

  void Start() {
    this.rigidbody_ = GetComponent<Rigidbody>();
  }

  // Update is called once per frame
  void Update() {
    var scytheTransform = this.scythe.transform;
    var currentDegrees = scytheTransform.localRotation.eulerAngles.y;

    float dragAngleDegreesDelta = 0;
    float heldAngleDegreesDelta = 0;

    // Calculate force from drag
    var currentVelocity = this.rigidbody_.velocity.Xz();
    var currentSpeedFraction =
        currentVelocity.magnitude / SkateMovementBehavior.MAXIMUM_SPEED;
    if (currentSpeedFraction > .01f) {
      var dragAngleDegrees =
          Mathf.Atan2(-currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg +
          180;
      dragAngleDegreesDelta =
          Mathf.DeltaAngle(dragAngleDegrees, currentDegrees);
    }

    // Calculate force from stick
    var heldMagnitude = this.RelativeHeldVector.magnitude;
    var isStickHeld = heldMagnitude > .1f;
    if (isStickHeld) {
      var heldAngleDegrees = Mathf.Atan2(
          -this.RelativeHeldVector.y,
          this.RelativeHeldVector.x) * Mathf.Rad2Deg;
      heldAngleDegreesDelta =
          Mathf.DeltaAngle(heldAngleDegrees, currentDegrees);
    }

    // Apply forces to rotation
    var maxDragForce = .8f;
    var maxHeldForce = .5f;

    var currentSpeedFractionFactor = Mathf.Pow(currentSpeedFraction, .75f);
    var heldForceAmount = (1 - currentSpeedFractionFactor) * maxHeldForce;
    var dragForceAmount = currentSpeedFractionFactor * maxDragForce;

    this.AngularVelocity *= .99f;
    this.AngularVelocity += (-heldAngleDegreesDelta * heldForceAmount -
                            dragAngleDegreesDelta * dragForceAmount) * Time.deltaTime;
    scytheTransform.localRotation =
        Quaternion.AngleAxis(currentDegrees + this.AngularVelocity, Vector3.up);

    // Update facing angle based on side of body
    var bodyTransform = this.body.transform;
    var deltaAngle =
        Mathf.DeltaAngle(scytheTransform.eulerAngles.y,
                         bodyTransform.eulerAngles.y);
    scytheTransform.localScale =
        new Vector3(1, 1, Mathf.Sign(deltaAngle) > 0 ? 1 : -1);
  }
}