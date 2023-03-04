using UnityEngine;

public static class VectorExtensions {
  public static Vector2 Xy(this Vector3 v) => new(v.x, v.y);
  public static Vector2 Xz(this Vector3 v) => new(v.x, v.z);
}