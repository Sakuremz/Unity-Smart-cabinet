using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class BoxControl : MonoBehaviour
{

    public string BoxCode;
    public GameObject BoxCodeNum;       //可以将Unity中的对象赋给GameObject类;
    public bool putortake;

    // Start is called before the first frame update  调试时自动调用
    void Start()
    {
        /*
         * 对象名称或引用名.获取对象上的组件<TextMesh>().属性 = 变量;
         * BoxCodeNum.GetComponent<组件>().属性 = 变量;       
        */
        BoxCodeNum.GetComponent<TextMesh>().text = BoxCode;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void BoxOpen()
    {
        //transform.Find("Door").GetComponent<Animator>().SetTrigger("Open");

        //找到子物体DOOR
        Transform child = transform.Find("Axis");
        //获取子物体上的动控制器组件
        Animator ani = child.GetComponent<Animator>();

        //存入或取出时判断门是否处于开启状态
        ani.SetTrigger("Open");
        /*
        * 判断动画是否播放完成
        * 如果当前动画状态不是"BoxOpen"并且动画的归一化时间大于等于1，那么表示动画已经播放完成
        * 游戏对象. 获取对象的动画状态信息(索引).名字是否是("动画名称") && 游戏对象.获取对象的动画状态信息(索引).动画播放进度大于 1
        */
        if (!ani.GetCurrentAnimatorStateInfo(0).IsName("BoxOpen") && ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            // 柜子门已经完全打开
            if (putortake)
            {
                itemPut();
            }
            else
            {
                itemTake();
            }
        }
    }

    //存入物品
    public void itemPut()
    {
        Transform item = transform.Find("Item");
        Animator putani = item.GetComponent<Animator>();
        putani.SetTrigger("Put");
        if (!putani.GetCurrentAnimatorStateInfo(0).IsName("ItemPut") && putani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            BoxClose();
        }
    }

    //取出物品
    public void itemTake()
    {
        Transform item = transform.Find("Item");
        Animator takeani = item.GetComponent<Animator>();

        if (item.position.z < -5)
        {
            //柜子没东西
            Animator box = transform.Find("Axis").GetComponent<Animator>();
            box.SetTrigger("Open");
            takeani.SetTrigger("Put");
            box.SetTrigger("Close");
        }
        else
        {
            takeani.SetTrigger("Stay");
        }
        if (!takeani.GetCurrentAnimatorStateInfo(0).IsName("ItemStay") && takeani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            BoxClose();
        }

    }

    //关闭柜子
    public void BoxClose()
    {
        transform.Find("Axis").GetComponent<Animator>().SetTrigger("Close");
    }
}
