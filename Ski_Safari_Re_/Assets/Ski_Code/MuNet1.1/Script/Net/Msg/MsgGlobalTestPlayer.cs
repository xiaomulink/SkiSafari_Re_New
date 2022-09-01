using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgGlobalTestPlayer : MsgBase
{
	//设置协议名
	public MsgGlobalTestPlayer()
	{
		this.protoName = "MsgGlobalTestPlayer";
	}

    // 接下来填写参数
    public string name = "";

    public string id = "";

    public int result;
}
