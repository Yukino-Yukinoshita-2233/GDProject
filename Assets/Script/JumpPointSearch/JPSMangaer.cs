using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathFinding;
using MapManagernamespace;

public class JPSMangaer : SingletonMono<JPSMangaer>
{

    public IMap imap;
    public JPS _jps;

    MapToolsDrawNode mapToolsDrawNode;
    public int X = 73;
    public int Y = 45;
    // Start is called before the first frame update
    void Start()
    {
        //imap = new MapQuad(0, 0, 45, 73, 1.0f, 1.0f);
        imap = new MapQuad(0, 0, Y, X, 1.0f, 1.0f);
        _jps = new JPS(imap);
        mapToolsDrawNode = new MapToolsDrawNode(imap);
    }

    public void SetNode()
    {
        int[,] maps = MapManager.gridMap;
        //Debug.Log(imap.Grid().Length);
        Node[] nodes = imap.Grid();

        //for (int Row = 0; Row < 80; Row++)
        //{
        //    for (int col = 0; col < 45; col++)
        //    {
        //        int num = Row * 45 + col;

        //        if (maps[Row,col]==3|| maps[Row, col] == 1)
        //        {
        //            nodes[num].NodeType = NodeType.Obstacle;
        //            Debug.Log(Row + " " + col);
        //        }
        //        else
        //        {
        //            nodes[num].NodeType = NodeType.Smooth;
        //        }

        //        int Gonum = Row * 45 + col;
        //        GameObject go = GameObject.Find("NodeParent/" + col + "_" + Row);
        //        go.GetComponent<Renderer>().material.color = mapToolsDrawNode.NodeColor(nodes[Gonum].NodeType);
        //    }
        //}

        for (int row = 0; row < Y; ++row)
        {
            for (int col = 0; col < X; ++col)
            {
                int num = RCToIndex(row, col);
                string name = string.Format("{0}_{1}", row, col);

                GameObject go = GameObject.Find("NodeParent/" + name);

                int index_x = (int)go.transform.position.x;
                int index_z = (int)go.transform.position.z;


                if (maps[index_x, index_z] == 3 || maps[index_x, index_z] == 1)
                {
                    nodes[num].NodeType = NodeType.Obstacle;
                }
                else
                {
                    nodes[num].NodeType = NodeType.Smooth;
                }

                //go.GetComponent<Renderer>().material.color = mapToolsDrawNode.NodeColor(nodes[num].NodeType);
            }
        }
    }

    public (int, int) GetCoordinatesFromValue(int value, int rows, int cols)
    {
        // 计算列索引，使用取模运算
        int col = value % cols;
        // 计算行索引，使用整除运算
        int row = value / rows;

        // 由于数组索引是从0开始的，所以不需要额外减1
        return (row, col);
    }

    private int RCToIndex(int row, int col)
    {
        int index = row * X + col;
        return index;
    }
}
