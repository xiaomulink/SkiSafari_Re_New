using System;
using UnityEngine;

[ExecuteInEditMode]
public class TextureResourceLoader : MonoBehaviour
{
	[Serializable]
	public class Binding
	{
		public Material material;

		public string textureName;
	}

	public Binding[] bindings;

	public bool autoLoadTextures = true;

	private bool m_updated;

	public void LoadTextures()
	{
		if (!m_updated && Application.isPlaying && AssetManager.IsInstantiated)
		{
			Binding[] array = bindings;
			foreach (Binding binding in array)
			{
				binding.material.mainTexture = AssetManager.LoadAsset<Texture>(binding.textureName);
			}
			m_updated = true;
		}
	}

	private void Update()
	{
		if (autoLoadTextures)
		{
			LoadTextures();
		}
	}

	private void OnEnable()
	{
		m_updated = false;
	}

	private void OnDisable()
	{
		if (bindings == null)
		{
			return;
		}
		Binding[] array = bindings;
		foreach (Binding binding in array)
		{
			if ((bool)binding.material.mainTexture)
			{
				AssetManager.UnloadTexture(binding.material);
				binding.material.mainTexture = null;
			}
		}
	}
}
