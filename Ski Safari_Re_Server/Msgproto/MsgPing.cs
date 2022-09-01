using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class MsgPing : MsgBase
{
    //设置协议名
    public MsgPing()
    {
        this.protoName = "MsgPing";
    }

    // 接下来填写参数
    public string return_;
}