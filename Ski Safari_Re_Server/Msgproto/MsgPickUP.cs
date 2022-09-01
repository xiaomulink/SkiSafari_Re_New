using System.Collections;
using System.Collections.Generic;

public class MsgPickUP : MsgBase
{
    //设置协议名
    public MsgPickUP()
    {
        this.protoName = "MsgPickUP";
    }
    public string PlayerId;
    // 接下来填写参数
    public int Itemid;
    public int RandomID;
}
