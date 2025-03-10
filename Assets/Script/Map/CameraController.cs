using MapManagernamespace;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraControll2r : MonoBehaviour
{
    public float moveSpeed = 10f;  // �ƶ��ٶ�
    public float zoomSpeed = 5f;   // �����ٶ�
    public float newZoom = 10f;     // ��ǰ���Ŵ�С
    public float minZoom = 10f;    // ��С���ž��루����Ϊ��ͼ�̱ߣ�
    float maxZoom = 100f;    // ������ž���
    public float boundaryPercent = 0.3f; // �߽����Ƶİٷֱ�

    private Camera camera;

    // ��ͼ���ݣ�ͨ�� MapManager ��ȡ
    private int[,] gridMap;

    private Vector3 lastMousePosition;
    private bool isMiddleMousePressed;

    void Start()
    {   
        camera = GetComponent<Camera>();
        gridMap = MapManager.gridMap; // ��ȡ��ͼ����
        transform.position = new Vector3(gridMap.GetLength(0)/2+0.5f,40,gridMap.GetLength(1)/2-0.5f);
        camera.orthographicSize = (gridMap.GetLength(1) + 1) / 2;

    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
        RestrictCameraPosition();
    }

    // ������������ƶ�
    void HandleMovement()
    {
        // ����WSAD�����ƶ�
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 moveDirection = new Vector3(horizontal, 0f, vertical).normalized;

        if (moveDirection.magnitude >= 0.1f)
        {
            transform.Translate(moveDirection * moveSpeed * Time.deltaTime, Space.World);
        }

        // ��������м��϶��ƶ�
        if (Input.GetMouseButtonDown(2))  // ��������м�
        {
            isMiddleMousePressed = true;
            lastMousePosition = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(2))  // �ɿ�����м�
        {
            isMiddleMousePressed = false;
        }

        if (isMiddleMousePressed)  // ����м�����ʱ�ƶ�
        {
            Vector3 delta = lastMousePosition - Input.mousePosition;
            lastMousePosition = Input.mousePosition;

            // ͨ������ƶ�����ƽ�������
            Vector3 moveDelta = new Vector3(delta.x, 0, delta.y) * moveSpeed * Time.deltaTime;
            transform.Translate(moveDelta, Space.World);
        }
    }

    // ���������������
    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        float newZoom = camera.orthographicSize - scroll * zoomSpeed;
        maxZoom = (gridMap.GetLength(1)+1) / 2;
        // ��������ֵ����С�����֮��
        newZoom = Mathf.Clamp(newZoom, minZoom, maxZoom);

        camera.orthographicSize = newZoom;
    }

    // ���������λ�ã�ȷ��������ӽǲ�������ͼ�ı߽�
    void RestrictCameraPosition()
    {
        // ��ȡ��ͼ�Ĵ�С
        float mapWidth = gridMap.GetLength(0); // ��ͼ�Ŀ�� (X��)
        float mapHeight = gridMap.GetLength(1); // ��ͼ�ĸ߶� (Z��)

        // ���㵱ǰ������ӽǵı߽�
        float halfWidth = camera.orthographicSize * Screen.width / Screen.height; // ������Ŀ��һ��
        float halfHeight = camera.orthographicSize; // ������ĸ߶�һ��

        // ����߽����ƣ���ͼ��50%��
        float boundaryX = halfWidth * boundaryPercent;
        float boundaryZ = halfHeight * boundaryPercent;
        // ��ȡ��ǰ�����λ��
        Vector3 cameraPosition = transform.position;

        // �����������X��λ��
        cameraPosition.x = Mathf.Clamp(cameraPosition.x, boundaryX, mapWidth - boundaryX);

        // �����������Z��λ��
        cameraPosition.z = Mathf.Clamp(cameraPosition.z, boundaryZ, mapHeight - boundaryZ);

        // ���������λ��
        transform.position = cameraPosition;
    }
}