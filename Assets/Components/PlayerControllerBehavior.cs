using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(SkateMovementBehavior))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerControllerBehavior : MonoBehaviour {
  private SkateMovementBehavior skateMovementBehavior_;
  private PlayerInput playerInput_;

  // Start is called before the first frame update
  void Start() {
    this.skateMovementBehavior_ = GetComponent<SkateMovementBehavior>();
    this.playerInput_ = GetComponent<PlayerInput>();
  }

  // Update is called once per frame
  void Update() {
  }

  void OnMove(InputValue movementValue) {
    var leftStick = movementValue.Get<Vector2>();
    this.skateMovementBehavior_.RelativeHeldVector = leftStick;
  }
}