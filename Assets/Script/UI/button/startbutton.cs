using UnityEngine;
using UnityEngine.SceneManagement;  // ����SceneManagement�����ռ�

public class startbutton : MonoBehaviour
{
    // ���������󶨵�UI��ť�ĵ���¼�
    public void startgame()
    {
        // ����ָ�����Ƶĳ���
        SceneManager.LoadScene(1);
    }
}
