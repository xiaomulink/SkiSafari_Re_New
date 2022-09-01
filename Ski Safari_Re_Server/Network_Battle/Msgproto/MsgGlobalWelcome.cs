using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class MsgGlobalWelcome : MsgBase
{
    //设置协议名
    public MsgGlobalWelcome()
    {
        this.protoName = "MsgGlobalWelcome";
    }

    // 接下来填写参数

    public int key;
}

