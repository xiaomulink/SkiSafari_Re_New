using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


//==========================
// - ScriptName:      SyncObj.cs         	
// - CreateTime:    2021/09/30 13:09:27	
// - Homepage:         http://touhouboredmutation.online/		
// - Email:         2252391595@qq.com		
// - Description:   物体同步
//==========================

public class SyncObj 
{
    public int Objid;//同步物体Id

    //位置x
    public double x;

    //位置y
    public double y;

    //位置z
    public double z;

    //旋转x
    public double ex;

    //旋转y
    public double ey;

    //旋转z
    public double ez;

    public string PlayerId;//拥有者Id

    public SyncObj(int id, string playerId)
    {
        Objid = id;
       
        PlayerId = playerId;
    }
    
}

