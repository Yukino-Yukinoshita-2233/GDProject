using UnityEngine;

/// <summary>
/// GameManager 类，管理游戏的整体逻辑和状态。
/// 继承自 Singleton<GameManager> 以确保只有一个实例。
/// </summary>
public class GameManager : Singleton<GameManager>
{
    // 游戏的当前状态
    public GameState State { get; private set; }

    /// <summary>
    /// Unity 的 Start 方法，游戏开始时调用。
    /// </summary>
    private void Start()
    {
        // 初始化游戏状态为 Init
        ChangeState(GameState.Init);
    }

    /// <summary>
    /// 改变游戏的状态并执行相应操作。
    /// </summary>
    /// <param name="newState">新的游戏状态。</param>
    public void ChangeState(GameState newState)
    {
        State = newState;

        // 根据新的状态执行相应的操作
        switch (newState)
        {
            case GameState.Init:
                // 初始化游戏
                Debug.Log("游戏初始化");
                break;
            case GameState.Playing:
                // 游戏进行中
                Debug.Log("游戏进行中");
                break;
            case GameState.Paused:
                // 游戏暂停
                Debug.Log("游戏暂停");
                break;
            case GameState.GameOver:
                // 游戏结束
                Debug.Log("游戏结束");
                break;
        }
    }
}

/// <summary>
/// GameState 枚举，表示游戏的不同状态。
/// </summary>
public enum GameState
{
    Init,       // 初始化
    Playing,    // 进行中
    Paused,     // 暂停
    GameOver    // 结束
}
