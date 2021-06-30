
using System;
using System.Collections.Generic;

public class EList<T> {

    public delegate void Action();
    public event Action<List<T>> OnNewValue;

    public List<T> List => new List<T>(list);

    private List<T> list = new List<T>();

    public void Add(T item) {
        list.Add(item);
        OnNewValue?.Invoke(list);
    }

    public void Remove(T item) {
        list.Remove(item);
        OnNewValue?.Invoke(list);
    }
}