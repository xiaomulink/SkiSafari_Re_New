using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//==========================
// - ScriptName:      MyGameInfo.cs         	
// - CreateTime:    2022/05/04 15:23:36	
// - Homepage:         http://touhouboredmutation.online/		
// - Email:         2252391595@qq.com		
// - Description:   
//==========================

public class MyGameInfo : MonoBehaviour
{
    public static string id;
    public static string N_name;
    public static int win;
    public static int lose;
    public static int coin;
    public static int roomid;

    public static string nowip = "";

    public static Player player;

    public enum RoomMode
    {
        Room=0,
        Room3RD=1
    }
    public static RoomMode roomMode;
    // 第一次运行调用的方法
    void Start()
    {
        
    }

    // 每次更新都要调用的方法
    void Update()
    {
        
    }
}
