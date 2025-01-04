using UnityEngine;

public class pausebutton : MonoBehaviour
{
    // 需要显示的暂停菜单Canvas
    public GameObject pauseMenuCanvas;
    //需要关闭暂停按钮canvas
    public GameObject pausebtnCanvas;

    // 游戏是否处于暂停状态
    private bool isPaused = false;

    void Start()
    {
        // 确保游戏开始时Canvas是关闭的
        if (pauseMenuCanvas != null)
        {
            pauseMenuCanvas.SetActive(false);
        }
    }

    // 当按钮点击时调用的函数
    public void TogglePause()
    {
        // 切换暂停状态
        isPaused = !isPaused;

        if (isPaused)
        {
            // 暂停游戏
            Time.timeScale = 0f; // 停止游戏的时间流动

            // 显示暂停菜单
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
        //    // 恢复游戏
        //    Time.timeScale = 1f; // 恢复正常时间流动

        //    // 隐藏暂停菜单
        //    if (pauseMenuCanvas != null)
        //    {
        //        pauseMenuCanvas.SetActive(false);
        //    }
        //}
    }
}
