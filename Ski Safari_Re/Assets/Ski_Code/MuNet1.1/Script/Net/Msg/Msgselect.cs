using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Msgselect : MsgBase
{
	//设置协议名
	public Msgselect()
	{
		this.protoName = "Msgselect";
	}

    // 接下来填写参数
    public int type;//0=get,1=set
    public int itemtype;//0=char,1=weapon,2=item
    public string item;
}
