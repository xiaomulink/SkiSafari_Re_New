using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgShop : MsgBase
{
	//设置协议名
	public MsgShop()
	{
		this.protoName = "MsgShop";
	}

    // 接下来填写参数
    public int coin;
    public int itemid;
    public int itemnub;
    public int itemtype;//0=null,1=char,2=bullet
    public int result;
}
