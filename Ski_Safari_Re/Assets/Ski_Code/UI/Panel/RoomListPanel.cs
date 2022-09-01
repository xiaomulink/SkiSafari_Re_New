using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomListPanel : BasePanel
{
    private Transform content;

    private GameObject roomObj;
    public static int Roomid;
    public static int GameMode;
    //UI

    private Button createButton;
    private Button reflashButton;
   
    private Button closeButton;

    //初始化UI
    public override void OnInit()
	{
		this.skinPath = "RoomListPanel";
		this.layer = PanelManager.Layer.Panel;
	}
	
	//UI绑定
	public override void OnShow(params object[] args)
	{
        //this.scoreText = this.skin.transform.Find("InfoPanel/ScoreText").GetComponent<Text>();
        this.createButton = this.skin.transform.Find("BackGround/CreateButton").GetComponent<Button>();
        this.reflashButton = this.skin.transform.Find("BackGround/ReflashButton").GetComponent<Button>();
        this.closeButton = this.skin.transform.Find("BackGround/CloseButton").GetComponent<Button>();
        this.content = this.skin.transform.Find("BackGround/Rooms/Scroll View/Viewport/Content");
        this.roomObj = this.skin.transform.Find("Room").gameObject;
        this.roomObj.SetActive(false);
        //this.idText.text = MyGameInfo.id;
        this.closeButton.onClick.AddListener(OnCloseClick);
        this.createButton.onClick.AddListener(OnCreateClick);
        this.reflashButton.onClick.AddListener(OnReflashClick);
        NetManager.AddMsgListener("MsgGetAchieve",OnMsgGetAchieve);
        NetManager.AddMsgListener("MsgGetRoomList",OnMsgGetRoomList);
        NetManager.AddMsgListener("MsgEnterRoom",OnMsgEnterRoom);
        NetManager.Send(new MsgGetAchieve());
        try
        {
            NetManager.Send(new MsgGetRoomList());
        }
        catch (Exception ex)
        {
            Debug.Log(ex);
        }
    }

    // Token: 0x06000757 RID: 1879 RVA: 0x000318F4 File Offset: 0x0002FAF4
    public void GenerateRoom(RoomInfo roomInfo)
    {
        GameObject gameObject = Instantiate(roomObj);
        gameObject.transform.SetParent(this.content);
        gameObject.SetActive(true);
        gameObject.transform.localScale = Vector3.one;
        Transform transform = gameObject.transform;
        Text component = transform.Find("IdText").GetComponent<Text>();
        Text describetext = transform.Find("DescribeText").GetComponent<Text>();
        
        Text component4 = transform.Find("PlayerCountText").GetComponent<Text>();
        //Text component5 = transform.Find("GamemodeText").GetComponent<Text>();
        Button btn = transform.Find("JoinButton").GetComponent<Button>();
        Button noJoinbtn = transform.Find("NoJoinButton").GetComponent<Button>();
        component.text = roomInfo.id.ToString();
        describetext.text = roomInfo.describe;

        component4.text = roomInfo.count.ToString() +"/"+roomInfo.maxplayer.ToString();
      
        if(roomInfo.status==0)
        {
            //可以加入
            noJoinbtn.gameObject.SetActive(false);
            btn.gameObject.SetActive(true);
        }
        else
        {
            //已经开始
            noJoinbtn.gameObject.SetActive(true);
            btn.gameObject.SetActive(false);
        }

        btn.name = roomInfo.id.ToString();
        btn.onClick.AddListener(delegate ()
        {
            this.OnJoinClick(btn.name);
        });
    }

    public void OnReflashClick()
    {
        try
        {
            NetManager.Send(new MsgGetRoomList());
        }
        catch
        { }
    }

    public void OnCreateClick()
    {
        PanelManager.Open<AddRoomPanel>();
        base.Close();
    }

    public void OnJoinClick(string idString)
    {
        NetManager.Send(new MsgEnterRoom
        {
            id = int.Parse(idString)
        });
    }

    public void OnMsgGetRoomList(MsgBase msgBase)
    {
        Debug.Log("Get!");
        MsgGetRoomList msgGetRoomList = (MsgGetRoomList)msgBase;
        for (int i = this.content.childCount - 1; i >= 0; i--)
        {
            UnityEngine.Object.Destroy(this.content.GetChild(i).gameObject);
        }
        if (msgGetRoomList.rooms == null)
        {
            return;
        }
        Debug.Log("Room" + msgGetRoomList.rooms.Length);
        for (int j = 0; j < msgGetRoomList.rooms.Length; j++)
        {
            this.GenerateRoom(msgGetRoomList.rooms[j]);
        }
    }

    public void OnMsgGetAchieve(MsgBase msgBase)
    {
        MsgGetAchieve msgGetAchieve = (MsgGetAchieve)msgBase;
    }

    public void OnMsgEnterRoom(MsgBase msgBase)
    {
        if (((MsgEnterRoom)msgBase).result == 0)
        {
            PanelManager.Open<RoomPanel>(PanelManager.UIstyle.Default, Array.Empty<object>());
            base.Close();
            return;
        }
        PanelManager.Open<Tip>(PanelManager.UIstyle.Default, new object[]
        {
            "进入失败,房间已经开战"
        });
    }

    public void OnCloseClick()
    {
     
        base.Close();
    }

}
