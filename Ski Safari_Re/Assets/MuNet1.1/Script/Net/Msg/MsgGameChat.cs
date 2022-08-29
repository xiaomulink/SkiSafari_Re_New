using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgGameChat : MsgBase
{
	//设置协议名
	public MsgGameChat()
	{
		this.protoName = "MsgGameChat";
	}

    // 接下来填写参数
    public int ChatType;//0 System,1 Player

    public string Id;
    public string Name;
    public string Chat;
}
