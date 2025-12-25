using System.Collections;
using System.Runtime.CompilerServices;

namespace BlazeEngine;

//TODO Make unordered/ordered
public class Seq<T> : IEnumerable<T>
{
    private T[] m_Items;
    private int m_Count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Seq()
    {
        m_Items = new T[0];
        m_Count = 0;
    }

    public T[] Items => m_Items;
    public int Count => m_Count;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Add(T item)
    {
        EnsureCapacity(++m_Count);     
        m_Items[m_Count - 1] = item;
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AddRef(ref T item)
    {
        EnsureCapacity(++m_Count);       
        m_Items[m_Count - 1] = item;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void EnsureCapacity(int capacity)
    {
        int newSize = Math.Max(1, m_Items.Length);
        do {
            newSize *= 2;
        } while (newSize < capacity);
        Array.Resize(ref m_Items, newSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Get(int id)
    {
        return m_Items[id];
    }
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ref T GetRef(int id)
    {
        return ref m_Items[id];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Resize(int newSize)
    {
        m_Count = newSize;
        Array.Resize(ref m_Items, newSize);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Pop()
    {
        return m_Items[--m_Count];
    }
    
    public T this[int id]
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        get
        {
            return m_Items[id];
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        set
        {
            m_Items[id] = value;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Clear()
    {
        Resize(0);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public IEnumerator<T> GetEnumerator()
    {
        return new Enumerator(m_Items, m_Count);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    IEnumerator IEnumerable.GetEnumerator()
    {
        return new Enumerator(m_Items, m_Count);
    }

    public struct Enumerator : IEnumerator<T>
    {
        private T[] m_Items;
        private int m_Count;
        private int m_Id;
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Enumerator(T[] items, int count)
        {
            m_Items = items;
            m_Count = count;
            m_Id = -1;
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() { }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext()
        {
            return ++m_Id < m_Count;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Reset()
        {
            m_Id = -1;
        }

        T IEnumerator<T>.Current => m_Items[m_Id];
        object? IEnumerator.Current => m_Items[m_Id];
    }
}