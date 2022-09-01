using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BundleReference
{
	private enum State
	{
		None = 0,
		Downloading = 1,
		Preloading = 2,
		ReadyToLoad = 3,
		Loaded = 4
	}

	private Bundle m_bundle;

	private Bundle.PlatformDescriptor m_platform;

	private State m_state;

	private List<UnityEngine.Object> m_assets = new List<UnityEngine.Object>();

	private AssetQuality m_preloadQuality;

	private int m_downloadedByteSize;

	private int m_totalByteSize;

	private AssetBundle m_downloadedAssetBundle;

	private bool m_cancelDownload;

	private Action<BundleReference, bool, string> m_callback;

	[CompilerGenerated]
	private static Predicate<Bundle.PlatformDescriptor> _003C_003Ef__am_0024cacheA;

	public Bundle Bundle
	{
		get
		{
			return m_bundle;
		}
	}

	public bool IsCached
	{
		get
		{
			if (AssetManager.HDEnabled)
			{
				switch (m_platform.hd.mode)
				{
				case Bundle.Mode.Resource:
					return true;
				case Bundle.Mode.AssetBundle:
					if (!string.IsNullOrEmpty(m_platform.hd.remoteURL))
					{
						return File.Exists(GetCachedAssetBundlePath(m_platform.hd));
					}
					return true;
				}
			}
			switch (m_platform.sd.mode)
			{
			case Bundle.Mode.Resource:
				return true;
			case Bundle.Mode.AssetBundle:
				if (!string.IsNullOrEmpty(m_platform.sd.remoteURL))
				{
					return File.Exists(GetCachedAssetBundlePath(m_platform.sd));
				}
				return true;
			default:
				return false;
			}
		}
	}

	public int DownloadedByteSize
	{
		get
		{
			return m_downloadedByteSize;
		}
	}

	public int TotalByteSize
	{
		get
		{
			return m_totalByteSize;
		}
	}

	public bool IsReadyToLoad
	{
		get
		{
			Bundle.DefinitionDescriptor definitionDescriptor = ((!AssetManager.HDEnabled) ? m_platform.sd : m_platform.hd);
			switch (definitionDescriptor.mode)
			{
			case Bundle.Mode.Resource:
				return true;
			case Bundle.Mode.AssetBundle:
				return m_state == State.ReadyToLoad;
			default:
				return false;
			}
		}
	}

	public bool IsLoaded
	{
		get
		{
			return m_state >= State.Loaded;
		}
	}

	public static BundleReference Create(Bundle bundle)
	{
		List<Bundle.PlatformDescriptor> platforms = bundle.platforms;
		if (_003C_003Ef__am_0024cacheA == null)
		{
			_003C_003Ef__am_0024cacheA = _003CCreate_003Em__C;
		}
		Bundle.PlatformDescriptor platformDescriptor = platforms.Find(_003C_003Ef__am_0024cacheA);
		if (platformDescriptor != null)
		{
			BundleReference bundleReference = new BundleReference();
			bundleReference.m_bundle = bundle;
			bundleReference.m_platform = platformDescriptor;
			return bundleReference;
		}
		return null;
	}

	public void Download(Action<BundleReference, bool, string> callback)
	{
		switch (m_state)
		{
		case State.None:
			m_callback = callback;
			m_cancelDownload = false;
			DoDownload(AssetManager.Quality);
			break;
		case State.Downloading:
			callback(this, false, "Already downloading");
			break;
		default:
			callback(this, true, string.Empty);
			break;
		}
	}

	public void CancelDownload()
	{
		switch (m_state)
		{
		case State.Downloading:
		case State.Preloading:
			m_cancelDownload = true;
			break;
		case State.ReadyToLoad:
			if ((bool)m_downloadedAssetBundle)
			{
				m_downloadedAssetBundle.Unload(false);
				m_downloadedAssetBundle = null;
			}
			m_state = State.None;
			break;
		case State.Loaded:
			Unload();
			break;
		}
	}

	public void Preload(Action<BundleReference, bool, string> callback)
	{
		switch (m_state)
		{
		case State.None:
			m_callback = callback;
			m_cancelDownload = false;
			DoPreload(AssetManager.Quality);
			break;
		case State.Downloading:
			callback(this, false, "Bundle is downloading");
			break;
		case State.Preloading:
			callback(this, false, "Already preloading");
			break;
		case State.ReadyToLoad:
		case State.Loaded:
			callback(this, true, string.Empty);
			break;
		}
	}

	public void Load()
	{
		switch (m_state)
		{
		case State.ReadyToLoad:
			if ((bool)m_downloadedAssetBundle)
			{
				LoadFromAssetBundle();
			}
			else
			{
				LoadFromResources();
			}
			m_state = State.Loaded;
			break;
		}
	}

	public void Reload()
	{
		if (m_state != State.Loaded)
		{
			return;
		}
		AssetQuality quality = AssetManager.Quality;
		if (m_preloadQuality != quality)
		{
			Bundle.DefinitionDescriptor definitionDescriptor = ((quality != AssetQuality.HD) ? m_platform.sd : m_platform.hd);
			if (definitionDescriptor.mode == Bundle.Mode.Resource)
			{
				UnloadAssets();
				m_preloadQuality = quality;
				LoadFromResources();
			}
		}
	}

	public T LoadResource<T>(string assetName, AssetQuality quality) where T : UnityEngine.Object
	{
		switch (quality)
		{
		case AssetQuality.HD:
			if (m_platform.hd.mode == Bundle.Mode.Resource)
			{
				T val2 = Resources.Load(Bundle.name + "/SD/" + assetName, typeof(T)) as T;
				if ((bool)(UnityEngine.Object)val2)
				{
					return val2;
				}
			}
			break;
		case AssetQuality.SD:
			if (m_platform.sd.mode == Bundle.Mode.Resource)
			{
				T val = Resources.Load(Bundle.name + "/SD/" + assetName, typeof(T)) as T;
				if ((bool)(UnityEngine.Object)val)
				{
					return val;
				}
			}
			break;
		}
		return (T)default(T);
	}

	public void Unload()
	{
		State state = m_state;
		if (state == State.Loaded)
		{
			UnloadAssets();
			m_state = State.None;
		}
	}

	public void ClearCache()
	{
		string cachedAssetBundlePath = GetCachedAssetBundlePath(m_platform.sd);
		if (File.Exists(cachedAssetBundlePath))
		{
			File.Delete(cachedAssetBundlePath);
		}
		string cachedAssetBundlePath2 = GetCachedAssetBundlePath(m_platform.hd);
		if (File.Exists(cachedAssetBundlePath2))
		{
			File.Delete(cachedAssetBundlePath2);
		}
	}

	private T LoadAssetBundleAsset<T>(string assetName, int initialRefCount) where T : UnityEngine.Object
	{
		T val = m_downloadedAssetBundle.LoadAsset<T>(assetName);
		if ((UnityEngine.Object)val == (UnityEngine.Object)default(UnityEngine.Object))
		{
			return (T)default(T);
		}
		m_assets.Add(val);
		AssetManager.AddBundleAsset(val, this, initialRefCount);
		return val;
	}

	private void LoadFromAssetBundle()
	{
		foreach (Bundle.TextureAsset texture2 in m_bundle.library.textures)
		{
			Texture texture = LoadAssetBundleAsset<Texture>(texture2.name, texture2.materials.Length + 1);
			if (texture != null)
			{
				Material[] materials = texture2.materials;
				foreach (Material material in materials)
				{
					material.mainTexture = texture;
				}
			}
		}
		foreach (Bundle.SoundAsset sound2 in m_bundle.library.sounds)
		{
			AudioClip audioClip = LoadAssetBundleAsset<AudioClip>(sound2.name, sound2.sounds.Length + 1);
			if (audioClip != null)
			{
				Sound[] sounds = sound2.sounds;
				foreach (Sound sound in sounds)
				{
					sound.clip = audioClip;
				}
			}
		}
		m_downloadedAssetBundle.Unload(false);
		m_downloadedAssetBundle = null;
	}

	private T LoadResourceAsset<T>(string assetName, int initialRefCount) where T : UnityEngine.Object
	{
		string path = string.Format("{0}/{1}/{2}", m_bundle.name, m_preloadQuality, assetName);
		T val = Resources.Load(path, typeof(T)) as T;
		if ((UnityEngine.Object)val == (UnityEngine.Object)default(UnityEngine.Object) && m_preloadQuality != 0)
		{
			path = string.Format("{0}/{1}/{2}", m_bundle.name, AssetQuality.SD, assetName);
			val = Resources.Load(path, typeof(T)) as T;
		}
		if ((UnityEngine.Object)val == (UnityEngine.Object)default(UnityEngine.Object))
		{
			return (T)default(T);
		}
		m_assets.Add(val);
		AssetManager.AddBundleAsset(val, this, initialRefCount);
		return val;
	}

	private void LoadFromResources()
	{
		foreach (Bundle.TextureAsset texture2 in m_bundle.library.textures)
		{
			Texture texture = LoadResourceAsset<Texture>(texture2.name, texture2.materials.Length + 1);
			if (texture != null)
			{
				Material[] materials = texture2.materials;
				foreach (Material material in materials)
				{
					material.mainTexture = texture;
				}
			}
		}
		foreach (Bundle.SoundAsset sound2 in m_bundle.library.sounds)
		{
			AudioClip audioClip = LoadResourceAsset<AudioClip>(sound2.name, sound2.sounds.Length + 1);
			if (audioClip != null)
			{
				Sound[] sounds = sound2.sounds;
				foreach (Sound sound in sounds)
				{
					sound.clip = audioClip;
				}
			}
		}
	}

	private void UnloadAssets()
	{
		foreach (Bundle.TextureAsset texture in m_bundle.library.textures)
		{
			AssetManager.DecreaseReferenceCount(texture.name, texture.materials.Length + 1);
			Material[] materials = texture.materials;
			foreach (Material material in materials)
			{
				material.mainTexture = null;
			}
		}
		foreach (Bundle.SoundAsset sound2 in m_bundle.library.sounds)
		{
			AssetManager.DecreaseReferenceCount(sound2.name, sound2.sounds.Length + 1);
			Sound[] sounds = sound2.sounds;
			foreach (Sound sound in sounds)
			{
				sound.clip = null;
			}
		}
	}

	private void HandleDownloaded()
	{
		m_state = State.None;
		Action<BundleReference, bool, string> callback = m_callback;
		m_callback = null;
		callback(this, true, string.Empty);
	}

	private void HandleReadyToLoad()
	{
		m_state = State.ReadyToLoad;
		Action<BundleReference, bool, string> callback = m_callback;
		m_callback = null;
		callback(this, true, string.Empty);
	}

	private void HandleError(string error)
	{
		m_state = State.None;
		Action<BundleReference, bool, string> callback = m_callback;
		m_callback = null;
		callback(this, false, error);
	}

	private void DoDownload(AssetQuality quality)
	{
		Bundle.DefinitionDescriptor definitionDescriptor = ((quality != AssetQuality.HD) ? m_platform.sd : m_platform.hd);
		switch (definitionDescriptor.mode)
		{
		case Bundle.Mode.Excluded:
			if (quality == AssetQuality.HD)
			{
				DoDownload(AssetQuality.SD);
			}
			break;
		case Bundle.Mode.Resource:
			HandleDownloaded();
			break;
		case Bundle.Mode.AssetBundle:
			if (!string.IsNullOrEmpty(definitionDescriptor.remoteURL) && !File.Exists(GetCachedAssetBundlePath(definitionDescriptor)))
			{
				BundleManager.Instance.StartCoroutine(DoDownloadFromURL(definitionDescriptor, definitionDescriptor.remoteURL));
			}
			else
			{
				HandleDownloaded();
			}
			break;
		}
	}

	private void DoPreload(AssetQuality quality)
	{
		m_preloadQuality = quality;
		Bundle.DefinitionDescriptor definitionDescriptor = ((quality != AssetQuality.HD) ? m_platform.sd : m_platform.hd);
		switch (definitionDescriptor.mode)
		{
		case Bundle.Mode.Excluded:
			if (quality == AssetQuality.HD)
			{
				DoPreload(AssetQuality.SD);
			}
			break;
		case Bundle.Mode.Resource:
			HandleReadyToLoad();
			break;
		case Bundle.Mode.AssetBundle:
			if (!string.IsNullOrEmpty(definitionDescriptor.remoteURL))
			{
				if (!File.Exists(GetCachedAssetBundlePath(definitionDescriptor)))
				{
					HandleError("Download was not successful");
				}
				else
				{
					BundleManager.Instance.StartCoroutine(DoPreloadFromURL(definitionDescriptor, "file://" + GetCachedAssetBundlePath(definitionDescriptor)));
				}
			}
			else
			{
				BundleManager.Instance.StartCoroutine(DoPreloadFromURL(definitionDescriptor, "file://" + GetStreamingAssetBundlePath(definitionDescriptor)));
			}
			break;
		}
	}

	private IEnumerator DoDownloadFromURL(Bundle.DefinitionDescriptor definition, string url)
	{
		m_state = State.Downloading;
		m_downloadedByteSize = 0;
		m_totalByteSize = definition.size;
		using (WWW request = new WWW(url))
		{
			while (!request.isDone && string.IsNullOrEmpty(request.error))
			{
				yield return 0;
				m_downloadedByteSize = Mathf.RoundToInt(request.progress * (float)m_totalByteSize);
				if (m_cancelDownload)
				{
					m_state = State.None;
					yield break;
				}
			}
			if (string.IsNullOrEmpty(request.error))
			{
				try
				{
					string cachedAssetBundlePath = GetCachedAssetBundlePath(definition);
					Directory.CreateDirectory(Path.GetDirectoryName(cachedAssetBundlePath));
					File.WriteAllBytes(cachedAssetBundlePath, request.bytes);
					HandleDownloaded();
					yield break;
				}
				catch (Exception e)
				{
					HandleError(e.Message);
					yield break;
				}
			}
			HandleError(request.error);
		}
	}

	private IEnumerator DoPreloadFromURL(Bundle.DefinitionDescriptor definition, string url)
	{
		m_state = State.Preloading;
		using (WWW request = WWW.LoadFromCacheOrDownload(url, definition.version))
		{
			while (!request.isDone && string.IsNullOrEmpty(request.error))
			{
				yield return 0;
			}
			if (string.IsNullOrEmpty(request.error))
			{
				PlayerPrefs.SetInt(definition.assetBundleName, 1);
				m_downloadedAssetBundle = request.assetBundle;
				HandleReadyToLoad();
			}
			else
			{
				HandleError(request.error);
			}
		}
	}

	private string GetCachedAssetBundlePath(Bundle.DefinitionDescriptor definition)
	{
		return Application.temporaryCachePath + "/Bundles/" + definition.assetBundleName;
	}

	private string GetStreamingAssetBundlePath(Bundle.DefinitionDescriptor definition)
	{
		return Application.streamingAssetsPath + "/" + definition.assetBundleName;
	}

	private string GetURL(Bundle.DefinitionDescriptor definition)
	{
		if (!string.IsNullOrEmpty(definition.remoteURL))
		{
			return definition.remoteURL;
		}
		return string.Format("file://{0}/{1}", Application.streamingAssetsPath, definition.assetBundleName);
	}

	[CompilerGenerated]
	private static bool _003CCreate_003Em__C(Bundle.PlatformDescriptor p)
	{
		return p.platform == PlatformUtils.TargetPlatform;
	}
}
