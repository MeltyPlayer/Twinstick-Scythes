using UnityEngine;

[RequireComponent(typeof(SkateMovementBehavior))]
public class EnemyControllerBehavior : MonoBehaviour {
  public GameObject player;
  private SkateMovementBehavior skateMovementBehavior_;

  // Start is called before the first frame update
  void Start() {
    this.skateMovementBehavior_ = GetComponent<SkateMovementBehavior>();
  }

  void FixedUpdate() {
    var currentPosition = this.transform.position;
    var target = this.player.transform.position;

    this.skateMovementBehavior_.RelativeHeldVector = (target - currentPosition).normalized.Xz();
  }
}