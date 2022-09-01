using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Web.Script.Serialization;

public class MsgHandler
{
	private static JavaScriptSerializer Js = new JavaScriptSerializer();
    /// <summary>
    /// 红方
    /// </summary>
    public static int red = 0;

	public static bool istimeout = false;

	public static bool isbulewin = false;

	public static bool isredwin = false;
    /// <summary>
    /// 蓝方
    /// </summary>
	public static int bule = 0;

    /// <summary>
    /// 开始时间
    /// </summary>
	public static float startTime = 0f;
    
    /// <summary>
    /// player测试
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
    public static void MsgTestPlayer(ClientState c, MsgBase msgBase)
    {
        MsgTestPlayer msgTestPlayer = (MsgTestPlayer)msgBase;
        c.player = new Player(c);
        c.player.id = msgTestPlayer.id;
        c.player.name = msgTestPlayer.name;

        c.player.data = new PlayerData();
        bool isadd = PlayerManager.AddPlayer(c.player.id, c.player);
        Player player = c.player;
        if (isadd)
        {
            msgTestPlayer.result = 0;
        }
        else
        {
            msgTestPlayer.result = 1;
        }
        player.Send(msgTestPlayer);
    }

    /// <summary>
    /// 使用技能
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
	public static void MsgUseSkill(ClientState c, MsgBase msgBase)
	{
		MsgUseSkill msgUseSkill = (MsgUseSkill)msgBase;
		Player player = c.player;
		if (player != null)
		{
			Room room = RoomManager.GetRoom(player.roomId);
			if (room != null && room.status == Room.Status.FIGHT)
			{
				player.Usecharname = msgUseSkill.charname;
				player.UseSkillnub = msgUseSkill.Skillnub;
				msgUseSkill.id = player.id;
				room.Broadcast(msgUseSkill);
			}
		}
	}

    /// <summary>
    /// 获取名字
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
	public static void MsgGetName(ClientState c, MsgBase msgBase)
	{
		MsgGetName msgGetName = (MsgGetName)msgBase;
		Player player = c.player;
		PlayerData playerData = DbManager.Get(msgGetName.id);
		msgGetName.name = playerData.P_name;
		player.Send(msgGetName);
	}

    /// <summary>
    /// 版本获取
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
	public static void Msggetversions(ClientState c, MsgBase msgBase)
	{
		Msggetversions msggetversions = (Msggetversions)msgBase;
		try
		{
			ReadLog._ReadLog("Msggetversions", null, true);
			msggetversions.value = NetManager.versionsvalue;
			NetManager.Send(c, msggetversions);
		}
		catch (Exception arg)
		{
			ReadLog._ReadLog("SendError!ins:" + arg, null, true);
		}
	}

    /// <summary>
    /// 可以ping
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
	public static void Msgisping(ClientState c, MsgBase msgBase)
	{
		Msgisping msgisping = (Msgisping)msgBase;
		bool flag = msgisping.result == 1;
		if (flag)
		{
            c.lastPingTime = NetManager.GetTimeStamp();
            c.isping = false;
		}
		else
		{
            c.lastPingTime = NetManager.GetTimeStamp();
            c.isping = true;
		}
		NetManager.Send(c, msgisping);
	}

    /// <summary>
    /// 登陆验证
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
    public static void MsgWelcome(ClientState c, MsgBase msgBase)
    {
        MsgWelcome msg = (MsgWelcome)msgBase;
      
        if (msg.key == new System.Random(c.randomkey).Next(0, 2000))
        {
            c.iswel = true;
            ReadLog._ReadLog("true");

        }else
        {
            ReadLog._ReadLog("false");
            c.iswel = false;

        }
    }

    /// <summary>
    /// 注册
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
    public static void MsgRegister(ClientState c, MsgBase msgBase)
	{
		MsgRegister msgRegister = (MsgRegister)msgBase;
		if (DbManager.Register(msgRegister.id, msgRegister.pw, msgRegister.name))
		{
			DbManager.CreatePlayer(msgRegister.id, msgRegister.name);
			msgRegister.result = 0;
		}
		else
		{
			msgRegister.result = 1;
			ReadLog._ReadLog("RegisterError", null, true);
		}
		NetManager.Send(c, msgRegister);
	}

    /// <summary>
    /// 设置角色
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
	public static void MsgAddChar(ClientState c, MsgBase msgBase)
	{
		MsgAddChar msgAddChar = (MsgAddChar)msgBase;
		Player player = PlayerManager.GetPlayer(msgAddChar.id);
		player.CharName = msgAddChar.CharName;
		if (PlayerManager.GetPlayer(msgAddChar.id) == null)
		{
			msgAddChar.result = 0;
			NetManager.Send(c, msgAddChar);
			return;
		}
		msgAddChar.result = 1;
		player.Send(msgAddChar);
	}

    /// <summary>
    /// 登陆
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
	public static void MsgLogin(ClientState c, MsgBase msgBase)
	{
		MsgLogin msgLogin = (MsgLogin)msgBase;
		bool flag = !DbManager.CheckPassword(msgLogin.id, msgLogin.pw, msgLogin.name);
		if (flag)
		{
			msgLogin.result = 1;
			ReadLog._ReadLog("Password！"+ msgLogin.id, null, true);
			NetManager.Send(c, msgLogin);
			return;
		}
		bool flag2 = c.player != null;
		if (flag2)
		{
			msgLogin.result = 1;
			NetManager.Send(c, msgLogin);
			return;
		}
		bool flag3 = PlayerManager.IsOnline(msgLogin.id);
		if (flag3)
		{
			Player player = PlayerManager.GetPlayer(msgLogin.id);
			player.Send(new MsgKick
			{
				reason = 0
			});
			NetManager.Close(player.state);
		}
		PlayerData playerData = DbManager.Get(msgLogin.id);
		if (playerData == null && !DbManager.CreatePlayer(msgLogin.id, DbManager.GetID(msgLogin.id)))
		{
			ReadLog._ReadLog(DbManager.GetID(msgLogin.id), null, true);
			msgLogin.result = 1;
			NetManager.Send(c, msgLogin);
			return;
		}
		Player player2 = new Player(c);
		player2.id = msgLogin.id;
		try
		{
			msgLogin.name = DbManager.GetID(msgLogin.id);
            player2.name = DbManager.GetID(msgLogin.id);
            ReadLog._ReadLog(player2.name, null, true);
        }
		catch
		{

		}
		player2.data = playerData;
		PlayerManager.AddPlayer(player2.id, player2);
		Player player3 = PlayerManager.GetPlayer(player2.id);
		bool flag4 = player3 == null;
		if (flag4)
		{
			msgLogin.result = 1;
			return;
		}
        msgLogin.win = player3.data.win;
        msgLogin.lost = player3.data.lost;
        msgLogin.coin = player3.data.coin;
		c.player = player2;
		msgLogin.result = 0;
		player2.Send(msgLogin);
	}

    /// <summary>
    /// 保存制造物体
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
	public static void MsgSaveCreatObjcet(ClientState c, MsgBase msgBase)
	{
		Player arg_06_0 = c.player;
		MsgSaveCreatObjcet msgSaveCreatObjcet = (MsgSaveCreatObjcet)msgBase;
		Dictionary<string, int> dictionary = MsgHandler.DeserializeStringToDictionary<string, int>(msgSaveCreatObjcet.text);
		foreach (string current in dictionary.Keys)
		{
			int nub = 0;
			dictionary.TryGetValue(current, out nub);
			DbManager.AddCreateObject(msgSaveCreatObjcet.id, current, nub);
		}
	}

	public static Dictionary<TKey, TValue> DeserializeStringToDictionary<TKey, TValue>(string jsonStr)
	{
		bool flag = string.IsNullOrEmpty(jsonStr);
		Dictionary<TKey, TValue> result;
		if (flag)
		{
			result = new Dictionary<TKey, TValue>();
		}
		else
		{
			Dictionary<TKey, TValue> dictionary = MsgHandler.Js.Deserialize<Dictionary<TKey, TValue>>(jsonStr);
			result = dictionary;
		}
		return result;
	}

    /// <summary>
    /// （不知道）
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
	public static void MsgGetAchieve(ClientState c, MsgBase msgBase)
	{
		try
		{
			MsgGetAchieve msgGetAchieve = (MsgGetAchieve)msgBase;
			Player player = c.player;
			if (player != null)
			{
				msgGetAchieve.win = player.data.win;
				msgGetAchieve.lost = player.data.lost;
				player.Send(msgGetAchieve);
			}
		}
		catch
		{
		}
	}

    /// <summary>
    /// 空
    /// </summary>
    /// <param name="o"></param>
    /// <param name="msgBase"></param>
	public static void Msgnull(ClientState o, MsgBase msgBase)
	{
		Msgnull arg_06_0 = (Msgnull)msgBase;
		ReadLog._ReadLog("Null", null, true);
	}

    /// <summary>
    /// 比分
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
	public static void Msgscoreboard(ClientState c, MsgBase msgBase)
	{
		Player player = c.player;
		Msgscoreboard msgscoreboard = (Msgscoreboard)msgBase;
		if (msgscoreboard.time != 0f)
		{
			MsgHandler.startTime = msgscoreboard.time;
		}
		bool flag = msgscoreboard.camp == 1;
		if (flag)
		{
			msgscoreboard.red++;
			MsgHandler.red = msgscoreboard.red;
		}
		else
		{
			bool flag2 = msgscoreboard.camp == 2;
			if (flag2)
			{
				msgscoreboard.bule++;
				MsgHandler.bule = msgscoreboard.bule;
			}
		}
        msgscoreboard.aim = RoomManager.GetRoom(c.player.roomId).aim;
		player.Send(msgscoreboard);
	}

    /// <summary>
    /// 获取？
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
    public static void Msgselect(ClientState c, MsgBase msgBase)
    {
        Player player = c.player;
        PlayerData pd = player.data;

        Msgselect msg = (Msgselect)msgBase;
        if(msg.type==0)
        {
            switch (msg.itemtype)
            {
                case 0:
                    msg.item = c.player.data.CharSkinName;
                    player.Send(msg);
                    break;
                case 1:
                    msg.item = c.player.data.Weapon;
                    DbManager.UpdatePlayerData(player.id, pd);
                    break;
            }
        }else
        {
            switch (msg.itemtype)
            {
                case 0:
                    pd.CharSkinName = msg.item;
                    DbManager.UpdatePlayerData(player.id, pd);
                    break;
                case 1:
                    pd.Weapon = msg.item;
                    DbManager.UpdatePlayerData(player.id, pd);
                    break;
            }
        }
    }

    /// <summary>
    /// 获取？
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
    public static void MsgSeletEq(ClientState c, MsgBase msgBase)
    {
        MsgSeletEq msg = (MsgSeletEq)msgBase;
        msg.id = 0;
        PlayerData pd= DbManager.GetPlayerData(c.player.id);

        switch (msg.type)
        {
            case "0":
                msg.id = int.Parse(pd.CharSkinName);
                break;
            case "1":
                msg.id = int.Parse(pd.Weapon);
                break;
        }
        
        c.player.Send(msg);
    }

    /// <summary>
    /// 商店
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
    public static void MsgShop(ClientState c, MsgBase msgBase)
    {
        Player player = c.player;
        MsgShop msg = (MsgShop)msgBase;
        ItemData item = new ItemData();
        item.itemId = msg.itemid;
        item.type = msg.itemtype;
        item.itemNub = msg.itemnub;
        if (DbManager.UpdatePlayerItem(player.id,c.player,item))
        {
            msg.result = 0;//成功
        }
        else
        {
            msg.result = 1;//失败
        }
        msg.coin = DbManager.GetPlayerData(c.player.id).coin;

        player.Send(msg);
    }

    /// <summary>
    /// 比分版
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
    public static void MsgScoreboardD(ClientState c, MsgBase msgBase)
    {
        Player player = c.player;
        MsgScoreboardD msgscoreboard = (MsgScoreboardD)msgBase;
        msgscoreboard.players = new HumanInfo[RoomManager.GetRoom(c.player.roomId).playerids().Length];
        int num = 0;
        foreach (string current in RoomManager.GetRoom(c.player.roomId).playerids())
        {
            msgscoreboard.players[num] = Room.PlayerToHumanInfo(PlayerManager.GetPlayer(current));
            num++;
        }
        player.Send(msgscoreboard);
    }

    /// <summary>
    /// 超时
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
    public static void MsgBattleTimeout(ClientState c, MsgBase msgBase)
	{
		MsgBattleTimeout msgBattleTimeout = (MsgBattleTimeout)msgBase;
		Player player = c.player;
        try
        {
            if (RoomManager.GetRoom(c.player.roomId).status == Room.Status.FIGHT)
            {
                if (NetManager.GetTimeStamp() - msgBattleTimeout.time >= 10L)
                {
                    Room room = RoomManager.GetRoom(player.roomId);
                    int num = 0;
                    if (MsgHandler.red == 0 && MsgHandler.bule == 0)
                    {
                        ReadLog._ReadLog("bule", null, true);
                        num = 2;
                        room.nub = num;
                        room.Result();
                    }
                    else if (MsgHandler.red > MsgHandler.bule)
                    {
                        num = 1;
                        room.nub = num;
                        room.Result();
                    }
                    else if (MsgHandler.red < MsgHandler.bule)
                    {
                        num = 2;
                        room.nub = num;
                        room.Result();
                    }
                    room.nub = num;
                    room.Result();
                    room.Broadcast(msgBattleTimeout);
                    return;
                }
            }
            else
            {
                ReadLog._ReadLog("NotTimeOut", null, true);
            }
        }catch
        { }
	}

    /// <summary>
    /// 死亡
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
	public static void MsgDie(ClientState c, MsgBase msgBase)
	{
		MsgDie msgDie = (MsgDie)msgBase;
		Player player = c.player;
        try
        {
            Player p = PlayerManager.GetPlayer(msgDie.Killerid);
            p.Score++;
        }
        catch
        {

        }
        player.Diescore++;
        c.player.CharName = msgDie.CharName;
        c.player.Weapon = msgDie.Weapon;
        msgDie.time = 5f;
		msgDie.result = 1;
		player.Send(msgDie);
	}

    /// <summary>
    /// 获取房间列表
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
	public static void MsgGetRoomList(ClientState c, MsgBase msgBase)
	{
		try
		{
			MsgGetRoomList arg_06_0 = (MsgGetRoomList)msgBase;
			Player player = c.player;
            if (player != null)
            {
                player.Send(RoomManager.ToMsg());
                ReadLog._ReadLog("playerGet");
            }
            else
            {
                ReadLog._ReadLog("playernull");

            }
        }
        catch (Exception ex)
        {
            ReadLog._ReadLog(ex.ToString());
        }
    }

    /// <summary>
    /// 创建房间
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
	public static void MsgCreateRoom(ClientState c, MsgBase msgBase)
	{
		MsgCreateRoom msgCreateRoom = (MsgCreateRoom)msgBase;
		Player player = c.player;
        if (c.player != null)
		{
			bool flag = player.roomId >= 0;
			if (flag)
			{
                ReadLog._ReadLog(" player.roomId >= 0", null, true);

                msgCreateRoom.result = 1;
				player.Send(msgCreateRoom);
				return;
			}
			Room room = RoomManager.AddRoom();
			room.maxPlayer = msgCreateRoom.maxplayer;
            room.describe = msgCreateRoom.describe;
            switch(room.maxPlayer)
            {
                case 2:
                    {
                        room.aim=5;
                        room.aimTime = 600f;
                    }
                    break;
                case 4:
                    {
                        room.aim = 10;
                        room.aimTime = 600f;
                    }
                    break;
                case 6:
                    {
                        room.aim = 20;
                        room.aimTime = 600f;
                    }
                    break;
            }
			room.Roomgamemode = msgCreateRoom.Gamemode;
			Player player2 = PlayerManager.GetPlayer(msgCreateRoom.id);
			bool flag2 = player2 == null;
			if (flag2)
			{
				ReadLog._ReadLog("房间玩家为空", null, true);
				msgCreateRoom.result = 1;
				player.Send(msgCreateRoom);
				return;
			}
			room.AddPlayer(msgCreateRoom.id);
			msgCreateRoom.result = 0;
			room.Roomgamemode = msgCreateRoom.Gamemode;
			msgCreateRoom.roomid = room.id;
			player.Send(msgCreateRoom);
		}else
        {
            ReadLog._ReadLog("没有此玩家！");
        }
	}




    /// <summary>
    /// 添加AI(废弃)
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
	public static void MsgJoinAi(ClientState c, MsgBase msgBase)
	{
		MsgJoinAi msgJoinAi = (MsgJoinAi)msgBase;
		bool flag = msgJoinAi.ison == 1;
		if (flag)
		{
			Player player = new Player(c);
			player.id = "1";
			player.data = DbManager.Get(player.id);
			player.name = "Ai";
			c.player.roomId = msgJoinAi.id;
			MsgBase msgBase2 = new MsgGetRoomInfo();
			Player player2 = c.player;
			if (player2 != null)
			{
				Room room = RoomManager.GetRoom(msgJoinAi.id);
				PlayerManager.AddPlayer(player.id, player);
				bool flag2 = room == null;
				if (flag2)
				{
					msgJoinAi.result = 1;
					player2.Send(msgJoinAi);
					return;
				}
				bool flag3 = !room.AddPlayer(player.id);
				if (flag3)
				{
					msgJoinAi.result = 1;
					player2.Send(msgJoinAi);
					return;
				}
				msgJoinAi.result = 0;
				player2.Send(msgJoinAi);
				MsgHandler.MsgGetRoomInfo(c, msgBase2);
				return;
			}
		}
		else
		{
			Room room2 = RoomManager.GetRoom(msgJoinAi.id);
			MsgBase msgBase3 = new MsgGetRoomInfo();
			MsgGetRoomInfo arg_FE_0 = (MsgGetRoomInfo)msgBase3;
			MsgHandler.MsgGetRoomInfo(c, msgBase3);
			room2.RemovePlayer("1");
		}
	}
    /// <summary>
    /// 开始战斗
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
	public static void MsgEnterRoom(ClientState c, MsgBase msgBase)
	{
		MsgEnterRoom msgEnterRoom = (MsgEnterRoom)msgBase;
		Player player = c.player;
		if (player != null)
		{
			if (!(player.id == "1"))
			{
				bool flag = player.roomId >= 0;
				if (flag)
				{
					msgEnterRoom.result = 1;
					player.Send(msgEnterRoom);
                    ReadLog._ReadLog("你tmid为一！");
                    return;
				}
			}
			Room room = RoomManager.GetRoom(msgEnterRoom.id);
			bool flag2 = room == null;
			if (flag2)
			{
				msgEnterRoom.result = 1;
				player.Send(msgEnterRoom);
                ReadLog._ReadLog("找不到Room");
                return;
			}
			bool flag3 = !room.AddPlayer(player.id);
			if (flag3)
			{
				msgEnterRoom.result = 1;
				player.Send(msgEnterRoom);
                ReadLog._ReadLog("无法加入Player");
                return;
			}
			msgEnterRoom.result = 0;
			player.Send(msgEnterRoom);
		}
	}
    /// <summary>
    /// 获取房间信息
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
	public static void MsgGetRoomInfo(ClientState c, MsgBase msgBase)
	{
		MsgGetRoomInfo msgBase2 = (MsgGetRoomInfo)msgBase;
		Player player = c.player;
		if (player != null)
		{
			Room room = RoomManager.GetRoom(player.roomId);
			bool flag = room == null;
			if (flag)
			{
				player.Send(msgBase2);
                ReadLog._ReadLog("RoomNull");
				return;
			}
			player.Send(room.ToMsg());
		}
	}

    /// <summary>
    /// 获取房间信息
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
    public static void MsgGetGameRoomInfo(ClientState c, MsgBase msgBase)
    {
        MsgGetGameRoomInfo msgBase2 = (MsgGetGameRoomInfo)msgBase;
        Player player = c.player;
        if (player != null)
        {
            Room room = RoomManager.GetRoom(player.roomId);
            bool flag = room == null;
            if (flag)
            {
                player.Send(msgBase2);
                ReadLog._ReadLog("RoomNull");
                return;
            }
            player.Send(room.ToMsg());
        }
    }
    /// <summary>
    /// 删除房间
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
	public static void MsgLeaveRoom(ClientState c, MsgBase msgBase)
	{
		MsgLeaveRoom msgLeaveRoom = (MsgLeaveRoom)msgBase;
		Player player = c.player;
		if (player != null)
		{
			Room room = RoomManager.GetRoom(player.roomId);
			bool flag = room == null;
			if (flag)
			{
				msgLeaveRoom.result = 1;
				player.Send(msgLeaveRoom);
				return;
			}
			room.RemovePlayer(player.id);
			msgLeaveRoom.result = 0;
			player.Send(msgLeaveRoom);
		}
	}

    /// <summary>
    /// 捡起物体
    /// </summary>
    public static void MsgPickUP(ClientState c, MsgBase msgBase)
    {
        MsgPickUP msg = (MsgPickUP)msgBase;
        Player player = c.player;
        msg.PlayerId = player.id;
        Room room = RoomManager.GetRoom(player.roomId);

        room.Broadcast(msg);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public static void MsgExpliciItem(ClientState c, MsgBase msgBase)
    {
        MsgExpliciItem msg = (MsgExpliciItem)msgBase;
        Player player = c.player;
        Room room = RoomManager.GetRoom(player.roomId);
        int ran = 3000;
        if (room != null)
        {
            for (int i = 0; i < msg.RandomID.Length; i++)
            {
                msg.result_RandomID[i] = new Random().Next(1, ran);

                ran = msg.result_RandomID[i];
            }
            room.Broadcast(msg);
        }
    }

    /// <summary>
    /// 确定战斗_(2)
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
	public static void MsgStartBattle(ClientState c, MsgBase msgBase)
	{
		MsgStartBattle msgStartBattle = (MsgStartBattle)msgBase;
		Player player = c.player;
		if (player != null)
		{
			Room room = RoomManager.GetRoom(player.roomId);
			bool flag = room == null;
			if (flag)
			{
				msgStartBattle.result = 1;
				player.Send(msgStartBattle);
				return;
			}
			bool flag2 = !room.isOwner(player);
			if (flag2)
			{
				msgStartBattle.result = 1;
				player.Send(msgStartBattle);
				return;
			}
			bool flag3 = !room.StartBattle();
			if (flag3)
			{
				msgStartBattle.result = 1;
				player.Send(msgStartBattle);
				return;
			}
			msgStartBattle.result = 0;
			player.Send(msgStartBattle);
		}
	}
    /// <summary>
    /// 获取玩家数据
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
    public static void MsgGetInfo(ClientState c, MsgBase msgBase)
    {
        MsgGetInfo msg = (MsgGetInfo)msgBase;
        Player player = PlayerManager.GetPlayer(msg.id);
        if(player==null)
        {
            msg.result = 1;
            return;
        }
        ReadLog._ReadLog("GetPlayer数据");
        msg.name = player.name;
        msg.coin = player.data.coin;
        msg.win = player.data.win;
        msg.lost = player.data.lost;
        msg.result = 0;
        player.Send(msg);
    }
    /// <summary>
    /// 其他玩家
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
    public static void MsgSyncPlayer(ClientState c, MsgBase msgBase)
	{
        MsgSyncPlayer msgSync = (MsgSyncPlayer)msgBase;
		Player player = c.player;
		if (player != null)
		{
			Room room = RoomManager.GetRoom(player.roomId);
            if (room != null && room.status == Room.Status.FIGHT)
            {
                /*bool flag = Math.Abs(player.x - msgSyncHuman.x) > 10 || Math.Abs(player.y - msgSyncHuman.y) > 10 || Math.Abs(player.z - msgSyncHuman.z) > 10;
                if (flag)
                {
                    ReadLog._ReadLog("疑似作弊 " + player.id, null, true);
                }*/
                player.mx = msgSync.mx;
                player.my = msgSync.my;
                player.x = msgSync.x;
                player.y = msgSync.y;
                player.z = msgSync.z;
                player.State = msgSync.State;
                player.playerType = msgSync.playerType;
                player.playerMovementType = msgSync.playerMovementType;
                player.playerMovementHandType = msgSync.playerMovementHandType;
                player.itembag = msgSync.itembag;
                player.itemcloth = msgSync.itemcloth;
                player.itemprotective = msgSync.itemprotective;
                player.itemshoes = msgSync.itemshoes;
                player.itemweapon = msgSync.itemweapon;
                msgSync.id = player.id;
                room.Broadcast(msgSync);
            }
            else if(room == null)
            {
                ReadLog._ReadLog("roomNull", null, true);

            }
            else if (room.status != Room.Status.FIGHT)
            {
                ReadLog._ReadLog("roomNOFIGHT", null, true);

            }
        }
        else
        {
            ReadLog._ReadLog("PlayerNull", null, true);
        }
    }

    /// <summary>
    /// 玩家聊天
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
    public static void MsgGameChat(ClientState c, MsgBase msgBase)
    {
        MsgGameChat msgChat = (MsgGameChat)msgBase;
        Player player = c.player;
        ReadLog._ReadLog("Chat");
        if (player != null)
        {
            Room room = RoomManager.GetRoom(player.roomId);
            ReadLog._ReadLog(msgChat.ChatType.ToString());

            if (msgChat.ChatType != 2 && msgChat.ChatType != 3)
            {
                if (room != null && room.status == Room.Status.FIGHT)
                {
                    msgChat.Name = player.name;
                    msgChat.ChatType = 1;
                    room.Broadcast(msgChat);
                }
                else
                {
                    ReadLog._ReadLog("2");
                }
            }
            else if (msgChat.ChatType == 2 || msgChat.ChatType == 3)
            {
                msgChat.Name = player.name;
                PlayerManager.Broadcast(msgChat);
            }
        }
        else
        {
            ReadLog._ReadLog("1");
        }
    }

    /// <summary>
    /// 系统聊天
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
    public static void MsgChat(ClientState c, MsgBase msgBase,string chat)
    {
        MsgGameChat msgChat = (MsgGameChat)msgBase;
        Player player = c.player;
        if (player != null)
        {
            Room room = RoomManager.GetRoom(player.roomId);
            if (room != null && room.status == Room.Status.FIGHT)
            {
                //msgChat.Name = player.name;
                msgChat.ChatType = 0;
                room.Broadcast(msgChat);
            }
        }
    }

    /// <summary>
    /// 瞄准(2D)
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
	public static void MsgAim(ClientState c, MsgBase msgBase)
	{
		MsgAim msgAim = (MsgAim)msgBase;
		Player player = c.player;
		if (player != null)
		{
			Room room = RoomManager.GetRoom(player.roomId);
			if (room != null && room.status == Room.Status.FIGHT)
			{
				msgAim.id = player.id;
				room.Broadcast(msgAim);
			}
		}
	}
    /// <summary>
    /// 开火
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
	public static void MsgFire(ClientState c, MsgBase msgBase)
	{
		MsgFire msgFire = (MsgFire)msgBase;
		Player player = c.player;
		if (player != null)
		{
			Room room = RoomManager.GetRoom(player.roomId);
			if (room != null && room.status == Room.Status.FIGHT)
			{
				msgFire.id = player.id;
				room.Broadcast(msgFire);
			}
		}
	}
    /// <summary>
    /// 复活
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
	public static void MsgResurrection(ClientState c, MsgBase msgBase)
	{
		MsgResurrection msgResurrection = (MsgResurrection)msgBase;
		Player player = c.player;
        PlayerData pd = DbManager.GetPlayerData(player.id);
        Room room = RoomManager.GetRoom(player.roomId);
		msgResurrection.result = 1;
		player.Broadcast(msgResurrection);
        /*
		MsgInstantiateHuman msgInstantiateHuman = new MsgInstantiateHuman();
		msgInstantiateHuman.id = msgResurrection.id;
		msgInstantiateHuman.name = msgResurrection.name;
		msgInstantiateHuman.camp = msgResurrection.camp;
        msgInstantiateHuman.Weapon = player.Weapon;
        if (pd != null)
        {
            msgInstantiateHuman.CharSkinName = pd.CharSkinName;
            msgResurrection.CharSkinName = pd.CharSkinName;
        }
        msgResurrection.CharName = player.CharName;
*/
		room.Broadcast(msgResurrection);
	}
    /// <summary>
    /// 受击
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
	public static void MsgHit(ClientState c, MsgBase msgBase)
	{
		MsgHit msgHit = (MsgHit)msgBase;
		Player player = c.player;
		if (player != null)
		{
			Player player2 = PlayerManager.GetPlayer(msgHit.targetId);
			if (player2 != null)
			{
				Room room = RoomManager.GetRoom(player.roomId);
				if (room != null && room.status == Room.Status.FIGHT && !(player.id != msgHit.id))
				{
					int num = 20;
					player2.hp -= num;
					msgHit.id = player.id;
					msgHit.hp = player.hp;
					msgHit.damage = num;
					room.Broadcast(msgHit);
				}
			}
		}
	}
    /// <summary>
    /// Ping
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
	public static void MsgPing(ClientState c, MsgBase msgBase)
	{
		c.lastPingTime = NetManager.GetTimeStamp();
		MsgPong msg = new MsgPong();
		NetManager.Send(c, msg);
	}
}
