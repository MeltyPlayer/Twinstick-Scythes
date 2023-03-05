using System;


public class StateDebouncer<T> {
  private T value_;
  private DateTime lastUpdateTime_;
  private double secondsBetweenUpdate_;

  public StateDebouncer(T initialValue, double secondsBetweenUpdate) {
    this.value_ = initialValue;
    this.secondsBetweenUpdate_ = secondsBetweenUpdate;
  }

  public T Value {
    get => this.value_;
    set {
      var currentTime = this.GetCurrentTime_();
      if ((currentTime - this.lastUpdateTime_).TotalSeconds >=
          this.secondsBetweenUpdate_ || value.Equals(this.value_)) {
        this.value_ = value;
        this.lastUpdateTime_ = currentTime;
      }
    }
  }

  private DateTime GetCurrentTime_() => DateTime.UtcNow;
}