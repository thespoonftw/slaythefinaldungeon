
using System;

public class EProperty<T> {

    public delegate void Action();
    public event Action PreUpdate;
    public event Action<T> PreNewValue;
    public event Action<T> PreOldValue;
    public event Action<T, T> PreChange;
    public event Action OnUpdate;
    public event Action<T> OnNewValue;
    public event Action<T> OnOldValue;
    public event Action<T, T> OnChange;

    private T privateValue;
    public T Value {
        get { return privateValue; }
        set {
            var oldValue = privateValue;
            PreUpdate?.Invoke();
            PreNewValue?.Invoke(value);
            PreOldValue?.Invoke(oldValue);
            PreChange?.Invoke(oldValue, value);
            privateValue = value;
            OnUpdate?.Invoke();
            OnNewValue?.Invoke(value);
            OnOldValue?.Invoke(oldValue);
            OnChange?.Invoke(oldValue, value);
        }
    }

}