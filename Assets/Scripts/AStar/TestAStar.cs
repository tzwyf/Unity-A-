using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestAStar : MonoBehaviour
{
    //左上角第一个立方体的位置
    public int beginX = -3;
    public int beginY = 5;
    //之后每一个立方体之间的偏移位置
    public int offsetX = 2;
    public int offsetY = 2;
    //地图格子的宽高
    public int mapW = 5;
    public int mapH = 5;

    public Material red;
    public Material yellow;
    public Material green;
    public Material normal;

    private Dictionary<string, GameObject> cubes = new Dictionary<string, GameObject>();
    private Vector2 beginPos = Vector2.right * -1;
    private List<AStarNode> list;

    void Start()
    {
        AStarMgr.Instance.InitNodeMap(mapW, mapH);

        for(int i = 0; i < mapW; i++)
        {
            for(int j = 0; j < mapH; j++)
            {
                //创建立方体
                GameObject obj = GameObject.CreatePrimitive(PrimitiveType.Cube);
                obj.transform.position = new Vector3(beginX + i * offsetX, beginY + j * offsetY, 0) ;
                obj.name = i + "_" + j;
                cubes.Add(obj.name, obj);

                //得到格子判断是不是阻挡
                AStarNode node = AStarMgr.Instance.nodes[i, j];
                if (node.type == E_Node_Type.stop)
                {
                    obj.GetComponent<MeshRenderer>().material = red;
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //进行射线检测
            RaycastHit info;
            //得到屏幕鼠标位置射出去的射线
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //射线检测
            if(Physics.Raycast(ray, out info, 1000))
            {
                if(beginPos == Vector2.right * -1)
                {
                    if (list != null)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            cubes[list[i].x + "_" + list[i].y].GetComponent<MeshRenderer>().material = normal;
                        }
                    }

                    string[] strs = info.collider.gameObject.name.Split('_');
                    beginPos = new Vector2(int.Parse(strs[0]), int.Parse(strs[1]));
                    info.collider.gameObject.GetComponent<MeshRenderer>().material = yellow;
                }
                else
                {
                    string[] strs = info.collider.gameObject.name.Split('_');
                    Vector2 endPos = new Vector2(int.Parse(strs[0]), int.Parse(strs[1]));

                    list =  AStarMgr.Instance.FindPath(beginPos, endPos);
                    Debug.Log("list:" + list);
                    if(list != null)
                    {
                        for (int i = 0; i < list.Count; i++)
                        {
                            cubes[list[i].x + "_" + list[i].y].GetComponent<MeshRenderer>().material = green;
                        }


                    }
                    else
                    {
                        cubes[beginPos.x + "_" + beginPos.y].GetComponent<MeshRenderer>().material = normal;
                    }

                    beginPos = Vector2.right * -1;
                }
            }
        }
    }
}
