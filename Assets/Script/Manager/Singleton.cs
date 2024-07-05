using UnityEngine;

/// <summary>
/// Singleton ���࣬����ȷ��ֻ��һ��ʵ�������ṩȫ�ַ��ʵ㡣
/// �̳��� MonoBehaviour �Ա��ܸ��ӵ� Unity GameObject��
/// </summary>
/// <typeparam name="T">ϣ��ʵ�ֵ���ģʽ�������͡�</typeparam>
public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    // Ψһʵ����˽�о�̬�ֶ�
    private static T _instance;

    // �����̰߳�ȫ��������
    private static readonly object _lock = new object();

    /// <summary>
    /// ������̬���ԣ��ṩ����Ψһʵ���ķ�����
    /// </summary>
    public static T Instance
    {
        get
        {
            // ʹ����ȷ���̰߳�ȫ
            lock (_lock)
            {
                // ���ʵ��Ϊ�գ����������ʵ���򴴽���ʵ��
                if (_instance == null)
                {
                    // �ڳ����в������е�ʵ��
                    _instance = (T)FindObjectOfType(typeof(T));

                    // ȷ��������û�ж��ʵ��
                    if (FindObjectsOfType(typeof(T)).Length > 1)
                    {
                        Debug.LogError("[Singleton] �����в�Ӧ���ж��ʵ������ȷ��ֻ��һ��ʵ����");
                        return _instance;
                    }

                    // ���û���ҵ�ʵ�����򴴽��µ�ʵ��
                    if (_instance == null)
                    {
                        // �����µ� GameObject �����ӵ���ʵ��
                        GameObject singleton = new GameObject();
                        _instance = singleton.AddComponent<T>();
                        singleton.name = "(singleton) " + typeof(T).ToString();

                        // ȷ������ʵ���ڳ����л�ʱ��������
                        DontDestroyOnLoad(singleton);

                        Debug.Log("[Singleton] ������һ���µĵ���ʵ��: " + singleton.name);
                    }
                    else
                    {
                        Debug.Log("[Singleton] ʹ���Ѵ��ڵ�ʵ��: " + _instance.gameObject.name);
                    }
                }

                return _instance;
            }
        }
    }
}
