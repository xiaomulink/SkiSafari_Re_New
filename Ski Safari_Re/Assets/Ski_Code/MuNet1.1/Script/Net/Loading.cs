using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

// Token: 0x0200010C RID: 268
public class Loading : MonoBehaviour
{
    [SerializeField] private string gameScene = "Game.unity";
    // Token: 0x0600056C RID: 1388 RVA: 0x000288D4 File Offset: 0x00026AD4
    public static void Load(string Scene, string className = null, string methodName = null, string loadsound = null)
	{
        //Gamememory.Load2Scene();
        Loading.sceneName = Scene;
		Loading.className = className;
		Loading.methodName = methodName;
		Loading.loadsound = loadsound;

        SceneManager.LoadScene("Load");
	}



    // Token: 0x0600056D RID: 1389 RVA: 0x000288F8 File Offset: 0x00026AF8
    private void Start()
	{
		Screen.sleepTimeout = -1;
		this.ShowProgressBar();
	}
    // Token: 0x0600056E RID: 1390 RVA: 0x00028908 File Offset: 0x00026B08
    private void Awake()
    {
      
    }
    private void Update()
    {
        try
        {
            Text = GameObject.Find("LoadCanvas/Text1").GetComponent<Text>();

            Text.text = async.progress * 100 + "%";
            this.progressSlider.value = (int)async.progress;
        }
        catch
        {

        }
        if (async.progress * 100 >= 89)
            {
                this.Text.text = "点击继续";

                progressSlider.value = 100;
                async.allowSceneActivation = true;


            }
    
    }
    // Token: 0x0600056F RID: 1391 RVA: 0x00028A20 File Offset: 0x00026C20
    public async void loadout()
	{
		//await Task.Delay(3000);
		//SceneManager.MoveGameObjectToScene(GameObject.Find("LoadCanvas"), SceneManager.GetSceneByName(Loading.sceneName));
	}

	// Token: 0x06000570 RID: 1392 RVA: 0x00028A54 File Offset: 0x00026C54
	public static void LoadoutEnvet()
	{
		try
		{
			if (!(Loading.className == "") && Loading.className != null && !(Loading.methodName == "") && Loading.methodName != null)
			{
				Type type = Type.GetType(Loading.className);
				object obj = type.Assembly.CreateInstance(Loading.className);
				type.GetMethod(Loading.methodName).Invoke(obj, null);
			}
		}
		catch
		{
			Debug.LogWarning("Error");
		}
	}
    int nub = 0;
	// Token: 0x06000571 RID: 1393 RVA: 0x00028AE0 File Offset: 0x00026CE0
	private IEnumerator LoadScene()
	{
        
        this.async = SceneManager.LoadSceneAsync(Loading.sceneName);
        this.async.allowSceneActivation = false;
        yield return this.async;
        yield break;
    }

	// Token: 0x06000572 RID: 1394 RVA: 0x00028AF0 File Offset: 0x00026CF0
	public void ShowProgressBar()
	{
        try
        {
            this.progressSlider.value = 0f;
            this.Text.text = "0%";
        }catch
        { }
		if (Loading.sceneName != "")
		{
			base.StartCoroutine(this.LoadScene());
		}
	}

	// Token: 0x0400070B RID: 1803
	public static string className;

	// Token: 0x0400070C RID: 1804
	public static string methodName;

	// Token: 0x0400070D RID: 1805
	public static string loadsound;

	// Token: 0x0400070E RID: 1806
	public Scene menu;

	// Token: 0x0400070F RID: 1807
	public Slider progressSlider;

	// Token: 0x04000710 RID: 1808
	public Text Text;

	// Token: 0x04000711 RID: 1809
	private int nowProcess;

	// Token: 0x04000712 RID: 1810
	private AsyncOperation async;

	// Token: 0x04000713 RID: 1811
	public static string sceneName = "";
}
