using System.Collections;
using UnityEngine;

public class GUITitle : MonoBehaviour
{
	public Renderer worldOverlay;

	public ParticleSystem worldParticles;

	public Renderer titleImage;

	public Renderer titleOverlay;

	public Renderer demoOverlay;

	public Renderer freeOverlay;

	public Sound demoShowSound;

	public float fadeInDuration = 1f;

	public float initialCutoff = 0.05f;

	public GUITransitionAnimator transitionAnimator;

	private bool m_fadingIn;

	private bool m_finishedTransition;

	private float m_titleErodeDelay = 2f;

	private float m_titleErodeDuration = 1.5f;

	private ParticleSystem m_titleParticles;

	private Sound m_titleParticlesSound;

	public bool FinishedTransition
	{
		get
		{
			return m_finishedTransition;
		}
	}

	public void SnapToWhite()
	{
		base.gameObject.SetActive(true);
	}

	public void SnapToIdle()
	{
		StopAllCoroutines();
		worldOverlay.enabled = false;
		titleOverlay.enabled = false;
		worldParticles.enableEmission = false;
		m_titleParticles.enableEmission = false;
	}

	public void FadeIn()
	{
		m_fadingIn = true;
	}

	public void SetTitleParticles(ParticleSystem prefab, Sound sound, float erodeDelay, float erodeDuration)
	{
		if (!m_titleParticles || prefab.name != m_titleParticles.name)
		{
			if ((bool)m_titleParticles)
			{
				Object.Destroy(m_titleParticles.gameObject);
			}
			m_titleParticles = Object.Instantiate(prefab, prefab.transform.localPosition, prefab.transform.localRotation) as ParticleSystem;
			m_titleParticles.transform.parent = base.transform.parent;
			m_titleParticles.name = prefab.name;
			m_titleParticles.enableEmission = false;
		}
		m_titleParticlesSound = sound;
		m_titleErodeDelay = erodeDelay;
		m_titleErodeDuration = erodeDuration;
	}

	public void ShowTitleErrode()
	{
		if (m_finishedTransition)
		{
			StopAllCoroutines();
			ResetTitleOverlay();
			StartCoroutine(ProcessTitleErrode());
		}
	}

	public void ResetTitleOverlay()
	{
		titleOverlay.enabled = true;
		titleOverlay.sharedMaterial.SetColor("_TintColor", Color.white);
		titleOverlay.sharedMaterial.SetFloat("_Cutoff", initialCutoff);
		if ((bool)m_titleParticles)
		{
			m_titleParticles.enableEmission = false;
		}
		demoOverlay.enabled = false;
		freeOverlay.enabled = false;
	}

	private IEnumerator ProcessFadingIn()
	{
		m_fadingIn = false;
		m_finishedTransition = false;
		worldOverlay.enabled = true;
		Color worldOverlayColor = worldOverlay.sharedMaterial.GetColor("_TintColor");
		worldOverlayColor.a = 1f;
		worldOverlay.sharedMaterial.SetColor("_TintColor", worldOverlayColor);
		worldParticles.enableEmission = false;
		ResetTitleOverlay();
		worldParticles.enableEmission = true;
		yield return 0;
		worldParticles.enableEmission = false;
		while (!m_fadingIn)
		{
			yield return 0;
		}
		yield return 0;
		yield return 0;
		float fadeInTimer = fadeInDuration;
		while (fadeInTimer > 0f)
		{
			worldOverlayColor.a = fadeInTimer / fadeInDuration;
			worldOverlay.sharedMaterial.SetColor("_TintColor", worldOverlayColor);
			fadeInTimer -= Time.deltaTime;
			yield return 0;
		}
		worldOverlay.enabled = false;
		m_finishedTransition = true;
		StartCoroutine(ProcessTitleErrode());
	}

	private IEnumerator ProcessTitleErrode()
	{
		while (!SkiGameManager.Instance.TitleScreenActive)
		{
			yield return 0;
		}
		yield return new WaitForSeconds(m_titleErodeDelay);
		if (transitionAnimator.IsShowing && (bool)m_titleParticles)
		{
			m_titleParticles.enableEmission = true;
			yield return 0;
			m_titleParticles.enableEmission = false;
		}
		SoundManager.Instance.PlaySound(m_titleParticlesSound);
		float titleErodeTimer = 0f;
		while (titleErodeTimer < m_titleErodeDuration)
		{
			titleOverlay.sharedMaterial.SetFloat("_Cutoff", Mathf.Lerp(initialCutoff, 1f, titleErodeTimer / m_titleErodeDuration));
			titleErodeTimer += Time.deltaTime;
			yield return 0;
		}
		titleOverlay.enabled = false;
		if (transitionAnimator.IsShowing && AchievementManager.Instance.IsDemo)
		{
			demoOverlay.enabled = true;
			Go.killAllTweensWithTarget(demoOverlay.gameObject.transform);
			Go.to(demoOverlay.gameObject.transform, 1f, new GoTweenConfig().scale(0.5f, true).setEaseType(GoEaseType.ElasticPunch));
			SoundManager.Instance.PlaySound(demoShowSound);
		}
		SocialManager.Instance.Authenticate(false);
	}

	private void OnEnable()
	{
		worldOverlay.gameObject.SetActive(true);
		StartCoroutine(ProcessFadingIn());
	}

	private void OnDisable()
	{
	}

	private void OnDestroy()
	{
		if ((bool)titleOverlay)
		{
			titleOverlay.sharedMaterial.SetColor("_TintColor", Color.white);
			titleOverlay.sharedMaterial.SetFloat("_Cutoff", initialCutoff);
		}
		if ((bool)worldOverlay)
		{
			Color color = worldOverlay.sharedMaterial.GetColor("_TintColor");
			color.a = 1f;
			worldOverlay.sharedMaterial.SetColor("_TintColor", color);
		}
	}
}
