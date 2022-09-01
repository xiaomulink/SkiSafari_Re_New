using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//==========================
// - ScriptName:      MapBattle.cs         	
// - CreateTime:    2021/07/20 10:32:31	
// - Homepage:         http://touhouboredmutation.online/		
// - Email:         2252391595@qq.com		
// - Description:   
//==========================


public class MapBattle : MonoBehaviour
{
    public Transform[] buleBirthpoint;
    public Transform[] redBirthpoint;

    // 第一次运行调用的方法
    void Start()
    {
        //BattleManager.EnterBattle(RoomPanel.Battleinfo); 
    }

    // 每次更新都要调用的方法
    void Update()
    {
        
    }
}
