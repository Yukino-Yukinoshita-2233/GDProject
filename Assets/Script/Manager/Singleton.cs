using UnityEngine;

/// <summary>
/// Singleton 基类，用于确保只有一个实例，并提供全局访问点。
/// 继承自 MonoBehaviour 以便能附加到 Unity GameObject。
/// </summary>
/// <typeparam name="T">希望实现单例模式的类类型。</typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // 唯一实例的私有静态字段
    private static T _instance;

    // 用于线程安全的锁对象
    private static readonly object _lock = new object();

    /// <summary>
    /// 公共静态属性，提供访问唯一实例的方法。
    /// </summary>
    public static T Instance
    {
        get
        {
            // 使用锁确保线程安全
            lock (_lock)
            {
                // 如果实例为空，则查找现有实例或创建新实例
                if (_instance == null)
                {
                    // 在场景中查找现有的实例
                    _instance = (T)FindObjectOfType(typeof(T));

                    // 确保场景中没有多个实例
                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("[Singleton] 场景中不应该有多个实例。请确保只有一个实例。");
                        return _instance;
                    }

                    // 如果没有找到实例，则创建新的实例
                    if (_instance == null)
                    {
                        // 创建新的 GameObject 并附加单例实例
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).ToString();

                        // 确保单例实例在场景切换时不被销毁
                        DontDestroyOnLoad(singleton);

                        Debug.Log("[Singleton] 创建了一个新的单例实例: " + singleton.name);
                    }
                    else
                    {
                        Debug.Log("[Singleton] 使用已存在的实例: " + _instance.gameObject.name);
                    }
                }

                return _instance;
            }
        }
    }
}
