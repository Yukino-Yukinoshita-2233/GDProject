using UnityEngine;

public class exitbutton : MonoBehaviour
{
    // �����˳���Ϸ�ķ���
    public void Quit()
    {
        // ������ڱ༭���У�ֹͣ����
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // ����ǹ�����İ汾���˳�Ӧ�ó���
            Application.Quit();
#endif
    }
}
