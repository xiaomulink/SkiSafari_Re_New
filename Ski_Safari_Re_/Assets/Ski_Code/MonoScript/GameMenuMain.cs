using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//==========================
// - ScriptName:      GameMenuMain.cs         	
// - CreateTime:    #CreateTime#	
// - Homepage:         #AuthorHomepage#		
// - Email:         #AuthorEmail#		
// - Description:   
//==========================

public class GameMenuMain : MonoBehaviour
{
    // 第一次运行调用的方法
    void Start()
    {
        PanelManager.PanelClear();
        PanelManager.Clear();
    }

    // 每次更新都要调用的方法
    void Update()
    {
        
    }
}
