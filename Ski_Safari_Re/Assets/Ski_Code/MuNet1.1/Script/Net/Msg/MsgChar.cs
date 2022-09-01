using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgChar : MsgBase
{
	//设置协议名
	public MsgChar()
	{
		this.protoName = "MsgChar";
	}

    // 接下来填写参数
    public int type;//0=set,1=get
    public string char_name;
}
