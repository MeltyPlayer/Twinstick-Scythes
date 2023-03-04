using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(ScytheMovementBehavior))]
[RequireComponent(typeof(SkateMovementBehavior))]
public class PlayerControllerBehavior : MonoBehaviour {
  private PlayerInput playerInput_;
  private ScytheMovementBehavior scytheMovementBehavior_;
  private SkateMovementBehavior skateMovementBehavior_;

  // Start is called before the first frame update
  void Start() {
    this.playerInput_ = GetComponent<PlayerInput>();
    this.scytheMovementBehavior_ = GetComponent<ScytheMovementBehavior>();
    this.skateMovementBehavior_ = GetComponent<SkateMovementBehavior>();
  }

  // Update is called once per frame
  void Update() {
  }

  void OnMove(InputValue movementValue) {
    var leftStick = movementValue.Get<Vector2>();
    this.skateMovementBehavior_.RelativeHeldVector = leftStick;
  }

  void OnLook(InputValue movementValue) {
    var rightStick = movementValue.Get<Vector2>();
    this.scytheMovementBehavior_.RelativeHeldVector = rightStick;
  }

  void OnCrouch(InputValue movementValue) {
    var crouchAmount = movementValue.Get<float>();
    this.skateMovementBehavior_.CrouchAmount = crouchAmount;
  }
}