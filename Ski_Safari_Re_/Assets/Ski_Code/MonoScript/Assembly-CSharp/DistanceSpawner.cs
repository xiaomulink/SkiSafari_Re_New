using System;
using UnityEngine;

public class DistanceSpawner : MonoBehaviour
{
	[Serializable]
	public class Bracket
	{
		public int maxDistance;

		public int interval = 1000;
	}

	public static DistanceSpawner Instance;

	public SpawnParams spawnParams;
	public SpawnParams FinishspawnParams;
	public SpawnParams RockspawnParams;

	public Bracket[] brackets;

	public float maxSlope = 45f;

	public bool matchSlopeRotation;

	private int m_lastPosX;

	private int m_nextPosX;

	private int m_bracketIndex;
    private int m_lastrockPosX;

	private int m_nextrockPosX;

	private int m_bracketrockIndex;

	private void Awake()
	{
		Instance = this;
	}

	private void OnEnable()
	{
		m_lastPosX = 0;
        m_lastrockPosX = 100;
		m_nextPosX = brackets[0].interval;
        m_nextrockPosX = UnityEngine.Random.Range(20, 100)+ m_lastrockPosX;
    }

	private void Update()
	{
        Player instance = Player.Instance;
        if (SkiGameManager.Instance.isOnline)
        {
            if ((bool)instance && !SkiGameManager.Instance.TitleScreenActive && Camera.main.transform.position.x >= (float)m_lastrockPosX)
            {
                Debug.LogError("Rocks");
                SpawnManager.ForegroundInstance.QueueSpawn(RockspawnParams, SpawnManager.SpawnFlags.None, m_nextrockPosX);

                m_lastrockPosX = m_nextrockPosX;
                m_nextrockPosX = UnityEngine.Random.Range(70, 150) + m_lastrockPosX;
            }
        }
		if ((bool)instance && !SkiGameManager.Instance.TitleScreenActive && Camera.main.transform.position.x >= (float)m_lastPosX)
		{
            if (m_nextPosX >= 5000&& SkiGameManager.Instance.isOnline)
            {
                SpawnManager.ForegroundInstance.QueueSpawn(FinishspawnParams, SpawnManager.SpawnFlags.None, m_nextPosX);
            }
            else
            {
                SpawnManager.ForegroundInstance.QueueSpawn(spawnParams, SpawnManager.SpawnFlags.None, m_nextPosX);
                m_lastPosX = m_nextPosX;
                m_nextPosX += brackets[m_bracketIndex].interval;
                if (m_nextPosX >= brackets[m_bracketIndex].maxDistance && m_bracketIndex < brackets.Length - 1)
                {
                    m_bracketIndex++;
                }
            }
		}
	}
}
