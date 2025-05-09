using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    static T instance;  // 私有静态实例
    public static T Instance { get { return instance; } }  // 公开实例属性

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            // 切换场景时不销毁这个游戏物体
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            // 切换场景时，如果场景里有单例游戏物体，在已经创建单例的情况下，销毁多余的游戏物体
            Destroy(gameObject);
        }
    }
}
