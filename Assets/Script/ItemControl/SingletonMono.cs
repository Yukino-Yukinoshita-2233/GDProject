using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonMono<T> : MonoBehaviour where T : MonoBehaviour
{
    static T instance;  // ˽�о�̬ʵ��
    public static T Instance { get { return instance; } }  // ����ʵ������

    protected virtual void Awake()
    {
        if (instance == null)
        {
            instance = this as T;
            // �л�����ʱ�����������Ϸ����
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            // �л�����ʱ������������е�����Ϸ���壬���Ѿ���������������£����ٶ������Ϸ����
            Destroy(gameObject);
        }
    }
}
