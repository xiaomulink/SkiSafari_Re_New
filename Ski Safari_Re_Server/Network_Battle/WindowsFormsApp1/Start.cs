using System;

namespace Netwolk_Battle
{
	public class Start
	{
        public bool isOn=false;
        public int post = 10021;
        public string ip = "0.0.0.0";
        public Start(int _post= 10021,string _ip="0.0.0.0")
        {
            post = _post;
            ip = _ip;
        }

        public bool GetServerisOn()
        {
            return isOn;
        }
		public void stop()
		{
			NetManager.listenfd.Close();
			NetManager.isclose = true;
            isOn = false;
        }

        public void start()
        {
            CreateRoom createroom = new CreateRoom();
            createroom.Show();
            NetManager.StartLoop(post, ip);

            NetManager.isclose = false;
            isOn = true;
        }
	}
}
