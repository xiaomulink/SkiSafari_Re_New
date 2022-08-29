using System.Collections;
using UnityEngine;

public class Launcher : MonoBehaviour
{
	public Transform characters;

	public float charactersStartPos = -20f;

	public float speed = 20f;

	public float duration = 2f;

	public Transform sleepyLogo;

	public float sleepyLogoShowDuration = 0.5f;

	public GoEaseType sleepyLogoShowEaseType = GoEaseType.ElasticOut;

	private float m_timer;

	private bool m_sleepyLogoDropped;

	private void Awake()
	{
		switch (SystemInfo.graphicsDeviceName)
		{
		case "PowerVR SGX 544MP":
		case "Mali-T628":
		case "Mali-450 MP":
		case "Adreno (TM) 330":
		case "Vivante GC4000":
		case "Mali-T604":
		case "NVIDIA Tegra":
		case "Mali-400 MP":
		case "Adreno (TM) 320":
		case "PowerVR SGX 540":
		case "PowerVR SGX 544MP2":
		case "GC2000 core":
		case "NVIDIA Tegra 3":
		case "Vivante GC1000":
		case "PowerVR SGX 544":
		case "Adreno (TM) 305":
		case "Immersion.16":
		case "PowerVR SGX 543MP":
		case "GC1000 core":
			Application.targetFrameRate = 60;
			break;
		default:
			Application.targetFrameRate = 45;
			Time.fixedDeltaTime = 2f / 45f;
			break;
		}
	}

	private void Start()
	{
		Vector3 position = characters.position;
		position.x = charactersStartPos;
		characters.position = position;
		characters.gameObject.SetActive(false);
		characters.gameObject.SetActive(true);
		sleepyLogo.localScale = Vector3.zero;
		StartCoroutine(LoadSequence());
	}

	private void Update()
	{
		m_timer += Time.deltaTime;
		if (m_timer < duration)
		{
			characters.position += new Vector3(speed * Time.deltaTime, 0f, 0f);
			if (!m_sleepyLogoDropped && characters.position.x > sleepyLogo.position.x)
			{
				m_sleepyLogoDropped = true;
				Go.to(sleepyLogo, sleepyLogoShowDuration, new GoTweenConfig().scale(Vector3.one).setEaseType(sleepyLogoShowEaseType));
			}
		}
	}

	private IEnumerator LoadSequence()
	{
		float timeRemaining = duration - m_timer;
		if (timeRemaining > 0f)
		{
			yield return new WaitForSeconds(timeRemaining);
		}
		Application.backgroundLoadingPriority = ThreadPriority.High;
		AsyncOperation levelLoad = Application.LoadLevelAsync("ski");
		levelLoad.allowSceneActivation = false;
        levelLoad.allowSceneActivation = true;
        while (levelLoad.progress < 0.9f)
		{
			yield return null;
		}
		GameState.Synchronise();
        
        {
            while (!LicenseManager.Instance.HasFinishedOrTimedOut)
            {
                yield return null;
            }
        }
    
            
		levelLoad.allowSceneActivation = true;
	}
}
