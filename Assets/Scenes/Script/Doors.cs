using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doors : MonoBehaviour
{
    bool Click = false;

    private void OnMouseDown()
    {
        //transform.parent.GetComponent<Animator>().SetTrigger("Close");
        //获取父对象Axis
        Transform t = transform.parent;
        //获取父对象的Animator组件
        Animator anim = t.GetComponent<Animator>();
        //改变参数值---->True <>False

        if (Click == true)
        {
            anim.SetTrigger("Open");
            Click = false;
        }
        else
        {
            anim.SetTrigger("Close");
            Click = true;
        }


    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
