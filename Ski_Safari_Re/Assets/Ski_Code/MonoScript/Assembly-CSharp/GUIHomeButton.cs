using UnityEngine;
using UnityEngine.SceneManagement;

public class GUIHomeButton : GUIButton
{
	public override void Click(Vector3 position)
	{
        if (!SkiGameManager.Instance.isOnline)
        {
            base.Click(position);
            Deactivate();
            if (SkiGameManager.Instance.ShowShop)
            {
                SkiGameManager.Instance.ShowShop = false;
            }
            else
            {
                SkiGameManager.Instance.Restart(SkiGameManager.RestartMode.Title);
            }
        }else
        {
            base.Click(position);
            SceneManager.LoadSceneAsync(2);
        }
	}
}
