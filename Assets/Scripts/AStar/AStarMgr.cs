using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 格子管理器类
/// </summary>
public class AStarMgr
{
    //管理器单例
    private static AStarMgr instance;
    public static AStarMgr Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new AStarMgr();
            }
            return instance;
        }
    }
    //地图相关所有格子对象容器
    public AStarNode[,] nodes;
    //开启列表
    List<AStarNode> openList = new List<AStarNode>();
    //关闭列表
    List<AStarNode> closeList = new List<AStarNode>();
    //格子地图的宽高
    public int mapW;
    public int mapH;

    /// <summary>
    /// 初始化格子地图
    /// </summary>
    /// <param name="w"></param>
    /// <param name="h"></param>
    public void InitNodeMap(int w,int h)
    {
        mapW = w;
        mapH = h;
        //初始化地图阻挡随机
        nodes = new AStarNode[w, h];
        int randomW = Random.Range(0, w);
        int randomH = Random.Range(0, h);
        for (int i=0; i < w; i++)
        {
            for(int j=0; j < h; j++)
            {
                //这里阻挡随机只是暂时展示逻辑使用的，真正项目中的阻挡信息需要从配置文件中读取出来
                AStarNode node = new AStarNode(i, j, Random.Range(0, 100) < 40 ? E_Node_Type.stop : E_Node_Type.walk);
                //if (i == randomW && j == randomH)
                //{
                //    node = new AStarNode(i, j, E_Node_Type.stop);
                //}
                //else
                //{
                //    node = new AStarNode(i, j, E_Node_Type.walk);
                //}
                nodes[i, j] = node;
            }
        }
    }

    //寻找相对最优路径
    int first = 0;
    public List<AStarNode> FindPath(Vector2 start,Vector2 end)
    {
        if(first == 0)
        {
            //首先判断传入的两个点对应的格子是否合法1、是否在范围内2、是否阻挡
            if (start.x < 0 || start.x >= mapW || start.y < 0 || start.y >= mapH || nodes[(int)start.x, (int)start.y].type == E_Node_Type.stop)
            {
                Debug.Log("输入起始点不合法");
                return null;
            }
            if (end.x < 0 || end.x >= mapW || end.y < 0 || end.y >= mapH || nodes[(int)end.x, (int)end.y].type == E_Node_Type.stop)
            {
                Debug.Log("输入结束点不合法");
                return null;
            }
            AStarNode startNode = nodes[(int)start.x, (int)start.y];
            closeList.Add(startNode);

            first++;
        }

        //从起点开始找周围的符合条件的格子并放入开启列表中
        for (var i=start.x-1;i<= start.x + 1; i++)
        {
            for(var j = start.y - 1; j <= start.y + 1; j++)
            {
                if (i == start.x && j == start.y) continue;
                if(i>=0 && i<mapW && j>=0 && j<mapH)
                {
                    if(nodes[(int)i, (int)j].type == E_Node_Type.walk)
                    {
                        int count = 0;
                        //这里可以直接使用Contains方法来判断
                        foreach (var node in openList)
                        {
                            if (node == nodes[(int)i, (int)j])
                            {
                                count++;
                            }
                        }
                        foreach (var node in closeList)
                        {
                            if (node == nodes[(int)i, (int)j])
                            {
                                count++;
                            }
                        }

                        if (count == 0)//找到符合条件的格子
                        {
                            AStarNode node = nodes[(int)i, (int)j];
                            //计算格子的相关信息
                            node.father = nodes[(int)start.x, (int)start.y];
                            if (i == start.x || j == start.x)
                            {
                                node.g += 1;
                            }
                            else
                            {
                                node.g += 1.4f;//格子对角线长度
                            }
                            node.h = Mathf.Abs(i - end.x) + Mathf.Abs(j - end.y);
                            node.f = node.g + node.h;//寻路消耗

                            openList.Add(nodes[(int)i, (int)j]);
                        }
                    }
                }
            }
        }

        //选出开启列表中寻路消耗最小的点
        if(openList != null && openList.Count != 0)
        {
            float cost = openList[0].f;
            int minCostIndex = 0;
            for (int k = 0; k < openList.Count; k++)
            {
                if (openList[k].f < cost)
                {
                    cost = openList[k].f;
                    minCostIndex = k;
                }
            }
            closeList.Add(openList[minCostIndex]);
            openList.RemoveAt(minCostIndex);
            AStarNode node = closeList[closeList.Count - 1];//找到最优点
            if (node.h == 0)//到达终点
            {
                List<AStarNode> path = new List<AStarNode>();
                path.Add(node);
                while (node.father != null)
                {
                    path.Add(node.father);
                    node = node.father;
                }
                path.Reverse();

                //初始化
                InitData();

                //返回最终路径
                Debug.Log("找到路径");
                return path;
            }
            else
            {
                return FindPath(new Vector2(node.x, node.y), end);//使用递归来继续寻路
            }
        }
        else
        {
            Debug.Log("死路一条");
            InitData();
            return null;
        }
    }

    public void InitData()
    {
        first = 0;
        foreach (var node1 in openList)
        {
            node1.father = null;
            node1.f = 0;
            node1.g = 0;
            node1.h = 0;
        }
        foreach (var node2 in closeList)
        {
            node2.father = null;
            node2.f = 0;
            node2.g = 0;
            node2.h = 0;
        }
        openList.Clear();
        closeList.Clear();
    }
}
