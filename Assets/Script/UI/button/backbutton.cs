using UnityEngine;

public class ResumeGame : MonoBehaviour
{
    // ��Ϸ�е�pauseCanvas
    public GameObject pausebtn;

    // ��ͣ�˵�Canvas
    public GameObject pauseMenuCanvas;

    // ��Ϸ�Ƿ���ͣ�ı�־
    private bool isPaused = false;

    void Start()
    {
        // ȷ����Ϸ��ʼʱCanvas����ʾ�ģ���ͣ�˵������ص�
        if (pausebtn != null)
        {
            pausebtn.SetActive(true);  // ��ϷUI��ʾ
        }

        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(false);  // ��ͣ�˵�����
        }
    }

    // ����ť���ʱ���õĺ���
    public void ResumeGameAction()
    {
        // ������Ϸ���ָ�ʱ������
        Time.timeScale = 1f; // �ָ���Ϸ��ʱ������

        // ��ʾ��ϷUI��������ͣ�˵�
        if (pausebtn != null)
        {
            pausebtn.SetActive(true);
        }

        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(false);
        }

        // ������ͣ��־
        isPaused = false;
    }
}
