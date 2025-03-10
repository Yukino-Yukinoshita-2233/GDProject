using MapManagernamespace;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraControll2r : MonoBehaviour
{
    public float moveSpeed = 10f;  // 移动速度
    public float zoomSpeed = 5f;   // 缩放速度
    public float newZoom = 10f;     // 当前缩放大小
    public float minZoom = 10f;    // 最小缩放距离（限制为地图短边）
    float maxZoom = 100f;    // 最大缩放距离
    public float boundaryPercent = 0.3f; // 边界限制的百分比

    private Camera camera;

    // 地图数据，通过 MapManager 获取
    private int[,] gridMap;

    private Vector3 lastMousePosition;
    private bool isMiddleMousePressed;

    void Start()
    {   
        camera = GetComponent<Camera>();
        gridMap = MapManager.gridMap; // 获取地图数据
        transform.position = new Vector3(gridMap.GetLength(0)/2+0.5f,40,gridMap.GetLength(1)/2-0.5f);
        camera.orthographicSize = (gridMap.GetLength(1) + 1) / 2;

    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
        RestrictCameraPosition();
    }

    // 处理摄像机的移动
    void HandleMovement()
    {
        // 处理WSAD键盘移动
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }

        // 处理鼠标中键拖动移动
        if (Input.GetMouseButtonDown(2))  // 按下鼠标中键
        {
            isMiddleMousePressed = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(2))  // 松开鼠标中键
        {
            isMiddleMousePressed = false;
        }

        if (isMiddleMousePressed)  // 鼠标中键按下时移动
        {
            Vector3 delta = lastMousePosition - Input.mousePosition;
            lastMousePosition = Input.mousePosition;

            // 通过鼠标移动量来平移摄像机
            Vector3 moveDelta = new Vector3(delta.x, 0, delta.y) * moveSpeed * Time.deltaTime;
            transform.Translate(moveDelta, Space.World);
        }
    }

    // 处理摄像机的缩放
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float newZoom = camera.orthographicSize - scroll * zoomSpeed;
        maxZoom = (gridMap.GetLength(1)+1) / 2;
        // 限制缩放值在最小和最大之间
        newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);

        camera.orthographicSize = newZoom;
    }

    // 限制摄像机位置，确保摄像机视角不超出地图的边界
    void RestrictCameraPosition()
    {
        // 获取地图的大小
        float mapWidth = gridMap.GetLength(0); // 地图的宽度 (X轴)
        float mapHeight = gridMap.GetLength(1); // 地图的高度 (Z轴)

        // 计算当前摄像机视角的边界
        float halfWidth = camera.orthographicSize * Screen.width / Screen.height; // 摄像机的宽度一半
        float halfHeight = camera.orthographicSize; // 摄像机的高度一半

        // 计算边界限制（地图的50%）
        float boundaryX = halfWidth * boundaryPercent;
        float boundaryZ = halfHeight * boundaryPercent;
        // 获取当前摄像机位置
        Vector3 cameraPosition = transform.position;

        // 限制摄像机的X轴位置
        cameraPosition.x = Mathf.Clamp(cameraPosition.x, boundaryX, mapWidth - boundaryX);

        // 限制摄像机的Z轴位置
        cameraPosition.z = Mathf.Clamp(cameraPosition.z, boundaryZ, mapHeight - boundaryZ);

        // 更新摄像机位置
        transform.position = cameraPosition;
    }
}