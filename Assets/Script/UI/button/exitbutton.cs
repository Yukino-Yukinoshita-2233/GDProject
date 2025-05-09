using UnityEngine;

public class exitbutton : MonoBehaviour
{
    // 用于退出游戏的方法
    public void Quit()
    {
        // 如果是在编辑器中，停止播放
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // 如果是构建后的版本，退出应用程序
            Application.Quit();
#endif
    }
}
