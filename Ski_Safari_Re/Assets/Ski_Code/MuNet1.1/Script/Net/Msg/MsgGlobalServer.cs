using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MsgGlobalServer : MsgBase
{
    //设置协议名
    public MsgGlobalServer()
    {
        this.protoName = "MsgGlobalServer";
    }

    // 接下来填写参数
    public string ip;

    public string playernub;
    public string serverdescription;
    public string serverping;
}
