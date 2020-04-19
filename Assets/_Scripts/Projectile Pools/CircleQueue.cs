
public class CircleQueue<T> {

    private T[] _q;
    private int _head = -1, _tail = -1, _length;

    public CircleQueue(int length)
    {
        _length = length;
        _q = new T[_length];
    }

    public void Queue(T val)
    {
        if (IsFull())
        {
            UnityEngine.Debug.Log("Circle Queue full");
            return;
        }
        _tail = (_tail == _length - 1) ? 0 : _tail + 1;
        _q[_tail] = val;
        //UnityEngine.Debug.Log (val.ToString () + " enqueued @" + _tail);
        if (_head == -1) _head = 0;
    }

    public T Dequeue()
    {
        T val;
        if (IsEmpty())
        {
            UnityEngine.Debug.Log("Circle Queue empty");
            return default(T);
        }
        val = _q[_head];
        _q[_head] = default(T);
        //UnityEngine.Debug.Log (val.ToString () + " dequeued @" + _head);
        if (_head == _tail) _head = _tail = -1;
        else _head = (_head == _length - 1) ? 0 : _head + 1;
        return (val);
    }

    public bool IsEmpty() { return (_head == -1) ? true : false; }

    public bool IsFull() { return (_head == 0 && (_tail == _length - 1)) || (_tail + 1 == _head) ? true : false; }
    
}
