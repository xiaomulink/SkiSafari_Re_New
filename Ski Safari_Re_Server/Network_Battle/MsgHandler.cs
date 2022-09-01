using Netwolk_Battle;
using System;
using System.Collections.Generic;
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
    public static void MsgGlobalTestPlayer(ClientState c, MsgBase msgBase)
    {
        MsgGlobalTestPlayer msgTestPlayer = (MsgGlobalTestPlayer)msgBase;
        c.player = new Player(c);
        c.player.id = msgTestPlayer.id;
        c.player.name = msgTestPlayer.name;

        c.player.data = new PlayerData();
        PlayerManager.AddPlayer(c.player.id, c.player);
        Player player = c.player;
        msgTestPlayer.result = 0;
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
    public static void MsgGlobalWelcome(ClientState c, MsgBase msgBase)
    {
        MsgGlobalWelcome msg = (MsgGlobalWelcome)msgBase;
        if (msg.key == new System.Random(c.randomkey).Next(0, 13004))
        {
            c.iswel = true;
            ReadLog._ReadLog("true");
        }
        else
        {
            ReadLog._ReadLog("false");
            c.iswel = false;
        }
        MsgGlobalServer(c, msgBase);
    }

    /// <summary>
    /// 设置角色
    /// </summary>
    /// <param name="c"></param>
    /// <param name="msgBase"></param>
	public static void MsgGlobalServer(ClientState c, MsgBase msgBase)
	{
        MsgGlobalServer msg = new MsgGlobalServer();
        msg.ip = Main.main.nowip;
        msg.playernub = RoomManager.GetRoom(1).playerIds.Count+"/"+ RoomManager.GetRoom(1).maxPlayer;
        msg.serverdescription = RoomManager.GetRoom(1).describe;
        msg.serverping ="6ms";

        c.player.Send(msg);
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
                    //DbManager.UpdatePlayerData(player.id, pd);
                    break;
            }
        }else
        {
            switch (msg.itemtype)
            {
                case 0:
                    pd.CharSkinName = msg.item;
                    //DbManager.UpdatePlayerData(player.id, pd);
                    break;
                case 1:
                    pd.Weapon = msg.item;
                    //DbManager.UpdatePlayerData(player.id, pd);
                    break;
            }
        }
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
		catch(Exception ex)
		{
            ReadLog._ReadLog(ex.ToString());
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
        ReadLog._ReadLog("JoinRoom");

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
            ReadLog._ReadLog("JoinRoom");

            msgEnterRoom.result = 0;
            player.Send(msgEnterRoom);
        }
        else
        {
            ReadLog._ReadLog("PlayerNull");

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
            msgBase2.serverdescription = room.describe;
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
                ReadLog._ReadLog("roomNull");
				msgLeaveRoom.result = 1;
				player.Send(msgLeaveRoom);
				return;
			}
			room.RemovePlayer(player.id);
			msgLeaveRoom.result = 0;
			player.Send(msgLeaveRoom);
		}else
        {
            ReadLog._ReadLog("playerNull");

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
                ReadLog._ReadLog("没有找到房间");
				msgStartBattle.result = 1;
				player.Send(msgStartBattle);
				return;
			}
			/*bool flag2 = !room.isOwner(player);
			if (flag2)
			{
                ReadLog._ReadLog("不是房主");

                msgStartBattle.result = 1;
				player.Send(msgStartBattle);
				return;
			}*/
        
			bool flag3 = !room.StartBattle(c,msgBase);
			if (flag3)
			{
                ReadLog._ReadLog("已经开战");

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
        if (player == null)
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
            else if (room == null)
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
        //PlayerData pd = DbManager.GetPlayerData(player.id);
        Room room = RoomManager.GetRoom(player.roomId);
		msgResurrection.result = 1;
		player.Broadcast(msgResurrection);
		MsgInstantiateHuman msgInstantiateHuman = new MsgInstantiateHuman();
		msgInstantiateHuman.id = msgResurrection.id;
		msgInstantiateHuman.name = msgResurrection.name;
		msgInstantiateHuman.camp = msgResurrection.camp;
        //msgInstantiateHuman.CharSkinName = pd.CharSkinName;
        msgInstantiateHuman.Weapon = player.Weapon;
        //msgResurrection.CharSkinName = pd.CharSkinName;
        msgResurrection.CharName = player.CharName;
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
