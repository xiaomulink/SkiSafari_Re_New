using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgPing : MsgBase
{
	//设置协议名
	public MsgPing()
	{
		this.protoName = "MsgPing";
	}

    // 接下来填写参数
    public string return_="";
}
