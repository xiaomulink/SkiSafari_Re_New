using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class AssetManager : MonoBehaviour
{
	protected class ReferencedAsset
	{
		public UnityEngine.Object asset;

		public int refCount;

		public BundleReference bundleReference;
	}

	private static string HDEnabledKey = "hd_enabled";

	private static AssetManager s_instance;

	private List<BundleReference> m_residentBundles = new List<BundleReference>();

	private Dictionary<string, ReferencedAsset> m_referencedAssets = new Dictionary<string, ReferencedAsset>();

	[CompilerGenerated]
	private static Comparison<ReferencedAsset> _003C_003Ef__am_0024cache4;

	public static bool IsInstantiated
	{
		get
		{
			return s_instance != null;
		}
	}

	public static bool CanEnableHD
	{
		get
		{
			int num = 800;
			return Mathf.Min(Screen.width, Screen.height) > num;
		}
	}

	public static AssetQuality Quality
	{
		get
		{
			return HDEnabled ? AssetQuality.SD : AssetQuality.SD;
		}
	}

	public static bool HDEnabled
	{
		get
		{
			return PlayerPrefs.GetInt(HDEnabledKey) == 1;
		}
		set
		{
			if (value != HDEnabled)
			{
				s_instance.SwitchAssetQuality(value ? AssetQuality.HD : AssetQuality.SD);
				BundleManager.Instance.ReloadBundles();
			}
		}
	}

	public static T LoadAsset<T>(string assetName) where T : UnityEngine.Object
	{
		if (!s_instance)
		{
			return (T)default(T);
		}
		if (string.IsNullOrEmpty(assetName))
		{
			return (T)default(T);
		}
		ReferencedAsset value;
		if (s_instance.m_referencedAssets.TryGetValue(assetName, out value))
		{
			if (!(value.asset is T))
			{
				return (T)default(T);
			}
			value.refCount++;
			return value.asset as T;
		}
		T val = s_instance.LoadResource<T>(assetName);
		if ((bool)(UnityEngine.Object)val)
		{
			ReferencedAsset referencedAsset = new ReferencedAsset();
			referencedAsset.asset = val;
			referencedAsset.refCount = 1;
			value = referencedAsset;
			s_instance.m_referencedAssets[assetName] = value;
		}
		return val;
	}

	public static void UnloadAsset<T>(T asset) where T : UnityEngine.Object
	{
		if (!s_instance || !(UnityEngine.Object)asset)
		{
			return;
		}
		ReferencedAsset value;
		if (s_instance.m_referencedAssets.TryGetValue(asset.name, out value))
		{
			if (value.asset is T && --value.refCount <= 0)
			{
				s_instance.m_referencedAssets.Remove(asset.name);
				Resources.UnloadAsset(asset);
			}
		}
		else
		{
			Resources.UnloadAsset(asset);
		}
	}

	public static void DecreaseReferenceCount(string assetName, int amount = 1)
	{
		ReferencedAsset value;
		if ((bool)s_instance && !string.IsNullOrEmpty(assetName) && s_instance.m_referencedAssets.TryGetValue(assetName, out value))
		{
			value.refCount -= amount;
			if (value.refCount <= 0)
			{
				s_instance.m_referencedAssets.Remove(assetName);
				Resources.UnloadAsset(value.asset);
			}
		}
	}

	public static void UpdateTexture(Renderer renderer, string textureName)
	{
		Material material = renderer.material;
		UpdateTexture(material, textureName);
		renderer.material = material;
	}

	public static void UpdateTexture(Material material, string textureName)
	{
		if ((bool)material.mainTexture)
		{
			if (material.mainTexture.name == textureName)
			{
				return;
			}
			UnloadAsset(material.mainTexture);
		}
		material.mainTexture = LoadAsset<Texture>(textureName);
	}

	public static void UnloadTexture(Renderer renderer)
	{
		if (renderer != null)
		{
			UnloadAsset(renderer.material.mainTexture);
			renderer.material.mainTexture = null;
		}
	}

	public static void UnloadTexture(Material material)
	{
		UnloadAsset(material.mainTexture);
		material.mainTexture = null;
	}

	public static void UpdateSound(Sound sound, string clipName)
	{
		if ((bool)sound.clip)
		{
			if (sound.clip.name == clipName)
			{
				return;
			}
			UnloadAsset(sound.clip);
		}
		sound.clip = LoadAsset<AudioClip>(clipName);
	}

	public static void DumpReferencedAssets()
	{
		string text = string.Empty;
		int num = 0;
		List<ReferencedAsset> list = new List<ReferencedAsset>(s_instance.m_referencedAssets.Values);
		if (_003C_003Ef__am_0024cache4 == null)
		{
			_003C_003Ef__am_0024cache4 = _003CDumpReferencedAssets_003Em__8;
		}
		list.Sort(_003C_003Ef__am_0024cache4);
		foreach (ReferencedAsset item in list)
		{
			int runtimeMemorySize = UnityEngine.Profiling.Profiler.GetRuntimeMemorySize(item.asset);
			text += string.Format("[{0}] ({1}) {2}: {3}{4} ({5:F0} kB)\n", (item.bundleReference == null) ? "Common" : item.bundleReference.Bundle.name, item.asset.GetType().Name, item.asset.name, item.refCount, (!(item.asset == null)) ? string.Empty : " -- ERROR: Missing asset", runtimeMemorySize / 1024);
			num += runtimeMemorySize;
		}
		string text2 = string.Format("Referenced assets (Total size: {0} kB)\n{1}", num / 1024, text);
	}

	public static void AddResidentBundle(BundleReference bundle)
	{
		s_instance.m_residentBundles.Add(bundle);
	}

	public static void FinishAddingResidentBundles()
	{
		if (HDEnabled)
		{
			s_instance.SwitchAssetQuality(AssetQuality.HD);
		}
	}

	private T LoadResource<T>(string assetName) where T : UnityEngine.Object
	{
		if (HDEnabled)
		{
			T val = LoadResource<T>(assetName, AssetQuality.SD);
			if ((bool)(UnityEngine.Object)val)
			{
				return val;
			}
		}
		return LoadResource<T>(assetName, AssetQuality.SD);
	}

	private T LoadResource<T>(string assetName, AssetQuality quality) where T : UnityEngine.Object
	{
		foreach (BundleReference residentBundle in m_residentBundles)
		{
			T val = residentBundle.LoadResource<T>(assetName, quality);
			if ((bool)(UnityEngine.Object)val)
			{
				return val;
			}
		}
		return (T)default(T);
	}

	private bool IsHDEnabledByDefault()
	{
		return CanEnableHD;
	}

	private void SwitchAssetQuality(AssetQuality quality)
	{
		UnityEngine.Object[] array = Resources.FindObjectsOfTypeAll(typeof(Material));
		UnityEngine.Object[] array2 = array;
		for (int i = 0; i < array2.Length; i++)
		{
			Material material = (Material)array2[i];
			if (!material.HasProperty("_MainTex") || !material.mainTexture)
			{
				continue;
			}
			Texture texture = LoadResource<Texture>(material.mainTexture.name, quality);
			if ((bool)texture)
			{
				ReferencedAsset value;
				if (m_referencedAssets.TryGetValue(material.mainTexture.name, out value) && value.asset != texture)
				{
					Resources.UnloadAsset(value.asset);
					value.asset = texture;
				}
				material.mainTexture = texture;
			}
		}
		PlayerPrefs.SetInt(HDEnabledKey, (quality == AssetQuality.HD) ? 1 : 0);
	}

	public static void AddBundleAsset(UnityEngine.Object asset, BundleReference bundleReference, int initialRefCount)
	{
		ReferencedAsset value;
		if (s_instance.m_referencedAssets.TryGetValue(asset.name, out value))
		{
			value.bundleReference = bundleReference;
			value.refCount += initialRefCount;
			return;
		}
		ReferencedAsset referencedAsset = new ReferencedAsset();
		referencedAsset.asset = asset;
		referencedAsset.refCount = initialRefCount;
		referencedAsset.bundleReference = bundleReference;
		value = referencedAsset;
		s_instance.m_referencedAssets[asset.name] = value;
	}

	private void Awake()
	{
		s_instance = this;
		if (!PlayerPrefs.HasKey(HDEnabledKey))
		{
			PlayerPrefs.SetInt(HDEnabledKey, IsHDEnabledByDefault() ? 1 : 0);
		}
		else if (!CanEnableHD)
		{
			PlayerPrefs.SetInt(HDEnabledKey, 0);
		}
	}

	private void OnDisable()
	{
	}

	[CompilerGenerated]
	private static int _003CDumpReferencedAssets_003Em__8(ReferencedAsset x, ReferencedAsset y)
	{
		if (x.bundleReference == null)
		{
			return 1;
		}
		if (y.bundleReference == null)
		{
			return -1;
		}
		int num = x.bundleReference.Bundle.name.CompareTo(y.bundleReference.Bundle.name);
		if (num == 0)
		{
			return x.asset.name.CompareTo(y.asset.name);
		}
		return num;
	}
}
