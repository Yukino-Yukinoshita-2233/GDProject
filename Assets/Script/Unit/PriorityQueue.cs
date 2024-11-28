using System;
using System.Collections.Generic;

//public class PriorityQueue<T>
//{
//    private List<(T item, int priority)> elements = new List<(T, int)>();

//    public int Count => elements.Count;

//    // 添加元素到队列
//    public void Enqueue(T item, int priority)
//    {
//        elements.Add((item, priority));
//        elements.Sort((x, y) => x.priority.CompareTo(y.priority)); // 按优先级从小到大排序
//    }

//    // 从队列中取出优先级最高的元素
//    public T Dequeue()
//    {
//        if (elements.Count == 0)
//            throw new InvalidOperationException("The priority queue is empty.");

//        var element = elements[0];
//        elements.RemoveAt(0);
//        return element.item;
//    }

//    // 检查是否包含某个元素
//    public bool Contains(T item)
//    {
//        return elements.Exists(e => EqualityComparer<T>.Default.Equals(e.item, item));
//    }

//    // 移除某个元素
//    public void Remove(T item)
//    {
//        elements.RemoveAll(e => EqualityComparer<T>.Default.Equals(e.item, item));
//    }
//}
