using LitJson;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

class GetServer
{
    class ServerData
    {
        public string playernub;
        public string serverdescription;
        public string serverping;
    }
    public static object httpevent(object obj)
    {

        string Request = "";
        HttpListenerContext ctx = (HttpListenerContext)obj;
        switch (ctx.Request.HttpMethod)
        {
            case "GET":
                {

                    ServerData personList =new ServerData();
                    personList.playernub = RoomManager.GetRoom(1).playerIds.Count + "/" + RoomManager.GetRoom(1).maxPlayer;
                    personList.serverdescription = RoomManager.GetRoom(1).describe;
                    personList.serverping = "6ms";


                    string json = JsonMapper.ToJson(personList);
                  

                    Request = json;
                }
                break;
        }
     
        return Request;
    }
}

