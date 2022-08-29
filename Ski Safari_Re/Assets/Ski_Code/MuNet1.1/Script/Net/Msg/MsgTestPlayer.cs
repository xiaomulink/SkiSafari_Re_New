using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgTestPlayer : MsgBase
{
	//设置协议名
	public MsgTestPlayer()
	{
		this.protoName = "MsgTestPlayer";
	}

    // 接下来填写参数
    public string name = "";

    public string id = "";

    public int result;
}
