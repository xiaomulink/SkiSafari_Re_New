using System;
using System.Collections.Generic;

public class Room
{
	public enum Status
	{
		PREPARE,
		FIGHT,
        OVER
	}

	public int id;

	public int Roomgamemode;

	public int maxPlayer = 6;
	public float aimTime =180;

    public string describe = "";

    public int aim = 10;

    // 开始的时间戳
    public long lasttime;

    public int nub;

	public Dictionary<string, bool> playerIds = new Dictionary<string, bool>();
    public List<SyncObj> SyncObj = new List<SyncObj>();

	public string ownerId = "";

	public Room.Status status;

	private static float[,,] birthConfig = new float[,,]
	{
		{
			{
				-15f,
				1f,
				0f,
				0f,
				0f,
				0f
			},
			{
				-15f,
				-0.5f,
				0f,
				0f,
				0f,
				0f
			},
			{
				-15f,
				-1.5f,
				0f,
				0f,
				0f,
				0f
			}
		},
		{
			{
				18f,
				1f,
				0f,
				0f,
				0f,
				0f
			},
			{
				18f,
				-0.5f,
				0f,
				0f,
				0f,
				0f
			},
			{
				18f,
				-1.5f,
				0f,
				0f,
				0f,
				0f
			}
		}
	};

	private long lastjudgeTime;

	public bool AddPlayer(string id)
	{
		Player player = PlayerManager.GetPlayer(id);
		bool flag = player == null;
		bool result;
		if (flag)
		{
			ReadLog._ReadLog("房间玩家为空", null, true);
			result = false;
		}
		else
		{
			bool flag2 = this.playerIds.Count >= this.maxPlayer;
			if (flag2)
			{
				ReadLog._ReadLog("房间人数已满", null, true);
				result = false;
			}
			else
			{
				bool flag3 = id != "1";
				if (flag3)
				{
					/*bool flag4 = this.status > Room.Status.PREPARE;
					if (flag4)
					{
						ReadLog._ReadLog("房间已经开始", null, true);
						return true;
					}*/
					bool flag5 = this.playerIds.ContainsKey(id);
					if (flag5)
					{
						ReadLog._ReadLog("已经在房间里", null, true);
						return false;
					}
				}
				this.playerIds[id] = true;
				player.camp = this.SwitchCamp();
				player.roomId = this.id;
				bool flag6 = this.ownerId == "";
                player.isbattle = 0;
				if (flag6)
				{
					this.ownerId = player.id;
				}
				this.Broadcast(this.ToMsg());
				result = true;
			}
		}
		return result;
	}

    public bool AddSyncObj(SyncObj so)
    {
        try
        {
            SyncObj.Add(so);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public SyncObj FindSyncObj(int soid)
    {
        foreach(SyncObj s in SyncObj)
        {
            if (s.Objid == soid)
                return s;
            else
                return null;
        }
        return null;
    }

	public int SwitchCamp()
	{
		int num = 0;
		int num2 = 0;
		foreach (string current in this.playerIds.Keys)
		{
			Player player = PlayerManager.GetPlayer(current);
			bool flag = player.camp == 1;
			if (flag)
			{
				num++;
			}
			bool flag2 = player.camp == 2;
			if (flag2)
			{
				num2++;
			}
		}
		bool flag3 = num <= num2;
		int result;
		if (flag3)
		{
			result = 1;
		}
		else
		{
			result = 2;
		}
		return result;
	}

	public bool isOwner(Player player)
	{
		return player.id == this.ownerId;
	}

	public bool RemovePlayer(string id)
	{
		Player player = PlayerManager.GetPlayer(id);
		bool flag = player == null;
		bool result;
		if (flag)
		{
			ReadLog._ReadLog("房间玩家为空", null, true);
			result = false;
		}
		else
		{
			bool flag2 = !this.playerIds.ContainsKey(id);
			if (flag2)
			{
				ReadLog._ReadLog("此玩家不在房间里", null, true);
				result = false;
			}
			else
			{
				this.playerIds.Remove(id);
				player.camp = 0;
				player.roomId = -1;
				bool flag3 = this.ownerId == player.id;
				if (flag3)
				{
					this.ownerId = this.SwitchOwner();
				}
				bool flag4 = this.status == Room.Status.FIGHT;
				if (flag4)
				{
					//player.data.lost++;
					this.Broadcast(new MsgLeaveBattle
					{
						id = player.id
					});
				}
				bool flag5 = this.playerIds.Count == 0;
				/*if (flag5)
				{
					RoomManager.RemoveRoom(this.id);
                    ReadLog._ReadLog(this.id + "号房间由于没有人被删除了");
				}*/
				this.Broadcast(this.ToMsg());
				result = true;
			}
		}
		return result;
	}

	public string SwitchOwner()
	{
		using (Dictionary<string, bool>.KeyCollection.Enumerator enumerator = this.playerIds.Keys.GetEnumerator())
		{
			if (enumerator.MoveNext())
			{
				return enumerator.Current;
			}
		}
		return "";
	}

	public void Broadcast(MsgBase msg)
	{
		foreach (string current in this.playerIds.Keys)
		{
			Player player = PlayerManager.GetPlayer(current);
			player.Send(msg);
		}
	}

	public MsgBase ToMsg()
	{
		MsgGetRoomInfo msgGetRoomInfo = new MsgGetRoomInfo();
		int count = this.playerIds.Count;
		msgGetRoomInfo.players = new Server_PlayerInfo[count];
		int num = 0;
		foreach (string current in this.playerIds.Keys)
		{
			Player player = PlayerManager.GetPlayer(current);
            Server_PlayerInfo playerInfo = new Server_PlayerInfo();
			playerInfo.id = player.id;
			playerInfo.name = player.name;
			playerInfo.camp = player.camp;
			try
			{
				playerInfo.win = player.data.win;
				playerInfo.lost = player.data.lost;
			}
			catch
			{
			}

			playerInfo.isOwner = 0;
			playerInfo.roomid = this.id;
            ReadLog._ReadLog("Playerer" + playerInfo.id);
            ReadLog._ReadLog("Playerer" + playerInfo.name);
            ReadLog._ReadLog("Playerer" + this.id);

            bool flag = this.isOwner(player);
			if (flag)
			{
				playerInfo.isOwner = 1;
			}
			msgGetRoomInfo.players[num] = playerInfo;
			num++;
		}
        ReadLog._ReadLog("Playerer" + msgGetRoomInfo.players.Length);
		return msgGetRoomInfo;
	}

	public bool CanStartBattle()
	{
		bool flag = this.status > Room.Status.PREPARE;
		bool result;
		if (flag)
		{
            int num = 0;
            int num2 = 0;
            foreach (string current in this.playerIds.Keys)
            {
                Player player = PlayerManager.GetPlayer(current);
                bool flag2 = player.camp == 1;
                if (flag2)
                {
                    num++;
                }
                else
                {
                    num2++;
                }
            }
            bool flag3 = num <= 0 && num2 <= 0;
            result = !flag3;
		}
		else
		{
			int num = 0;
			int num2 = 0;
			foreach (string current in this.playerIds.Keys)
			{
				Player player = PlayerManager.GetPlayer(current);
				bool flag2 = player.camp == 1;
				if (flag2)
				{
					num++;
				}
				else
				{
					num2++;
				}
			}
			bool flag3 = num <= 0 && num2 <= 0;
			result = !flag3;
		}
		return result;
	}

	private void SetBirthPos(Player player, int index)
	{
		int camp = player.camp;
		player.x = Room.birthConfig[camp - 1, index, 0];
		player.y = Room.birthConfig[camp - 1, index, 1];
		player.z = Room.birthConfig[camp - 1, index, 2];
	}

	public static HumanInfo PlayerToHumanInfo(Player player)
	{
		HumanInfo humanInfo = new HumanInfo();
		humanInfo.camp = player.camp;
		humanInfo.id = player.id;
		humanInfo.name = player.name;
		humanInfo.hp = player.hp;
        humanInfo.Score = player.Score;
        humanInfo.Diescore = player.Diescore;
        humanInfo.CharName = player.CharName;
        humanInfo.CharSkinName = player.CharSkinName;
        humanInfo.isbattle = player.isbattle;
        humanInfo.Weapon = player.Weapon;
        humanInfo.x = player.x;
		humanInfo.y = player.y;
		humanInfo.z = player.z;
		return humanInfo;
	}

	private void ResetPlayers()
	{
		int num = 0;
		int num2 = 0;
		MsgHandler.red = 0;
		MsgHandler.bule = 0;
		foreach (string current in this.playerIds.Keys)
		{
			Player player = PlayerManager.GetPlayer(current);
			bool flag = player.camp == 1;
			if (flag)
			{
				this.SetBirthPos(player, num);
				num++;
			}
			else
			{
				this.SetBirthPos(player, num2);
				num2++;
			}
		}
		foreach (string current2 in this.playerIds.Keys)
		{
			Player player2 = PlayerManager.GetPlayer(current2);
			player2.hp = 100;
		}
	}

	public bool StartBattle(ClientState c, MsgBase msgBase)
	{
        bool flag = false;
		bool result;
		if (flag)
		{
			result = false;
		}
		else
		{
			this.status = Room.Status.FIGHT;
			this.ResetPlayers();
			MsgEnterBattle msgEnterBattle = new MsgEnterBattle();
            msgEnterBattle.time = aimTime;
			msgEnterBattle.mapId = 1;
            Player player_ = c.player;
            player_.isbattle = 1;
            ReadLog._ReadLog("Battle:" + player_.id);
            HumanInfo[] humans = new HumanInfo[this.playerIds.Count];
            List <HumanInfo> Battlhumans=new List<HumanInfo>();
            for (int i = 0; i < humans.Length; i++)
            {
                List<string> ids = new List<string>(playerIds.Keys);

                Player player = PlayerManager.GetPlayer(ids[i]);

                humans[i] = PlayerToHumanInfo(player);
                if(humans[i].isbattle == 1)
                {
                    ReadLog._ReadLog(humans[i].id);
                    ReadLog._ReadLog(humans[i].name);
                    Battlhumans.Add(humans[i]);
                }
            }

            msgEnterBattle.humans =Battlhumans.ToArray();

            for (int i = 0; i < msgEnterBattle.humans.Length; i++)
            {
                if (msgEnterBattle.humans[i].isbattle == 1)
                {
                    List<string> ids = new List<string>(playerIds.Keys);



                    foreach (string playerid in ids)
                    {
                        Player player = PlayerManager.GetPlayer(playerid);
                        if (player.isbattle == 1)
                        {
                            msgEnterBattle.humans[i] = PlayerToHumanInfo(player);
                            if (msgEnterBattle.humans[i].CharName == "")
                            {
                                msgEnterBattle.humans[i].CharName = "博丽灵梦";
                            }
                            //PlayerData pd = DbManager.GetPlayerData(player.id);
                            try
                            {
                                /* msgEnterBattle.humans[i].CharSkinName = pd.CharSkinName;
                                 msgEnterBattle.humans[i].Weapon = pd.Weapon;*/
                            }
                            catch
                            {

                            }
                            ReadLog._ReadLog(msgEnterBattle.humans[i].id);
                            ReadLog._ReadLog(msgEnterBattle.humans[i].name);
                            ReadLog._ReadLog("isBattle" + msgEnterBattle.humans[i].isbattle.ToString());
                            player.Send(msgEnterBattle);
                        }
                    }
                }
            }

			result = true;
		}
		return result;
	}

	public bool IsDie(Player player)
	{
		return player.hp <= 0;
	}

	public void Update()
	{
		if (this.status == Room.Status.FIGHT && (float)(NetManager.GetTimeStamp() - this.lastjudgeTime) >= 1f)
		{
			this.lastjudgeTime = NetManager.GetTimeStamp();
			int num = this.Judgment();
			if (num != 0)
			{
				ReadLog._ReadLog("over", null, true);
				MsgBattleResult msgBattleResult = new MsgBattleResult();

				msgBattleResult.winCamp = num;
				this.status = Room.Status.PREPARE;
				foreach (string current in this.playerIds.Keys)
				{
					Player player = PlayerManager.GetPlayer(current);
					bool flag = player.camp == num;
					if (flag)
					{
						//player.data.win++;
                        player.data.coin += (int)aimTime - int.Parse((lasttime - NetManager.GetTimeStamp()).ToString());
                        ReadLog._ReadLog("Coin" + player.data.coin + ":" + "+coin" + ((int)aimTime - int.Parse((lasttime - NetManager.GetTimeStamp()).ToString())));
                    }
                    else
					{
						//player.data.lost++;
					}
                    //DbManager.UpdatePlayerData(player.id, player.data);
                }

                this.Broadcast(msgBattleResult);
			}
		}
	}

	public void Result()
	{
		if (this.status == Room.Status.FIGHT)
		{
			this.lastjudgeTime = NetManager.GetTimeStamp();
			int num = this.nub;
			if (num != 0)
			{
                
				MsgBattleResult msgBattleResult = new MsgBattleResult();
				msgBattleResult.winCamp = num;
				this.status = Room.Status.PREPARE;
				foreach (string current in this.playerIds.Keys)
				{
					Player player = PlayerManager.GetPlayer(current);
					bool flag = player.camp == num;
					if (flag)
					{
						player.data.win++;
                        player.data.coin=(int)aimTime - int.Parse((lasttime- NetManager.GetTimeStamp()).ToString());
                    }
                    else
					{
						player.data.lost++;
					}
                    //DbManager.UpdatePlayerData(player.id, player.data);

                }

                this.Broadcast(msgBattleResult);
                foreach (string s in playerIds.Keys)
                {
                    RemovePlayer(s);
                }
            }
		}
	}

	public int Judgment()
	{
		int num = 0;
		int num2 = 0;
		foreach (string current in this.playerIds.Keys)
		{
			Player player = PlayerManager.GetPlayer(current);
			bool flag = !this.IsDie(player);
			if (flag)
			{
				bool flag2 = player.camp == 1;
				if (flag2)
				{
					num++;
				}
				bool flag3 = player.camp == 2;
				if (flag3)
				{
					num2++;
				}
			}
		}
		bool flag4 = MsgHandler.bule >= aim;
		int result;
		if (flag4)
		{
			result = 1;
		}
		else
		{
			bool flag5 = MsgHandler.red >= aim;
			if (flag5)
			{
				result = 2;
			}
			else
			{
				result = 0;
			}
		}
		return result;
	}

    public string[] playerids()
    {
        List<string> IDs = new List<string>();
        foreach(string s in playerIds.Keys)
        {
            IDs.Add(s);
        }
        return IDs.ToArray();
    }
}
