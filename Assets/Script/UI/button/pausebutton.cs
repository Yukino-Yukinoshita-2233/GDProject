using UnityEngine;

public class pausebutton : MonoBehaviour
{
    // ��Ҫ��ʾ����ͣ�˵�Canvas
    public GameObject pauseMenuCanvas;
    //��Ҫ�ر���ͣ��ťcanvas
    public GameObject pausebtnCanvas;

    // ��Ϸ�Ƿ�����ͣ״̬
    private bool isPaused = false;

    void Start()
    {
        // ȷ����Ϸ��ʼʱCanvas�ǹرյ�
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(false);
        }
    }

    // ����ť���ʱ���õĺ���
    public void TogglePause()
    {
        // �л���ͣ״̬
        isPaused = !isPaused;

        if (isPaused)
        {
            // ��ͣ��Ϸ
            Time.timeScale = 0f; // ֹͣ��Ϸ��ʱ������

            // ��ʾ��ͣ�˵�
            if (pauseMenuCanvas != null)
            {
                pauseMenuCanvas.SetActive(true);
            }
            if (pausebtnCanvas != null)
            {
                pausebtnCanvas.SetActive(false);
            }
        }
        //else
        //{
        //    // �ָ���Ϸ
        //    Time.timeScale = 1f; // �ָ�����ʱ������

        //    // ������ͣ�˵�
        //    if (pauseMenuCanvas != null)
        //    {
        //        pauseMenuCanvas.SetActive(false);
        //    }
        //}
    }
}
