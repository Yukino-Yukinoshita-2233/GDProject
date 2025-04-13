using MapManagernamespace;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraControll2r : MonoBehaviour
{
    [Header("移动和缩放参数")]
    public float moveSpeed = 10f;      // 摄像机移动速度
    public float zoomSpeed = 5f;       // 缩放速度（通过改变摄像机高度实现缩放）
    public float minZoom = 10f;        // 最小高度（缩放上限）
    public float maxZoom = 100f;       // 最大高度（缩放下限）
    
    [Header("地图边界限制偏移（可根据需要调整）")]
    public float Limit_up = 0;         // 地图上边界偏移
    public float Limit_down = 0;       // 地图下边界偏移
    public float Limit_right = 0;      // 地图右边界偏移
    public float Limit_left = 0;       // 地图左边界偏移

    private Camera cam;              // 主摄像机引用
    private int[,] gridMap;          // 地图数据，通过 MapManager 获取

    private Vector3 lastMousePosition;
    private bool isMiddleMousePressed;

    void Start()
    {
        // 获取摄像机组件，并确保使用透视投影
        cam = GetComponent<Camera>();
        cam.orthographic = false;

        // 设置摄像机为60度俯视角（X轴旋转 60°），保留Y轴角度（可根据需要设置其他旋转）
        Vector3 currentEuler = transform.eulerAngles;
        transform.eulerAngles = new Vector3(60f, currentEuler.y, currentEuler.z);

        // 获取地图数据（假设 MapManager 中有静态 gridMap）
        gridMap = MapManager.gridMap;

        // 根据地图尺寸设置摄像机初始位置（地图中心上方，初始高度设为40）
        float mapWidth = gridMap.GetLength(0);
        float mapHeight = gridMap.GetLength(1);
        transform.position = new Vector3(mapWidth / 2f, 40f, mapHeight / 2f);
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
        RestrictCameraPosition();
    }

    /// <summary>
    /// 处理摄像机移动：键盘 WSAD 和鼠标中键拖动
    /// </summary>
    void HandleMovement()
    {
        // 键盘控制
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical);
        if (moveDirection.magnitude >= 0.1f)
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }

        // 鼠标中键拖动控制
        if (Input.GetMouseButtonDown(2))
        {
            isMiddleMousePressed = true;
            lastMousePosition = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(2))
        {
            isMiddleMousePressed = false;
        }
        if (isMiddleMousePressed)
        {
            Vector3 delta = lastMousePosition - Input.mousePosition;
            lastMousePosition = Input.mousePosition;
            // 由于摄像机视角固定（60°），这里直接将屏幕位移转换为世界位移（简单处理，实际项目中可根据需求细调）
            Vector3 moveDelta = new Vector3(delta.x, 0, delta.y) * moveSpeed * Time.deltaTime;
            transform.Translate(moveDelta, Space.World);
        }
    }

    /// <summary>
    /// 处理摄像机缩放：通过鼠标滚轮调整摄像机高度（Y轴）
    /// </summary>
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            Vector3 pos = transform.position;
            // 使用鼠标滚轮改变摄像机高度，实现缩放效果
            pos.y -= scroll * zoomSpeed;
            pos.y = Mathf.Clamp(pos.y, minZoom, maxZoom);
            transform.position = pos;
        }
    }

    /// <summary>
    /// 限制摄像机位置，确保摄像机视野（与地面的交点包围盒）始终位于地图边界内
    /// </summary>
    void RestrictCameraPosition()
    {
        // 获取地图尺寸（假设地图位于XZ平面，X:0~mapWidth, Z:0~mapHeight）
        float mapWidth = gridMap.GetLength(0);
        float mapHeight = gridMap.GetLength(1);
        float mapMinX = 0 + Limit_left;
        float mapMaxX = mapWidth - Limit_right;
        float mapMinZ = 0 + Limit_down;
        float mapMaxZ = mapHeight - Limit_up;

        // 计算当前摄像机视锥与地面（y=0）交点的包围盒
        Bounds viewBounds = CalculateCameraViewBounds();

        // 如果视野包围盒超出地图边界，则计算需要的偏移量
        Vector3 delta = Vector3.zero;
        if (viewBounds.min.x < mapMinX)
        {
            delta.x = mapMinX - viewBounds.min.x;
        }
        if (viewBounds.max.x > mapMaxX)
        {
            delta.x = mapMaxX - viewBounds.max.x;
        }
        if (viewBounds.min.z < mapMinZ)
        {
            delta.z = mapMinZ - viewBounds.min.z;
        }
        if (viewBounds.max.z > mapMaxZ)
        {
            delta.z = mapMaxZ - viewBounds.max.z;
        }

        // 调整摄像机位置，使视野包围盒完全位于地图边界内
        transform.position += delta;
    }

    /// <summary>
    /// 计算摄像机视锥体与地面（y=0）的交点包围盒，
    /// 通过视口四角的射线与地面平面的交点得到
    /// </summary>
    /// <returns>当前视野在地面上的包围盒</returns>
    Bounds CalculateCameraViewBounds()
    {
        // 定义地面平面（y=0，法向量向上）
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        Vector3[] corners = new Vector3[4];
        // 视口四个角：左下、右下、左上、右上
        Vector2[] viewportCorners = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };

        for (int i = 0; i < viewportCorners.Length; i++)
        {
            Ray ray = cam.ViewportPointToRay(viewportCorners[i]);
            float enter;
            if (groundPlane.Raycast(ray, out enter))
            {
                // 获取射线与地面交点
                corners[i] = ray.GetPoint(enter);
            }
            else
            {
                corners[i] = Vector3.zero;
            }
        }

        // 用四个交点构造包围盒
        Bounds bounds = new Bounds(corners[0], Vector3.zero);
        for (int i = 1; i < corners.Length; i++)
        {
            bounds.Encapsulate(corners[i]);
        }
        return bounds;
    }
}
