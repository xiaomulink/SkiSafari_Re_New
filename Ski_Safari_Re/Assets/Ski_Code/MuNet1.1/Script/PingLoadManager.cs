using System;
using System.Threading.Tasks;

// Token: 0x02000118 RID: 280
public class PingLoadManager : BasePanel
{
	// Token: 0x060005A3 RID: 1443 RVA: 0x00029210 File Offset: 0x00027410
	private async void Start()
	{
		try
		{
			await Task.Delay(5000);
			base.Close();
		}
		catch
		{
		}
	}

	// Token: 0x060005A4 RID: 1444 RVA: 0x0002924C File Offset: 0x0002744C
	public override void OnInit()
	{
		this.skinPath = "LoadPanel(Ping)";
		this.layer = PanelManager.Layer.Panel;
	}

	// Token: 0x060005A5 RID: 1445 RVA: 0x00029260 File Offset: 0x00027460
	public override async void OnShow(params object[] args)
	{
	}

	// Token: 0x060005A6 RID: 1446 RVA: 0x00029294 File Offset: 0x00027494
	public override void OnClose()
	{
	}

	// Token: 0x060005A7 RID: 1447 RVA: 0x00029298 File Offset: 0x00027498
	public async static void AddPanel(string name)
	{
		if (name == "MsgPing" || name == "Msgisping" || name == "MsgSyncPlayer" || name == "MsgAim" || name == "MsgFire")
		{
			return;
		}
		PanelManager.Open<PingLoadManager>(PanelManager.UIstyle.Default, Array.Empty<object>());
        await Task.Delay(5000);
        PanelManager.Close("PingLoadManager");

    }

    // Token: 0x060005A8 RID: 1448 RVA: 0x000292F4 File Offset: 0x000274F4
    public static void RePanel(string name)
	{
		PanelManager.Close("PingLoadManager");
	}

	// Token: 0x060005A9 RID: 1449 RVA: 0x00029300 File Offset: 0x00027500
	private void Update()
	{
	}
}
