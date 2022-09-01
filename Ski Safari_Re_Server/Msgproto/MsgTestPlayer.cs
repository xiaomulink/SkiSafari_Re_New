using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

class MsgTestPlayer : MsgBase
{
    public string name = "";

    public string id = "";

    public int result;


    public MsgTestPlayer()
    {
        this.protoName = "MsgTestPlayer";
    }
}
