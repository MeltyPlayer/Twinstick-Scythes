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
    get => GetDebounced();
    set => SetDebounced(value);
  }

  public T GetDebounced() => this.value_;

  public void SetDebounced(T value) {
    if (TimeSinceLastUpdate.TotalSeconds >= this.secondsBetweenUpdate_ ||
        value.Equals(this.value_)) {
      SetForced(value);
    }
  }

  public void SetForced(T value) {
    this.value_ = value;
    this.lastUpdateTime_ = this.GetCurrentTime_();
  }

  public TimeSpan TimeSinceLastUpdate
    => this.GetCurrentTime_() - this.lastUpdateTime_;

  private DateTime GetCurrentTime_() => DateTime.UtcNow;
}