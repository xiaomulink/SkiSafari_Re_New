using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//==========================
// - ScriptName:      Test_Ping.cs         	
// - CreateTime:    2021/07/17 15:38:30	
// - Homepage:         http://touhouboredmutation.online/		
// - Email:         2252391595@qq.com		
// - Description:   
//==========================

using UnityEngine;
using System.Collections;

public class _Ping : MonoBehaviour
{

    public string IP = "";
    Ping ping;
    float delayTime;

    void Start()
    {
        SendPing();
    }

    void OnGUI()
    {
        GUI.color = Color.red;
        GUI.Label(new Rect(10, 10, 100, 20), "ping: " + delayTime.ToString() + "ms");

        if (null != ping && ping.isDone)
        {
            delayTime = ping.time;
            ping.DestroyPing();
            ping = null;
            Invoke("SendPing", 1.0F);//每秒Ping一次
        }
    }

    void SendPing()
    {
        //Debug.LogError(delayTime.ToString() + "ms");
        ping = new Ping(IP);
    }
}
