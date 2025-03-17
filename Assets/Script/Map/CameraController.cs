using MapManagernamespace;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraControll2r : MonoBehaviour
{
    [Header("�ƶ������Ų���")]
    public float moveSpeed = 10f;      // ������ƶ��ٶ�
    public float zoomSpeed = 5f;       // �����ٶȣ�ͨ���ı�������߶�ʵ�����ţ�
    public float minZoom = 10f;        // ��С�߶ȣ��������ޣ�
    public float maxZoom = 100f;       // ���߶ȣ��������ޣ�
    
    [Header("��ͼ�߽�����ƫ�ƣ��ɸ�����Ҫ������")]
    public float Limit_up = 0;         // ��ͼ�ϱ߽�ƫ��
    public float Limit_down = 0;       // ��ͼ�±߽�ƫ��
    public float Limit_right = 0;      // ��ͼ�ұ߽�ƫ��
    public float Limit_left = 0;       // ��ͼ��߽�ƫ��

    private Camera cam;              // �����������
    private int[,] gridMap;          // ��ͼ���ݣ�ͨ�� MapManager ��ȡ

    private Vector3 lastMousePosition;
    private bool isMiddleMousePressed;

    void Start()
    {
        // ��ȡ������������ȷ��ʹ��͸��ͶӰ
        cam = GetComponent<Camera>();
        cam.orthographic = false;

        // ���������Ϊ60�ȸ��ӽǣ�X����ת 60�㣩������Y��Ƕȣ��ɸ�����Ҫ����������ת��
        Vector3 currentEuler = transform.eulerAngles;
        transform.eulerAngles = new Vector3(60f, currentEuler.y, currentEuler.z);

        // ��ȡ��ͼ���ݣ����� MapManager ���о�̬ gridMap��
        gridMap = MapManager.gridMap;

        // ���ݵ�ͼ�ߴ������������ʼλ�ã���ͼ�����Ϸ�����ʼ�߶���Ϊ40��
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
    /// ����������ƶ������� WSAD ������м��϶�
    /// </summary>
    void HandleMovement()
    {
        // ���̿���
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical);
        if (moveDirection.magnitude >= 0.1f)
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }

        // ����м��϶�����
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
            // ����������ӽǹ̶���60�㣩������ֱ�ӽ���Ļλ��ת��Ϊ����λ�ƣ��򵥴���ʵ����Ŀ�пɸ�������ϸ����
            Vector3 moveDelta = new Vector3(delta.x, 0, delta.y) * moveSpeed * Time.deltaTime;
            transform.Translate(moveDelta, Space.World);
        }
    }

    /// <summary>
    /// ������������ţ�ͨ�������ֵ���������߶ȣ�Y�ᣩ
    /// </summary>
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(scroll) > 0.001f)
        {
            Vector3 pos = transform.position;
            // ʹ�������ָı�������߶ȣ�ʵ������Ч��
            pos.y -= scroll * zoomSpeed;
            pos.y = Mathf.Clamp(pos.y, minZoom, maxZoom);
            transform.position = pos;
        }
    }

    /// <summary>
    /// ���������λ�ã�ȷ���������Ұ�������Ľ����Χ�У�ʼ��λ�ڵ�ͼ�߽���
    /// </summary>
    void RestrictCameraPosition()
    {
        // ��ȡ��ͼ�ߴ磨�����ͼλ��XZƽ�棬X:0~mapWidth, Z:0~mapHeight��
        float mapWidth = gridMap.GetLength(0);
        float mapHeight = gridMap.GetLength(1);
        float mapMinX = 0 + Limit_left;
        float mapMaxX = mapWidth - Limit_right;
        float mapMinZ = 0 + Limit_down;
        float mapMaxZ = mapHeight - Limit_up;

        // ���㵱ǰ�������׶����棨y=0������İ�Χ��
        Bounds viewBounds = CalculateCameraViewBounds();

        // �����Ұ��Χ�г�����ͼ�߽磬�������Ҫ��ƫ����
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

        // ���������λ�ã�ʹ��Ұ��Χ����ȫλ�ڵ�ͼ�߽���
        transform.position += delta;
    }

    /// <summary>
    /// �����������׶������棨y=0���Ľ����Χ�У�
    /// ͨ���ӿ��Ľǵ����������ƽ��Ľ���õ�
    /// </summary>
    /// <returns>��ǰ��Ұ�ڵ����ϵİ�Χ��</returns>
    Bounds CalculateCameraViewBounds()
    {
        // �������ƽ�棨y=0�����������ϣ�
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        Vector3[] corners = new Vector3[4];
        // �ӿ��ĸ��ǣ����¡����¡����ϡ�����
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
                // ��ȡ��������潻��
                corners[i] = ray.GetPoint(enter);
            }
            else
            {
                corners[i] = Vector3.zero;
            }
        }

        // ���ĸ����㹹���Χ��
        Bounds bounds = new Bounds(corners[0], Vector3.zero);
        for (int i = 1; i < corners.Length; i++)
        {
            bounds.Encapsulate(corners[i]);
        }
        return bounds;
    }
}
