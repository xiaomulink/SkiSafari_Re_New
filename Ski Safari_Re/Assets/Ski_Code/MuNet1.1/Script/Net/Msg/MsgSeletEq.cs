using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgSeletEq : MsgBase
{
	//设置协议名
	public MsgSeletEq()
	{
		this.protoName = "MsgSeletEq";
	}
    // 接下来填写参数
    public int id;
    public string type;//0=char,1=bullet
}

