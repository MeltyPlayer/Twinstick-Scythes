using UnityEngine;

[RequireComponent(typeof(IMovementBehavior))]
public class EnemyControllerBehavior : MonoBehaviour {
  public GameObject player;
  private IMovementBehavior movementBehavior_;

  // Start is called before the first frame update
  void Start() {
    this.movementBehavior_ =
        this.GetFirstEnabledComponent<IMovementBehavior>();
  }

  void FixedUpdate() {
    var currentPosition = this.transform.position;
    var target = this.player.transform.position;

    this.movementBehavior_.RelativeHeldVector =
        (target - currentPosition).normalized.Xz();
  }
}