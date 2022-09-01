using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
[AddComponentMenu("Sprites/Animated Sprite")]
[RequireComponent(typeof(MeshFilter))]
public class AnimatedSprite : Sprite
{
	public bool realtime;

	public SpriteAnimation[] animations;

	public string initialAnimationName;

	private SpriteAnimation m_animation;

	private int m_animationFrame;

	private float m_animationFrameTimer;

	private float m_animationSpeed = 1f;

	private string m_nextAnimationName;

	private Vector3 m_initialLocalScale;

	private bool m_requiresFrameUpdate;

	private float m_lastUpdateTime;

	public string CurrentAnimation
	{
		get
		{
			return (m_animation == null) ? string.Empty : m_animation.name;
		}
	}

	public bool PlayAnimation(string name)
	{
		return PlayAnimation(name, false);
	}

	public bool PlayAnimation(string name, bool forceReset)
	{
		m_nextAnimationName = string.Empty;
		if (!forceReset && m_animation != null && m_animation.name == name)
		{
			return true;
		}
		if (animations == null)
		{
			return false;
		}
		SpriteAnimation[] array = animations;
		foreach (SpriteAnimation spriteAnimation in array)
		{
			if (!(spriteAnimation.name == name))
			{
				continue;
			}
			if (m_animation != null && (bool)m_animation.sound && GetComponent<AudioSource>().clip == m_animation.sound.clip)
			{
				GetComponent<AudioSource>().Stop();
			}
			m_animation = spriteAnimation;
			m_animationFrameTimer = 0f;
			if (!spriteAnimation.synchronise || m_animationFrame >= m_animation.frameCount)
			{
				m_animationFrame = spriteAnimation.initialFrame;
			}
			if (m_animationFrame >= m_animation.bakedMeshes.Length)
			{
				Vector3 initialLocalScale = m_initialLocalScale;
				if (!Mathf.Approximately(rect.width, 0f))
				{
					initialLocalScale.x *= m_animation.rect.width / rect.width;
				}
				if (!Mathf.Approximately(rect.height, 0f))
				{
					initialLocalScale.y *= m_animation.rect.height / rect.height;
				}
				base.transform.localScale = initialLocalScale;
			}
			UpdateFrame();
			if ((bool)spriteAnimation.sound && (bool)spriteAnimation.sound.clip && (bool)SoundManager.Instance && SoundManager.Instance.SFXEnabled)
			{
				GetComponent<AudioSource>().clip = spriteAnimation.sound.clip;
				GetComponent<AudioSource>().loop = spriteAnimation.looping;
				GetComponent<AudioSource>().Play();
			}
			return true;
		}
		return false;
	}

	public void QueueAnimation(string name)
	{
		m_nextAnimationName = name;
	}

	public void StopAnimation()
	{
		m_animation = null;
		if ((bool)bakedMesh)
		{
			base.MeshFilter.mesh = bakedMesh;
			return;
		}
		base.transform.localScale = m_initialLocalScale;
		UpdateUVs(rect);
	}

	public void SetAnimationSpeed(float animationSpeed)
	{
		m_animationSpeed = animationSpeed;
	}

	public void PrevFrame()
	{
		if (m_animation != null)
		{
			m_animationFrame--;
			if (m_animationFrame < 0)
			{
				m_animationFrame = m_animation.frameCount - 1;
			}
		}
	}

	public void NextFrame()
	{
		if (m_animation == null)
		{
			return;
		}
		m_animationFrame++;
		if (m_animationFrame >= m_animation.frameCount)
		{
			if (!string.IsNullOrEmpty(m_nextAnimationName))
			{
				PlayAnimation(m_nextAnimationName);
				m_requiresFrameUpdate = false;
			}
			else if (m_animation.looping)
			{
				m_animationFrame = 0;
				m_requiresFrameUpdate = true;
			}
			else if (m_animation.hold)
			{
				m_animationFrame = m_animation.frameCount - 1;
			}
			else
			{
				PlayAnimation(initialAnimationName);
				m_requiresFrameUpdate = false;
			}
		}
		else
		{
			m_requiresFrameUpdate = true;
		}
	}

	public void UpdateFrame()
	{
		if (m_animation == null)
		{
			return;
		}
		if (m_animationFrame < m_animation.bakedMeshes.Length)
		{
			base.MeshFilter.mesh = m_animation.bakedMeshes[m_animationFrame];
		}
		else
		{
			Rect newRect = m_animation.rect;
			for (int i = 0; i < m_animation.frameCount; i++)
			{
				newRect.x += newRect.width;
				if (newRect.x + newRect.width > base.TextureWidth)
				{
					newRect.x = 0f;
					newRect.y += newRect.height;
				}
			}
			UpdateUVs(newRect);
		}
		if (m_animation.nodeTracks.Length > 0)
		{
			UpdateNodeTracks();
		}
		m_requiresFrameUpdate = false;
	}

	private void UpdateNodeTracks()
	{
		SpriteNodeTrack[] nodeTracks = m_animation.nodeTracks;
		foreach (SpriteNodeTrack spriteNodeTrack in nodeTracks)
		{
			SpriteNodeKey[] keys = spriteNodeTrack.keys;
			if (m_animationFrame < keys.Length && (bool)spriteNodeTrack.node)
			{
				int num = m_animationFrame + 1;
				if (num == keys.Length)
				{
					num = ((!m_animation.looping) ? m_animationFrame : 0);
				}
				float t = m_animationFrameTimer * m_animation.fps;
				spriteNodeTrack.node.localPosition = Vector3.Lerp(keys[m_animationFrame].pos, keys[num].pos, t);
				spriteNodeTrack.node.localRotation = Quaternion.Slerp(keys[m_animationFrame].rot, keys[num].rot, t);
			}
		}
	}

	protected override void Awake()
	{
		base.Awake();
		m_initialLocalScale = base.transform.localScale;
	}

	private void OnEnable()
	{
		m_lastUpdateTime = Time.realtimeSinceStartup;
		if (!string.IsNullOrEmpty(initialAnimationName))
		{
			PlayAnimation(initialAnimationName, true);
		}
	}

	private void Update()
	{
		float num = Time.deltaTime;
		if (realtime)
		{
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			num = realtimeSinceStartup - m_lastUpdateTime;
			m_lastUpdateTime = realtimeSinceStartup;
		}
		if (m_animation != null && m_animation.fps > 0f)
		{
			m_animationFrameTimer += m_animationSpeed * num;
			float num2 = 1f / m_animation.fps;
			m_requiresFrameUpdate = false;
			while (m_animationFrameTimer > num2)
			{
				m_animationFrameTimer -= num2;
				NextFrame();
			}
			if (m_requiresFrameUpdate)
			{
				UpdateFrame();
			}
			else if (m_animation.nodeTracks.Length > 0)
			{
				UpdateNodeTracks();
			}
		}
		else if (!string.IsNullOrEmpty(m_nextAnimationName))
		{
			PlayAnimation(m_nextAnimationName);
		}
	}
}
