using System.Collections;
using System.Collections.Generic;
using B1;
using UnityEngine;

public class ListStack<T> : Base
{
    private string m_Message = null;
    public ListStack(string f_Message, uint count = 10)
    {
        count = count <= 0 ? 1 : count;
        m_List = new List<T>(new T[count]);
        m_AddCount = count;
        m_Pointer = 0;
        m_Message = f_Message;

    }
    private List<T> m_List = new();
    private uint m_Pointer = 0;
    public uint Count => m_Pointer;

    private uint m_AddCount = 0;

    public T this[int key]
    {
        get
        {
            return m_List[key];
        }
    }
    public bool TryValue(out T f_Value)
    {
        f_Value = default(T);
        if (m_Pointer > 0)
        {
            f_Value = m_List[(int)m_Pointer - 1];
            return true;
        }
        else
        {
            return false;
        }
        
    }

    public bool TryPop(out T value)
    {
        bool isTry = false;
        value = default(T);
        if (m_Pointer > 0)
        {
            value = m_List[(int)--m_Pointer];
            m_List.Remove(value);
            isTry = true;
        }
        return isTry;
    }
    public void Push(T item)
    {
        m_List[(int)m_Pointer++] = item;
        if (m_Pointer >= m_List.Count)
        {
            Extend();
        }

    }
    public void Extend()
    {
        var newList = new List<T>(new T[(uint)m_List.Count + m_AddCount]);
        for (int i = 0; i < m_List.Count; i++)
        {
            newList[i] = m_List[i];
        }
        m_List = newList;
    }
    public bool Contains(T f_Item)
    {
        return m_List.Contains(f_Item);
    }
    public void LogData()
    {
        string str = $"{m_Message}";
        uint index = 0;
        foreach (var item in m_List)
        {
            str += $"\n[ {index++} ] = {item}";
        }
        Log(str);
    }

    public IEnumerable<KeyValuePair<int, T>> GetEnumerator()
    {
        for (int i = 0; i < m_Pointer; i++)
        {
            var index = i;
            var tempItem = m_List[index];
            yield return new KeyValuePair<int, T>(index, tempItem);
        }
    }
}

public class DicStack<TKey, TValue> : Base
{
    public TValue this[TKey key]
    {
        get
        {
            return m_Dic[key];
        }
    }


    private string m_Message = null;
    public DicStack(string f_Message, uint count = 10)
    {
        count = count <= 0 ? 1 : count;
        m_KeyStack = new ListStack<TKey>(f_Message, count);
        m_Dic = new();
        m_Message = f_Message;

    }
    private Dictionary<TKey, TValue> m_Dic = null;
    private ListStack<TKey> m_KeyStack = null;
    public uint Count => m_KeyStack.Count;

    public bool TryPop(out TValue f_Value)
    {
        bool isTry = false;
        f_Value = default(TValue);
        if (m_KeyStack.TryPop(out var key))
        {
            f_Value = m_Dic[key];
            m_Dic.Remove(key);
            isTry = true;
        }
        return isTry;
    }
    public void Push(TKey f_Key, TValue f_Value)
    {
        if (!m_KeyStack.Contains(f_Key) || m_Dic.ContainsKey(f_Key))
        {
            m_KeyStack.Push(f_Key);
            m_Dic.Add(f_Key, f_Value);
        }
        else
        {
            Log($"字典列表添加失败 已经存在 键值对  key = {f_Key}   " +
                $"key stack = {m_KeyStack.Contains(f_Key)}  " +
                $"dic data = {m_Dic.ContainsKey(f_Key)}");
        }
    }
    public bool TryGetValue(TKey f_Key, out TValue f_Value)
    {
        bool isTry = false;
        f_Value = default(TValue);
        if (m_KeyStack.Contains(f_Key))
        {
            f_Value = m_Dic[f_Key];
            isTry = true;
        }
        return isTry;
    }
    public void LogData()
    {
        string str = $"{m_Message}";
        uint index = 0;
        foreach (var item in m_Dic)
        {
            str += $"\n[ {index++} ] = " +
                $"\n{{" +
                $"\n\tkey \t= {item.Key}" +
                $"\n\tvalue \t= {item.Value}" +
                $"\n}}";
        }
        Log(str);
    }

    public IEnumerator<KeyValuePair<TKey,TValue>> GetEnumerator()
    {
        foreach (var item in m_Dic)
        {
            yield return item;
        }
    }
}
