using System;
using System.Collections.Generic;

//public class PriorityQueue<T>
//{
//    private List<(T item, int priority)> elements = new List<(T, int)>();

//    public int Count => elements.Count;

//    // ���Ԫ�ص�����
//    public void Enqueue(T item, int priority)
//    {
//        elements.Add((item, priority));
//        elements.Sort((x, y) => x.priority.CompareTo(y.priority)); // �����ȼ���С��������
//    }

//    // �Ӷ�����ȡ�����ȼ���ߵ�Ԫ��
//    public T Dequeue()
//    {
//        if (elements.Count == 0)
//            throw new InvalidOperationException("The priority queue is empty.");

//        var element = elements[0];
//        elements.RemoveAt(0);
//        return element.item;
//    }

//    // ����Ƿ����ĳ��Ԫ��
//    public bool Contains(T item)
//    {
//        return elements.Exists(e => EqualityComparer<T>.Default.Equals(e.item, item));
//    }

//    // �Ƴ�ĳ��Ԫ��
//    public void Remove(T item)
//    {
//        elements.RemoveAll(e => EqualityComparer<T>.Default.Equals(e.item, item));
//    }
//}
