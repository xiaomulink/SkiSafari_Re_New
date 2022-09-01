using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgPeertopeerServer : MsgBase
{
	//设置协议名
	public MsgPeertopeerServer()
	{
		this.protoName = "MsgPeertopeerServer";
	}

    // 接下来填写参数
    public int Type;//0 Server,1 Client
    public string PackageInfo;
    public string Ip;
    public int port;
}
