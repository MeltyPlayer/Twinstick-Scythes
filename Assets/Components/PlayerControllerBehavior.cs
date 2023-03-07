using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(IScytheBehavior))]
[RequireComponent(typeof(IMovementBehavior))]
public class PlayerControllerBehavior : MonoBehaviour {
  public GameObject camera;

  private PlayerInput playerInput_;
  private IScytheBehavior scytheBehavior_;
  private IMovementBehavior movementBehavior_;

  // Start is called before the first frame update
  void Start() {
    this.playerInput_ = GetComponent<PlayerInput>();
    this.scytheBehavior_ = this.GetFirstEnabledComponent<IScytheBehavior>();
    this.movementBehavior_ = this.GetFirstEnabledComponent<IMovementBehavior>();
  }

  // Update is called once per frame
  void Update() {
  }

  void OnMove(InputValue movementValue) {
    var leftStick = movementValue.Get<Vector2>();
    this.movementBehavior_.RelativeHeldVector = this.GetVectorRelativeToCamera_(leftStick);
  }

  void OnLook(InputValue movementValue) {
    var rightStick = movementValue.Get<Vector2>();
    this.scytheBehavior_.RelativeHeldVector = this.GetVectorRelativeToCamera_(rightStick);
  }

  void OnCrouch(InputValue movementValue) {
    var dashAmount = movementValue.Get<float>();
    this.movementBehavior_.DashAmount = dashAmount;
  }

  private Vector2 GetVectorRelativeToCamera_(Vector2 angle) {
    var magnitude = angle.magnitude;
    var angleRadians = Mathf.Atan2(-angle.y, angle.x);
    var cameraRadians = this.camera.transform.eulerAngles.y * Mathf.Deg2Rad;

    var relativeAngleRadians = cameraRadians - Mathf.PI / 2 + angleRadians;
    return magnitude * new Vector2(-Mathf.Sin(relativeAngleRadians), -Mathf.Cos(relativeAngleRadians));
  }
}