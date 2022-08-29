using System;
using UnityEngine;

public class GUIShopDownloadPopup : MonoBehaviour
{
	public GUITransitionAnimator transitionAnimator;

	public GUIDropShadowText titleText;

	public Renderer sprite;

	public TextMesh progressText;

	public GameObject[] progressBarFills;

	public GUIButton cancelButton;

	public Action<bool, string> OnDownloadFinished;

	private Item m_item;

	private bool m_downloaded;

	public void SetItem(Item item)
	{
		m_item = item;
		AssetManager.UpdateTexture(sprite.material, item.iconTextureName);
		Invoke("Download", transitionAnimator.transitionInTime);
	}

	private void Download()
	{
		m_item.DownloadAssets(HandleDownloadFinished);
		cancelButton.gameObject.SetActive(true);
	}

	private void Cancel()
	{
		BundleManager.Instance.CancelDownloads();
	}

	private void HandleDownloadFinished(bool success, string error)
	{
		if (success)
		{
			m_downloaded = true;
			titleText.Text = "Installing...";
			progressText.text = string.Empty;
			cancelButton.gameObject.SetActive(false);
			Invoke("Preload", Time.deltaTime);
		}
		else
		{
			HandlePreloadFinished(false, error);
		}
	}

	private void Preload()
	{
		m_item.PreloadAssets(HandlePreloadFinished);
	}

	private void HandlePreloadFinished(bool success, string error)
	{
		transitionAnimator.Hide();
		SkiGameManager.Instance.PopupEnabled = false;
		OnDownloadFinished(success, error);
	}

	private void Update()
	{
		if (m_downloaded)
		{
			return;
		}
		BundleReference downloadingBundle = BundleManager.Instance.GetDownloadingBundle();
		if (downloadingBundle != null)
		{
			progressText.text = string.Format("{0:F2} MB / {1:F2} MB", (float)downloadingBundle.DownloadedByteSize / 1048576f, (float)downloadingBundle.TotalByteSize / 1048576f);
			float num = (float)downloadingBundle.DownloadedByteSize / (float)downloadingBundle.TotalByteSize;
			int num2 = Mathf.FloorToInt(num * (float)(progressBarFills.Length - 1));
			for (int i = 0; i < progressBarFills.Length; i++)
			{
				progressBarFills[i].SetActive(i == num2);
			}
		}
	}

	private void Awake()
	{
		cancelButton.gameObject.SetActive(false);
		GUIButton gUIButton = cancelButton;
		gUIButton.OnClick = (GUIButton.OnClickDelegate)Delegate.Combine(gUIButton.OnClick, new GUIButton.OnClickDelegate(Cancel));
		SkiGameManager.Instance.PopupEnabled = true;
	}

	private void OnDisable()
	{
		AssetManager.UnloadTexture(sprite.material);
	}
}
