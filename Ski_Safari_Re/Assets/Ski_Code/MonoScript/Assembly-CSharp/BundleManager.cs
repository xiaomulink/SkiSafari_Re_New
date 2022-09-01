using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BundleManager : MonoBehaviour
{
	private enum ProcessMode
	{
		None = 0,
		Download = 1,
		Preload = 2
	}

	[CompilerGenerated]
	private sealed class _003CFindBundle_003Ec__AnonStorey16
	{
		internal Bundle bundle;

		internal bool _003C_003Em__9(BundleReference b)
		{
			return b.Bundle == bundle;
		}
	}

	[CompilerGenerated]
	private sealed class _003CValidateBundleList_003Ec__AnonStorey17
	{
		internal Material material;

		internal bool _003C_003Em__A(KeyValuePair<Material, Bundle> p)
		{
			return p.Key == material;
		}
	}

	[CompilerGenerated]
	private sealed class _003CValidateBundleList_003Ec__AnonStorey18
	{
		internal Sound sound;

		internal bool _003C_003Em__B(KeyValuePair<Sound, Bundle> p)
		{
			return p.Key == sound;
		}
	}

	public static BundleManager Instance;

	public List<Bundle> bundles = new List<Bundle>();

	private List<BundleReference> m_bundleReferences = new List<BundleReference>();

	private ProcessMode m_processMode;

	private List<Bundle> m_queuedBundles = new List<Bundle>();

	private BundleReference m_activeBundleReference;

	private Action<bool, string> m_processCallback;

	private List<BundleReference> m_bundlesToUnload = new List<BundleReference>();

	public bool IsCached(List<Bundle> bundles)
	{
		foreach (Bundle bundle in bundles)
		{
			BundleReference bundleReference = FindBundle(bundle);
			if (bundleReference == null || !bundleReference.IsCached)
			{
				return false;
			}
		}
		return true;
	}

	public void Download(List<Bundle> bundles, Action<bool, string> callback)
	{
		ProcessBundles(ProcessMode.Download, bundles, callback);
	}

	public BundleReference GetDownloadingBundle()
	{
		if (m_processMode == ProcessMode.Download)
		{
			return m_activeBundleReference;
		}
		return null;
	}

	public void CancelDownloads()
	{
		if (m_processMode == ProcessMode.Download)
		{
			m_queuedBundles.Clear();
			if (m_activeBundleReference != null)
			{
				m_activeBundleReference.CancelDownload();
				m_activeBundleReference = null;
			}
			FinishProcessing(false, "Download cancelled");
		}
	}

	public void Preload(List<Bundle> bundles, Action<bool, string> callback)
	{
		ProcessBundles(ProcessMode.Preload, bundles, callback);
	}

	public void Load(List<Bundle> bundles)
	{
		List<BundleReference> list = new List<BundleReference>();
		foreach (Bundle bundle in bundles)
		{
			BundleReference bundleReference = FindBundle(bundle);
			if (bundleReference != null && bundleReference.IsReadyToLoad)
			{
				m_bundlesToUnload.Remove(bundleReference);
				list.Add(bundleReference);
			}
		}
		ProcessBundlesToUnload();
		foreach (BundleReference item in list)
		{
			item.Load();
		}
	}

	public void Unload(List<Bundle> bundles)
	{
		foreach (Bundle bundle in bundles)
		{
			BundleReference bundleReference = FindBundle(bundle);
			if (bundleReference != null)
			{
				m_bundlesToUnload.Add(bundleReference);
			}
		}
	}

	public BundleReference FindBundle(Bundle bundle)
	{
		_003CFindBundle_003Ec__AnonStorey16 _003CFindBundle_003Ec__AnonStorey = new _003CFindBundle_003Ec__AnonStorey16();
		_003CFindBundle_003Ec__AnonStorey.bundle = bundle;
		return m_bundleReferences.Find(_003CFindBundle_003Ec__AnonStorey._003C_003Em__9);
	}

	public void ReloadBundles()
	{
		foreach (BundleReference bundleReference in m_bundleReferences)
		{
			if (bundleReference.IsLoaded)
			{
				bundleReference.Reload();
			}
		}
	}

	public void ClearCachedBundles()
	{
		foreach (BundleReference bundleReference in m_bundleReferences)
		{
			bundleReference.ClearCache();
		}
	}

	public void ValidateBundleList(List<Bundle> bundles)
	{
		List<KeyValuePair<Material, Bundle>> list = new List<KeyValuePair<Material, Bundle>>();
		List<KeyValuePair<Sound, Bundle>> list2 = new List<KeyValuePair<Sound, Bundle>>();
		List<Bundle> list3 = new List<Bundle>();
		foreach (Bundle bundle in bundles)
		{
			if (list3.Contains(bundle))
			{
				continue;
			}
			foreach (Bundle.TextureAsset texture in bundle.library.textures)
			{
				_003CValidateBundleList_003Ec__AnonStorey17 _003CValidateBundleList_003Ec__AnonStorey = new _003CValidateBundleList_003Ec__AnonStorey17();
				Material[] materials = texture.materials;
				for (int i = 0; i < materials.Length; i++)
				{
					_003CValidateBundleList_003Ec__AnonStorey.material = materials[i];
					if (!list.Find(_003CValidateBundleList_003Ec__AnonStorey._003C_003Em__A).Key)
					{
						list.Add(new KeyValuePair<Material, Bundle>(_003CValidateBundleList_003Ec__AnonStorey.material, bundle));
					}
				}
			}
			foreach (Bundle.SoundAsset sound in bundle.library.sounds)
			{
				_003CValidateBundleList_003Ec__AnonStorey18 _003CValidateBundleList_003Ec__AnonStorey2 = new _003CValidateBundleList_003Ec__AnonStorey18();
				Sound[] sounds = sound.sounds;
				for (int j = 0; j < sounds.Length; j++)
				{
					_003CValidateBundleList_003Ec__AnonStorey2.sound = sounds[j];
					if (!list2.Find(_003CValidateBundleList_003Ec__AnonStorey2._003C_003Em__B).Key)
					{
						list2.Add(new KeyValuePair<Sound, Bundle>(_003CValidateBundleList_003Ec__AnonStorey2.sound, bundle));
					}
				}
			}
		}
	}

	private void ProcessBundlesToUnload()
	{
		foreach (BundleReference item in m_bundlesToUnload)
		{
			item.Unload();
		}
		m_bundlesToUnload.Clear();
	}

	private void ProcessBundles(ProcessMode processMode, List<Bundle> bundles, Action<bool, string> callback)
	{
		if (bundles.Count == 0)
		{
			callback(true, string.Empty);
			return;
		}
		if (m_processMode != 0)
		{
		}
		m_processMode = processMode;
		m_queuedBundles.AddRange(bundles);
		m_processCallback = (Action<bool, string>)Delegate.Combine(m_processCallback, callback);
		if (m_activeBundleReference == null)
		{
			ProcessNextBundle();
		}
	}

	private void ProcessNextBundle()
	{
		if (m_queuedBundles.Count > 0)
		{
			Bundle bundle = m_queuedBundles[0];
			m_queuedBundles.RemoveAt(0);
			m_activeBundleReference = FindBundle(bundle);
			if (m_activeBundleReference == null)
			{
				m_activeBundleReference = null;
				ProcessNextBundle();
				return;
			}
			switch (m_processMode)
			{
			case ProcessMode.Download:
				m_activeBundleReference.Download(OnBundleProcessed);
				break;
			case ProcessMode.Preload:
				m_activeBundleReference.Preload(OnBundleProcessed);
				break;
			}
		}
		else
		{
			FinishProcessing(true, string.Empty);
		}
	}

	private void FinishProcessing(bool success, string error)
	{
		m_processMode = ProcessMode.None;
		m_activeBundleReference = null;
		if (m_processCallback != null)
		{
			Action<bool, string> processCallback = m_processCallback;
			m_processCallback = null;
			processCallback(success, error);
		}
	}

	private void OnBundleProcessed(BundleReference bundleReference, bool success, string error)
	{
		if (success)
		{
			if (bundleReference == m_activeBundleReference)
			{
				m_activeBundleReference = null;
				ProcessNextBundle();
			}
			return;
		}
		if (bundleReference != m_activeBundleReference)
		{
		}
		m_activeBundleReference = null;
		m_queuedBundles.Clear();
		FinishProcessing(false, error);
	}

	private void Awake()
	{
		Instance = this;
		foreach (Bundle bundle in bundles)
		{
			BundleReference bundleReference = BundleReference.Create(bundle);
			if (bundleReference != null)
			{
				m_bundleReferences.Add(bundleReference);
				if (bundleReference.Bundle.resident)
				{
					AssetManager.AddResidentBundle(bundleReference);
				}
			}
		}
		AssetManager.FinishAddingResidentBundles();
	}

	private void OnDisable()
	{
		foreach (BundleReference bundleReference in m_bundleReferences)
		{
			bundleReference.Unload();
		}
	}
}
