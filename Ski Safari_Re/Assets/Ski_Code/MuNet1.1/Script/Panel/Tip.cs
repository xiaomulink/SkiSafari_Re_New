using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200016F RID: 367
public class Tip : BasePanel
{
	// Token: 0x0600072A RID: 1834 RVA: 0x00030A78 File Offset: 0x0002EC78
	public override void OnInit()
	{
		this.skinPath = "Tip";
		this.layer = PanelManager.Layer.Tip;
	}

	// Token: 0x0600072B RID: 1835 RVA: 0x00030A8C File Offset: 0x0002EC8C
	public override void OnShow(params object[] args)
	{
		this.text = this.skin.transform.Find("UI_O/TipText").GetComponent<Text>();
		this.okBtn = this.skin.transform.Find("UI_O/OkBtn").GetComponent<Button>();
		this.okBtn.onClick.AddListener(new UnityAction(this.OnOkClick));
		if (args.Length == 1)
		{
			this.text.text = (string)args[0];
		}
	}

	

	// Token: 0x0600072D RID: 1837 RVA: 0x00030B3C File Offset: 0x0002ED3C
	public void OnOkClick()
	{
		if (this.bools == 1)
		{
			Application.Quit();
		}
		base.Close();
	}

	// Token: 0x04000854 RID: 2132
	private Text text;

	// Token: 0x04000855 RID: 2133
	public int bools;

	// Token: 0x04000856 RID: 2134
	private Button okBtn;

	// Token: 0x04000857 RID: 2135
	public bool isaffirm;
}
