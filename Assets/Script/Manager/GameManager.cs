using UnityEngine;

/// <summary>
/// GameManager �࣬������Ϸ�������߼���״̬��
/// �̳��� Singleton<GameManager> ��ȷ��ֻ��һ��ʵ����
/// </summary>
public class GameManager : Singleton<GameManager>
{
    // ��Ϸ�ĵ�ǰ״̬
    public GameState State { get; private set; }

    /// <summary>
    /// Unity �� Start ��������Ϸ��ʼʱ���á�
    /// </summary>
    private void Start()
    {
        // ��ʼ����Ϸ״̬Ϊ Init
        ChangeState(GameState.Init);
    }

    /// <summary>
    /// �ı���Ϸ��״̬��ִ����Ӧ������
    /// </summary>
    /// <param name="newState">�µ���Ϸ״̬��</param>
    public void ChangeState(GameState newState)
    {
        State = newState;

        // �����µ�״ִ̬����Ӧ�Ĳ���
        switch (newState)
        {
            case GameState.Init:
                // ��ʼ����Ϸ
                Debug.Log("��Ϸ��ʼ��");
                break;
            case GameState.Playing:
                // ��Ϸ������
                Debug.Log("��Ϸ������");
                break;
            case GameState.Paused:
                // ��Ϸ��ͣ
                Debug.Log("��Ϸ��ͣ");
                break;
            case GameState.GameOver:
                // ��Ϸ����
                Debug.Log("��Ϸ����");
                break;
        }
    }
}

/// <summary>
/// GameState ö�٣���ʾ��Ϸ�Ĳ�ͬ״̬��
/// </summary>
public enum GameState
{
    Init,       // ��ʼ��
    Playing,    // ������
    Paused,     // ��ͣ
    GameOver    // ����
}
