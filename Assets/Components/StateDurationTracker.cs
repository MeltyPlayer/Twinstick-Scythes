using System;


public class StateDurationTracker<T> {
  private T value_;
  private DateTime lastUpdateTime_;

  public StateDurationTracker(T initialValue) {
    this.value_ = initialValue;
    this.lastUpdateTime_ = GetCurrentTime_();
  }

  public T Value {
    get => this.value_;
    set {
      if (!value.Equals(this.value_)) {
        this.value_ = value;
        this.lastUpdateTime_ = this.GetCurrentTime_();
      }
    }
  }

  public TimeSpan TimeSinceLastUpdate => this.GetCurrentTime_() - this.lastUpdateTime_;

  private DateTime GetCurrentTime_() => DateTime.UtcNow;
}