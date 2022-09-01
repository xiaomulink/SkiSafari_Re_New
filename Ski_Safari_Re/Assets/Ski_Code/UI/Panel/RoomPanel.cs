using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : BasePanel
{
    private int num;
    public static int Roomid;
    public static int GameMode;
    private bool IsOwner;
    private Transform content;
    private GameObject playerObj;
    //UI
    private Button startButton;
    private Button closeButton;
    
    private Text roomidT;

    //初始化UI
    public override void OnInit()
	{
		this.skinPath = "RoomPanel";
		this.layer = PanelManager.Layer.Panel;
	}
	
	
    //UI绑定

    public async override void OnShow(params object[] args)
    {
        this.startButton = this.skin.transform.Find("StartButton").GetComponent<Button>();
        this.closeButton = this.skin.transform.Find("Button").GetComponent<Button>();
        this.content = skin.transform.Find("PlayerPanel/Scroll View/Viewport/Content");
        this.playerObj = this.skin.transform.Find("PlayerItem").gameObject;
        this.roomidT = this.skin.transform.Find("RMText").GetComponent<Text>();
        this.playerObj.SetActive(false);
        this.startButton.onClick.AddListener(this.OnStartClick);
        this.closeButton.onClick.AddListener(this.OnCloseClick);
        NetManager.AddMsgListener("MsgGetRoomInfo", OnMsgGetRoomInfo);
        NetManager.AddMsgListener("MsgLeaveRoom", OnMsgLeaveRoom);
        NetManager.AddMsgListener("MsgEnterBattle",OnMsgEnterBattle);
        await Task.Delay(100);
        NetManager.Send(new MsgGetRoomInfo());
    }

    public override void OnClose()
    {
        NetManager.RemoveMsgListener("MsgGetRoomInfo", new NetManager.MsgListener(this.OnMsgGetRoomInfo));
        NetManager.RemoveMsgListener("MsgLeaveRoom", new NetManager.MsgListener(this.OnMsgLeaveRoom));
        NetManager.RemoveMsgListener("MsgEnterBattle", new NetManager.MsgListener(this.OnMsgEnterBattle));
    }

    public void OnMsgGetRoomInfo(MsgBase msgBase)
    {
        MsgGetRoomInfo msgGetRoomInfo = (MsgGetRoomInfo)msgBase;

        Debug.LogError("2");
        try
        {
            Debug.LogError("Length:" + msgGetRoomInfo.serverdescription);
            Debug.LogError("Length:" + msgGetRoomInfo.players.Length);
        }
        catch(Exception ex)
        {
            if(msgGetRoomInfo.players==null)
            {
                Debug.LogError("PlayerNull");
            }
            Debug.LogError(ex);
        }
        Debug.LogError("1");


        foreach (PlayerDataInfo server_PlayerInfo in msgGetRoomInfo.players)
        {
            Debug.LogError(server_PlayerInfo.name);
            Debug.LogError(server_PlayerInfo.isOwner);
        }

        for (int i = this.content.childCount - 1; i >= 0; i--)
        {
            UnityEngine.Object.Destroy(this.content.GetChild(i).gameObject);
        }
        if (msgGetRoomInfo.players == null)
        {
            return;
        }
        for (int j = 0; j < msgGetRoomInfo.players.Length; j++)
        {
            this.GeneratePlayerInfo(msgGetRoomInfo.players[j]);
        }
    }

    // Token: 0x06000765 RID: 1893 RVA: 0x00031E68 File Offset: 0x00030068
    public void GeneratePlayerInfo(PlayerDataInfo playerInfo)
    {
        GameObject gameObject = Instantiate<GameObject>(playerObj);
        gameObject.transform.SetParent(this.content);
        gameObject.SetActive(true);
        gameObject.transform.localScale = Vector3.one;
        Transform transform = gameObject.transform;
        Text component = transform.Find("NameText").GetComponent<Text>();
        Text component4 = transform.Find("Owner").GetComponent<Text>();
        skin.transform.Find("RoomNum").GetComponent<Text>().text = playerInfo.roomNum.ToString();
        this.num = playerInfo.roomNum;
        component.text = playerInfo.name;
       
        if (playerInfo.isOwner == 1)
        {
            component4.text = "房主";
            this.IsOwner = true;
        }
        if (playerInfo.isOwner == 0 && playerInfo.id == MyGameInfo.id)
        {
            component4.text = "";
            this.IsOwner = false;
            this.startButton.gameObject.SetActive(false);
        }
        this.roomidT.text = "房间：" + playerInfo.roomid;

    }

    public void OnCloseClick()
    {
        NetManager.Send(new MsgLeaveRoom());
    }

    public void OnMsgLeaveRoom(MsgBase msgBase)
    {
        if (((MsgLeaveRoom)msgBase).result == 0)
        {
            try
            {
             
                base.Close();
                return;
            }
            catch
            {
               
                base.Close();
                return;
            }
        }
        PanelManager.Open<Tip>(PanelManager.UIstyle.Default, new object[]
        {
            "退出失败,房间状态错误"
        });
    }

    public void OnStartClick()
    {
        NetManager.Send(new MsgStartBattle());
    }

    public void fight()
    {
        
    }

    public static MsgEnterBattle Battleinfo = new MsgEnterBattle();

    public void OnMsgEnterBattle(MsgBase msgBase)
    {
        MsgEnterBattle msg = (MsgEnterBattle)msgBase;


        Debug.LogError("Start");
        foreach (HumanInfo hi in msg.humans)
        {
            Debug.LogError("E" + hi.id);
        }
        Battleinfo = msg;
        SkiGameManager.Instance.StartSkiing();
        Close();

    }

}
