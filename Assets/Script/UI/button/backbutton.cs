using UnityEngine;

public class ResumeGame : MonoBehaviour
{
    // 游戏中的pauseCanvas
    public GameObject pausebtn;

    // 暂停菜单Canvas
    public GameObject pauseMenuCanvas;

    // 游戏是否暂停的标志
    private bool isPaused = false;

    void Start()
    {
        // 确保游戏开始时Canvas是显示的，暂停菜单是隐藏的
        if (pausebtn != null)
        {
            pausebtn.SetActive(true);  // 游戏UI显示
        }

        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(false);  // 暂停菜单隐藏
        }
    }

    // 当按钮点击时调用的函数
    public void ResumeGameAction()
    {
        // 继续游戏，恢复时间流动
        Time.timeScale = 1f; // 恢复游戏的时间流动

        // 显示游戏UI，隐藏暂停菜单
        if (pausebtn != null)
        {
            pausebtn.SetActive(true);
        }

        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(false);
        }

        // 更新暂停标志
        isPaused = false;
    }
}
