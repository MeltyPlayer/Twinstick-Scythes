using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
[RequireComponent(typeof(IScytheBehavior))]
[RequireComponent(typeof(IMovementBehavior))]
public class PlayerControllerBehavior : MonoBehaviour {
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
    this.movementBehavior_.RelativeHeldVector = leftStick;
  }

  void OnLook(InputValue movementValue) {
    var rightStick = movementValue.Get<Vector2>();
    this.scytheBehavior_.RelativeHeldVector = rightStick;
  }

  void OnCrouch(InputValue movementValue) {
    var dashAmount = movementValue.Get<float>();
    this.movementBehavior_.DashAmount = dashAmount;
  }
}