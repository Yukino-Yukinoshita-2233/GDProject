using UnityEngine;
using UnityEngine.SceneManagement;  // 引入SceneManagement命名空间

public class startbutton : MonoBehaviour
{
    // 这个方法会绑定到UI按钮的点击事件
    public void startgame()
    {
        // 加载指定名称的场景
        SceneManager.LoadScene(1);
    }
}
