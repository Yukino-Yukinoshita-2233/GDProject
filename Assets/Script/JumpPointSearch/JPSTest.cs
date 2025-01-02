﻿using System.Collections.Generic;
using UnityEngine;
using PathFinding;

public class JPSTest : MonoBehaviour
{
    private IMap imap;
    private JPS _jps;
    private float speed = 3;
    private void Start()
    {
        // 获取地图数据
        imap = QuadMapCreate.CreateQuad("Terrain7", 0, 0, 10, 20);
        //imap = new MapQuad(0, 0, 80, 45, 1.0f, 1.0f);
        // 初始化 算法，并将地图数据传递进去
        _jps = new JPS(imap);

        new MapToolsDrawNode(imap);
        CreatePerson();
    }

    private Stack<Position> _stackPos = new Stack<Position>();
    /// <summary>
    /// 搜索路径
    /// </summary>
    private void StartSearchPath()
    {
        DestroyGO();

        // 获取开始位置、终点位置
        Position from = new Position(personGo.transform.position.z, personGo.transform.position.x);
        Position to = new Position(destination.transform.position.z, destination.transform.position.x);

        // 搜索路径，如果返回结果为 null，则说明没有找到路径，否则说明已找到路径，且 pathNode 为终点节点
        // 顺着 pathNode 一直向上查找 parentNode，最终将到达开始点
        Node pathNode = _jps.SearchPath(from, to);

        //需要通过 pathNode 逆序向上查找 parentNode
        //所以使用 栈：FILO 先进后出,存放路径点
        _stackPos.Clear();
        while (null != pathNode)
        {
            Position pos = imap.NodeToPosition(pathNode);
            // 数据入栈
            _stackPos.Push(pos);
            pathNode = pathNode.Parent;
        }
        // 顺次执行 _stackPos.Peek(); 将 路点从 栈中取出即是从 开始点到结束点的路径
    }

    private void Move()
    {
        if (_stackPos.Count <= 0)
        {
            return;
        }

        Position position = _stackPos.Peek();
        Vector3 destinationPos = new Vector3(position.ColPos, 0.3f, position.RowPos);
        Vector3 dir = destinationPos - personGo.transform.position;
        personGo.transform.Translate(dir.normalized * speed * Time.deltaTime, Space.World);
        if (Vector3.Distance(personGo.transform.position, destinationPos) > 0.05f)
        {
            return;
        }
        _stackPos.Pop();
        CreatePathPos();
    }

    #region DebugUse
    private List<GameObject> pathGoList = new List<GameObject>();
    public static List<KeyValuePair<int, Node>> checkNodeList = new List<KeyValuePair<int, Node>>();
    private void Update()
    {
        if (CreateCheckGameObject())
        {
            return;
        }

        Move();
    }

    private void OnGUI()
    {
        GUI.skin.button.fontSize = 40;
        if (GUI.Button(new Rect(10, 10, 200, 80), "Start"))
        {
            StartSearchPath();
        }

        // JPS+ 还没有完成，不能使用
        if (!_jps.IsPreprocess)
        {
            if (GUI.Button(new Rect(300, 10, 400, 80), "JPS+ 预处理地图"))
            {
                Debug.LogError("还未完成，功能还不能使用");
                _jps.Preprocess();
            }
        }
    }

    /// <summary>
    /// 创建加入到 OpenList 表的节点
    /// 和从 OpenList 中取出来的节点
    /// </summary>
    /// <returns></returns>
    private bool CreateCheckGameObject()
    {
        if (checkNodeList.Count <= 0)
        {
            return false;
        }

        KeyValuePair<int, Node> kv = checkNodeList[0];
        Node node = kv.Value;
        checkNodeList.RemoveAt(0);

        Position pos = imap.NodeToPosition(node);
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Capsule);
        go.transform.position = (kv.Key == 1) ? new Vector3(pos.ColPos + 0.1f, 0f, pos.RowPos + 0.1f) : new Vector3(pos.ColPos - 0.1f, 0f, pos.RowPos - 0.1f);
        go.transform.localScale = Vector3.one * 0.3f;
        go.name = (kv.Key == 1) ? string.Format("open:{0}_{1}", node.Row, node.Col) : string.Format("insertOpen:{0}_{1}", node.Row, node.Col);
        go.GetComponent<Renderer>().material.color = (kv.Key == 1) ? Color.green : Color.blue;
        pathGoList.Add(go);

        return true;
    }

    private void DestroyGO()
    {
        for (int i = pathGoList.Count - 1; i >= 0; --i)
        {
            GameObject.Destroy(pathGoList[i]);
        }
        pathGoList.Clear();
    }

    /// <summary>
    /// 创建走过的节点
    /// </summary>
    private void CreatePathPos()
    {
        GameObject go = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        go.transform.localScale = Vector3.one * 0.2f;
        go.transform.position = new Vector3(personGo.transform.position.x, 0.6f, personGo.transform.position.z);
        go.GetComponent<Renderer>().material.color = Color.red;
        pathGoList.Add(go);
    }

    private GameObject personGo;
    private GameObject destination;
    private void CreatePerson()
    {
        // 角色出发点
        personGo = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        personGo.name = "Person";
        personGo.transform.position = new Vector3(2.5f, 0.3f, 3.85f);
        personGo.GetComponent<Renderer>().material.color = Color.green;

        // 目标终点
        destination = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        destination.name = "Destination";
        destination.transform.position = new Vector3(10.5f, 0.3f, 4f);
        destination.GetComponent<Renderer>().material.color = Color.black;
    }
    #endregion
}