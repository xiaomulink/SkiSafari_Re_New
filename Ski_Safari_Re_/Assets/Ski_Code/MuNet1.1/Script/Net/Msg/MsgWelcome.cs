using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgWelcome : MsgBase
{
	//设置协议名
	public MsgWelcome()
	{
		this.protoName = "MsgWelcome";
	}

    // 接下来填写参数
    public int key;
}
