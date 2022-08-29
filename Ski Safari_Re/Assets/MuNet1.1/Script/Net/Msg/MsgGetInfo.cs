using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgGetInfo : MsgBase
{

    public string id = "";

    public string name = "";

    public int win = 0;

    public int lost = 0;

    public int coin = 0;

    public int result;
    //设置协议名
    public MsgGetInfo()
	{
		this.protoName = "MsgGetInfo";
	}
   
    // 接下来填写参数
    
}
