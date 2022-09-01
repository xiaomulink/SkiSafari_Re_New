using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgGlobalWelcome : MsgBase
{
	//设置协议名
	public MsgGlobalWelcome()
	{
		this.protoName = "MsgGlobalWelcome";
	}
    // 接下来填写参数
    public int key;
}
