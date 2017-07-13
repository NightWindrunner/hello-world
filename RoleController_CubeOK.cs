using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using UnityEngine.SceneManagement;


public class RoleController_CubeOK : MonoBehaviour
{
    Text tt;
    GameObject baseobj;
    Vector3 child;
    Rigidbody body;
    Material material;
    Transform[] childpos;
    Vector3 originpos;
    Vector3 originpos1;
    bool isStart = true;
    bool end = true;
    bool Isanchor = true;
    bool Isanchor1 = true;
    bool xunhuan = true;
    bool SceondRun;
    int num = 0;
    int num1 = 0;
    int countn = 0;
    int a = 0;
    int chang = 331;//原来长度是431
    int kuang = 100;
    int kuang1 = 175;
    int count;
    int result = 1;
    
    string posstations = "fildAddress";
    List<int> Aresult = new List<int>();
    List<string> Aname = new List<string>();
    Dictionary<string, int> dic = new Dictionary<string, int>();
    Dictionary<string, int> dicmax = new Dictionary<string, int>();

    public float distance = 1.5f;//设置方块离地面高度
    private float targetheight;//离地高度所对应的世界坐标
    public GameObject greencube;//绿色方块预制体
    public GameObject redcube;//红色方块预制体
    public float Minjizhan = 6;//设置接收的最小基站数量
    public int cubeturn = 2;

    void Start()
    {
        InitInfo();
    }
    void InitInfo()
    {

        tt = GameObject.Find("test").GetComponent<Text>();
        material = Instantiate(Resources.Load("green")) as Material;
        baseobj = GameObject.Find("BaseStation");
        body = GetComponent<Rigidbody>();
        originpos = new Vector3(-74f, 6f, -68f);
        originpos1 = new Vector3(-24f, 6f, -307f);//原来位置是（-24，6，-207）
        // var fildAddress = Path.Combine("Assets", "posstations" + ".txt");
        // StreamWriter sw1 = new StreamWriter(fildAddress,false);//覆盖文档
        StartCoroutine(ChangPos());
    }

    IEnumerator ChangPos()
    {
        int i = 0;
        int j = 0;
        int k = 0;

        if (end)
        {
            result++;
            for (; i < chang; i++)
            {
                Vector3 pos = new Vector3();
                pos.z = originpos.z;
                pos.y = 15f;
                if (j >= kuang)
                {
                    //Debug.Log("jjjjjj" + j);
                    pos.z = originpos.z - i;
                    //Debug.Log("zzzzzz" + pos);
                    transform.position = new Vector3(originpos.x, 10f, pos.z);
                    j = 0;
                }
                if (i < 140)
                {
                    for (; j < kuang; j++)
                    {
                        Ray ray = new Ray(transform.position, Vector3.down);
                        RaycastHit hit;
                        pos.x = originpos.x - j;
                        //Debug.Log("xxxxxxx" + pos);
                        if (Physics.Raycast(ray, out hit))
                        {
                            targetheight = hit.point.y + distance;
                            pos.y = targetheight;
                        }
                        transform.position = pos;

                        if (result ==cubeturn+1)//第二遍循环生成方块
                        {
                        if (num1 < Minjizhan)
                            Instantiate(redcube,pos,Quaternion.identity);
                        else
                            Instantiate(greencube, pos, Quaternion.identity);
                        }

                        Condition();
                        yield return new WaitForSeconds(0f);
                    }
                }

                transform.position = originpos1;
                if (k >= kuang1)
                {
                    pos.z = originpos.z - i-100;
                    transform.position = new Vector3(originpos1.x, 10.0f, pos.z);
                    k = 0;
                }
                if (i >= 140)
                {
                    for (; k < kuang1; k++)
                    {
                        pos.x = originpos1.x - k;
                        pos.y = 10.0f;
                        pos.z = originpos.z - i - 100;
                        Ray ray = new Ray(transform.position, Vector3.down);//向下发射射线控制距离地面高度
                        RaycastHit hit;
                        
                        if (Physics.Raycast(ray, out hit))
                        {
                            targetheight = hit.point.y + distance;
                            pos.y = targetheight;
                        }
                        transform.position = pos;
                        if (result == cubeturn+1)//第二遍循环生成方块
                        {
                        if (num1 < Minjizhan)
                            Instantiate(redcube, pos, Quaternion.identity);
                        else
                            Instantiate(greencube, pos, Quaternion.identity);
                        }

                        Condition();
                        yield return new WaitForSeconds(0f);
                    }
                }
                yield return new WaitForSeconds(0f);
            }
        }
        end = false;
    }

 

    void LateUpdate()
    {
        GetChilds();
        if (end == false)
        {
            AnchorNum();
            AnameNum();
        }

    }

    void GetChilds()//计算Anchor的可用度
    {
        count = baseobj.transform.childCount;
        childpos = new Transform[count];
        if (isStart)
        {
            for (int i = 0; i < count; i++)
            {
                if (!baseobj.transform.GetChild(i).gameObject.GetComponent<LineRenderer>())
                {
                    baseobj.transform.GetChild(i).gameObject.AddComponent<LineRenderer>();
                }
                childpos[i] = baseobj.transform.GetChild(i).transform;
            }
            for (int j = 0; j < count; j++)
            {
                if (Vector3.Distance(transform.position, childpos[j].position) <= 100f)
                {
                    num++;
                    OnViewXian(childpos[j]);
                    tt.text += "Anchor:" + childpos[j].name + "\t\t";
                }
            }
            Aresult.Add(num1);
            tt.text += "\n100米包含Anchor数量:" + num.ToString() + "\n";
            tt.text += "无遮挡Anchor数量:" + num1.ToString() + "\n";
            tt.text += "有遮挡的Anchor数量:" + (num - num1).ToString();
            WritePos();
            isStart = false;
        }
    }

    void OnViewXian(Transform pos)//画线
    {
        Vector3 direc = transform.position - pos.position;
        Ray ray = new Ray(pos.position, direc.normalized);
        int layerMask = ~(1 << 8);
        RaycastHit hit;
        bool iscollider = Physics.Raycast(ray,out hit, 100.0f,layerMask);
        Debug.DrawLine(pos.position, transform.position, Color.red);
        pos.gameObject.GetComponent<LineRenderer>().material = material;
        pos.gameObject.GetComponent<LineRenderer>().SetColors(Color.red, Color.red);
        pos.gameObject.GetComponent<LineRenderer>().SetWidth(0.05f, 0.05f);
        pos.gameObject.GetComponent<LineRenderer>().SetPosition(0, transform.position);
        pos.gameObject.GetComponent<LineRenderer>().SetPosition(1, pos.position);
        int num = 0;
        if (iscollider && hit.collider.tag == "JiZhan")
        {
            num1++;
            tt.text += "无遮挡Anchor:" + pos.name + "\t";
            Debug.DrawLine(pos.position, transform.position, Color.green);
            pos.gameObject.GetComponent<LineRenderer>().material = material;
            pos.gameObject.GetComponent<LineRenderer>().SetColors(Color.green, Color.green);
            pos.gameObject.GetComponent<LineRenderer>().SetWidth(0.05f, 0.05f);
            pos.gameObject.GetComponent<LineRenderer>().SetPosition(0, transform.position);
            pos.gameObject.GetComponent<LineRenderer>().SetPosition(1, pos.position);
            Aname.Add(pos.name);
        }
    }

    void AnameNum()//计算全场每个无遮挡anchor的出现数量
    {
        if (Isanchor1)
        {
            Isanchor1 = false;
            Aname.Sort();
            if (Aname.Count > 0)
            {

                for (int i = 0; i < Aname.Count; i++)
                {
                    if (i == Aname.Count - 1)
                    {
                        break;
                    }
                    else
                    {
                        if (Aname[i].Equals(Aname[i + 1]))
                        {
                            countn++;
                        }

                        else
                        {
                            dic.Add(Aname[i], (countn + 1));
                            countn = 0;
                        }
                    }
                }
                AnchorSort();
            }
            Aresult.Clear();
            Aname.Clear();
            dic.Clear();
            dicmax.Clear();


            if (result <= 2)
            {
                end = true;
                Isanchor = true;
            }
            else
            {
                end = false;
                Isanchor = false;
            }
            StartCoroutine(ChangPos());
            
           
        }
    }

    void AnchorSort()//键/值排序方法
    {
        int index = 0;//原来是1，应该设置为0
        var dicsort = from objDic in dic orderby objDic.Value descending select objDic;//linq排序
        foreach (KeyValuePair<string, int> item in dicsort)
        {
            index++;
            if (index <= 50)
            {
                tt.text = "全场每个Anchor的出现数量：" + item.Key + ":" + item.Value;
            }
            if (index > 50)
            {
                if (index == 51)
                {
                    tt.text = "\n" + "全场每个Anchor的出现数量：" + item.Key + ":" + item.Value;
                }
                else
                {
                    tt.text = "全场每个Anchor的出现数量：" + item.Key + ":" + item.Value;
                }
                GameObject obj = baseobj.transform.Find(item.Key).gameObject;
                Destroy(obj);
            }
            WritePos();
        }
    }

    void AnchorNum()//Anchor的数量比较     
    {
        if (Isanchor)
        {
            Isanchor = false;
            for (int i = 0; i < Aresult.Count; i++)
            {
                if (Aresult[i] < 6)
                {
                    a++;
                }
            }
            tt.text = "全场无遮挡小于6的Anchor：" + a;
            WritePos();
            Isanchor1 = true;
        }
    }

    void Condition()
    {
        tt.text = "";
        num = 0;
        num1 = 0;
        isStart = true;
    }

    void WritePos()//生成text文档记录
    {

        var fildAddress = Path.Combine("Assets", "posstations" + ".txt");
        if (!File.Exists(fildAddress))
        {
            StreamWriter sw = new StreamWriter(fildAddress);//新建文件
            string w = tt.text + "\n";
            print(w);
            sw.Write(w);
            sw.Close();
        }
        else
        {
            StreamWriter sw = File.AppendText(fildAddress);//追加新的内容在文件后而非覆盖
            string w = "\n" + tt.text;
            sw.Write(w);
            sw.Close();
        }
    }
}
