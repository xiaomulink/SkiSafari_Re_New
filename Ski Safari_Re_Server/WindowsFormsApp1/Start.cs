using System;

namespace Network
{
	public class Start
	{
        public bool isOn=false;
        public int post = 10021;
        public Start(int _post= 10021)
        {
            post = _post;
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
			if (DbManager.Connect("game", "127.0.0.1", 3306, "root", "", "name"))
			{
				NetManager.versionsvalue = DbManager.Getvalue();
                Main.main.serverData = new ServerData();
       
                NetManager.StartLoop(post);
                ReadLog._ReadLog(DbManager.Getvalue().ToString(), null, true);
				NetManager.isclose = false;
                isOn = true;
              

            }
        }
       
    }
}
