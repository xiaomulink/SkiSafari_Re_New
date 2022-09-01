using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using UnityEngine;

//==========================
// - ScriptName:      GameMain.cs         	
// - CreateTime:    2022/05/04 16:17:24	
// - Homepage:         http://touhouboredmutation.online/		
// - Email:         2252391595@qq.com		
// - Description:   
//==========================

public class GameMain : MonoBehaviour
{
    public bool isrun = false;
    // 第一次运行调用的方法
    async void Start()
    {
        GameObject[] main = GameObject.FindGameObjectsWithTag("GameMain");
        if (GameObject.FindGameObjectsWithTag("GameMain").Length > 1)
        {
            foreach (GameObject go in main)
            {
                if (go.GetComponent<GameMain>().isrun == true)
                {
                    Destroy(go);
                }
            }
        }
        PanelManager.PanelClear();
        PanelManager.Clear();
        NetManager.Connect("127.0.0.1");
        //PanelManager.Open<QuickRoomPanel>();

        NetManager.AddEventListener(NetManager.NetEvent.Close, OnConnectClose);

        NetManager.AddEventListener(NetManager.NetEvent.ConnectSucc, new NetManager.EventListener(OnConnectSucc));
        NetManager.AddEventListener(NetManager.NetEvent.ConnectFail, new NetManager.EventListener(OnConnectFail));

        NetWebServerManager.AddEventListener(NetWebServerManager.NetEvent.Close, OnConnectClose);

        NetWebServerManager.AddEventListener(NetWebServerManager.NetEvent.ConnectSucc, new NetWebServerManager.EventListener(OnConnectSucc));
        NetWebServerManager.AddEventListener(NetWebServerManager.NetEvent.ConnectFail, new NetWebServerManager.EventListener(OnConnectFail));

        NetManager.AddMsgListener("MsgWelcome", OnMsgWelcome);

        NetManager.AddMsgListener("MsgKick", OnMsgKick);
        NetWebServerManager.AddMsgListener("MsgKick", OnMsgKick);

        await Task.Delay(500);
        PanelManager.Init();

        PanelManager.Open<ChatPanel>();
        PanelManager.Open<LoginPanel>();

        PanelManager.Open<Tip>(PanelManager.UIstyle.Default, "公告 \n 没有什么公告");

        DontDestroyOnLoad(gameObject);
        isrun = true;
    }

    public void OnApplicationQuit()
    {
        NetManager.Close();
        NetManager.RemoveEventListener(NetManager.NetEvent.ConnectSucc, new NetManager.EventListener(OnConnectSucc));
        NetManager.RemoveEventListener(NetManager.NetEvent.ConnectFail, new NetManager.EventListener(OnConnectFail));
        NetWebServerManager.Close();
        NetWebServerManager.RemoveEventListener(NetWebServerManager.NetEvent.ConnectSucc, new NetWebServerManager.EventListener(OnConnectSucc));
        NetWebServerManager.RemoveEventListener(NetWebServerManager.NetEvent.ConnectFail, new NetWebServerManager.EventListener(OnConnectFail));
      
    }

    public static void Close()
    {
        NetManager.Close();
        NetManager.RemoveEventListener(NetManager.NetEvent.ConnectSucc, new NetManager.EventListener(OnConnectSucc));
        NetManager.RemoveEventListener(NetManager.NetEvent.ConnectFail, new NetManager.EventListener(OnConnectFail));
        NetWebServerManager.Close();
        NetWebServerManager.RemoveEventListener(NetWebServerManager.NetEvent.ConnectSucc, new NetWebServerManager.EventListener(OnConnectSucc));
        NetWebServerManager.RemoveEventListener(NetWebServerManager.NetEvent.ConnectFail, new NetWebServerManager.EventListener(OnConnectFail));
    }

    public void OnMsgWelcome(MsgBase msgBase)
    {
        MsgWelcome msg = (MsgWelcome)msgBase;
        Debug.LogError("Welcome!");
    }

    // Token: 0x0600073B RID: 1851 RVA: 0x00030EE8 File Offset: 0x0002F0E8
    private static void OnConnectSucc(string err)
    {
        Debug.Log("OnConnectSucc");
    }

    // Token: 0x0600073C RID: 1852 RVA: 0x00030EF4 File Offset: 0x0002F0F4
    private static void OnConnectFail(string err)
    {
        Debug.Log("OnConnectFail");

    }
    // Token: 0x06000514 RID: 1300 RVA: 0x00026AAC File Offset: 0x00024CAC
    private static void OnConnectClose(string err)
    {

    }

    // Token: 0x06000515 RID: 1301 RVA: 0x00026AB0 File Offset: 0x00024CB0
    private void OnMsgKick(MsgBase msgBase)
    {
        PanelManager.Open<Tip>(PanelManager.UIstyle.Default, new object[]
        {
            "你的账号在别地登录，请重新登录"
        });
        Loading.Load("Login");
    }


    // 每次更新都要调用的方法
    void Update()
    {
        NetManager.Update();
        NetWebServerManager.Update();
    }
}
