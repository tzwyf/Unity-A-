using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum E_Node_Type
{
    walk,//正常
    stop//阻挡
}

/// <summary>
/// 格子类
/// </summary>
public class AStarNode
{
    //格子坐标
    public int x;
    public int y;

    //格子消耗
    public float f;
    public float g;
    public float h;
    //父对象
    public AStarNode father;
    //格子类型
    public E_Node_Type type;

    public AStarNode(int x,int y,E_Node_Type type)
    {
        this.x = x;
        this.y = y;
        this.type = type;
    }
}
