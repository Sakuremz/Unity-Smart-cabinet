using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO.Ports;
using System;
using UnityEngine.UIElements;
using UnityEngine.Animations;

public class SerialManager : MonoBehaviour
{
    static SerialPort _serialPort;

    byte[] readBuffer;
    int rdIndex = 0;
    bool isPacketStart = false;
    
    // Start is called before the first frame update
    void Start()
    {
        
        _serialPort = new SerialPort();
        _serialPort.PortName = "COM4";
        _serialPort.BaudRate = 9600;
        _serialPort.Parity = Parity.None;
        _serialPort.DataBits = 8;
        _serialPort.StopBits = StopBits.One;
        _serialPort.ReadTimeout = 500;
        _serialPort.WriteTimeout = 500;

        Open();
        readBuffer = new byte[4];

    }

    public void Open()
    {
        _serialPort.Open();
    }

    public void Close()
    {
        _serialPort.Close();
    }

    //发送返回数据，判断是否成功存取

    public void Send(byte cmd, byte boxCode)
    {
        //发送信息
        byte[] sendBuffer = new byte[4] { 0xFF, 0x00, 0x00, 0xFE };
        sendBuffer[1] = cmd;
        sendBuffer[2] = boxCode;
        //              Write(字节流, 偏移量, 发送的字节数量)
        _serialPort.Write(sendBuffer, 0, sendBuffer.Length);
    }

    private void Update()
    {
        int receiveData = 0;
        if (_serialPort.BytesToRead > 0)
        {
            receiveData = _serialPort.ReadByte();

            if (!isPacketStart && receiveData == 0xFF)
            {
                // 接收到数据包的起始标识位
                isPacketStart = true;
                rdIndex = 0;
            }

            if (isPacketStart)
            {
                readBuffer[rdIndex] = (byte)receiveData;
                rdIndex++;

                if (rdIndex >= 4)
                {

                    // 数据包接收完成
                    isPacketStart = false;
                    ProcessPacket();
                }
            }
        }
/*        for (int i = 0; i < readBuffer.Length; i++)
        {
            Debug.Log("第" + i + "个元素是：" + readBuffer[i]);
        }*/
    }

    private void ProcessPacket()
    {
        // 处理数据包
        if (readBuffer[0] == 0xFF && readBuffer[readBuffer.Length - 1] == 0xFE)
        {
            //找到所有Lockerbox脚本
            BoxControl[] boxList = GameObject.FindObjectsOfType<BoxControl>();
            // 数据包的校验成功
            if (readBuffer[2] == 0xAF)
            {
                int boxCode = readBuffer[1];
                //打开boxCode对应的柜子

                //找到要打开的柜子
                foreach (BoxControl box in boxList)
                {
                    string _boxCode = box.BoxCode;
                    int _boxCodeInt = int.Parse(_boxCode);
                    if (boxCode == _boxCodeInt)
                    {
                        //存入物品=TRUE
                        box.putortake = true;

                        //打开柜子
                        box.BoxOpen();

                        //返回存储结果
                        Send((byte)_boxCodeInt, 0xAC);
                    }
                }
                // 其他逻辑处理...
            }
            //取出操作
            else if (readBuffer[2] == 0xAE)
            {
                int boxCode = readBuffer[1];
                //打开boxCode对应的柜子

                //找到要打开的柜子
                foreach (BoxControl box in boxList)
                {
                    string _boxCode = box.BoxCode;
                    int _boxCodeInt = int.Parse(_boxCode);
                    if (boxCode == _boxCodeInt)
                    {
                        //取出物品=FALSE
                        box.putortake = false;

                        //打开柜子
                        box.BoxOpen();

                        //返回取出结果
                        Send((byte)_boxCodeInt, 0xAD);
                    }
                }
            }
            else if (readBuffer[2] == 0xAB)
            {
                int boxCode = readBuffer[1];
                //将对应的物品放入柜子

                //找到对应的柜子
                foreach (BoxControl box in boxList)
                {
                    string _boxCode = box.BoxCode;
                    int _boxCodeInt = int.Parse(_boxCode);
                    if (boxCode == _boxCodeInt)
                    {
                        box.itemTake();
                    }
                }
            }
        }
        // 清空读到的数据
        Array.Clear(readBuffer, 0, readBuffer.Length);
    }
}