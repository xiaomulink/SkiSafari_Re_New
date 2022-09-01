using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class MsgGetInfo : MsgBase
{
    public string id = "";

    public string name = "";

    public int win = 0;

    public int lost = 0;

    public int coin = 0;

    public int result;

    public MsgGetInfo()
    {
        this.protoName = "MsgGetInit";
    }
}

