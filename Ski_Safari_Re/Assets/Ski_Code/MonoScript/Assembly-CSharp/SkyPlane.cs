using System;
using System.Collections.Generic;
using UnityEngine;

public class SkyPlane : MonoBehaviour
{
	[Serializable]
	public class CloudLayer
	{
		public Transform[] prefabs;

		public float minSeparation = 100f;

		public float maxSeparation = 200f;

		public float minHeight = 0.75f;

		public float maxHeight = 0.75f;

		public float moveSpeed = 100f;

		public float depthOffset = -200f;

		public float scale = 1f;

		private List<Transform> m_instances = new List<Transform>();

		private float m_minPosX;

		private float m_maxPosX;

		public void Generate(Transform transform)
		{
			m_minPosX = -0.5f - minSeparation;
			m_maxPosX = 0.5f + minSeparation;
			float num = -0.5f + Mathf.Lerp(0f - minSeparation, 0f, UnityEngine.Random.value);
			for (float num2 = 0.5f + Mathf.Lerp(0f, minSeparation, UnityEngine.Random.value); num < num2; num += UnityEngine.Random.Range(minSeparation, maxSeparation))
			{
				Transform transform2 = prefabs[UnityEngine.Random.Range(0, prefabs.Length)];
				Transform transform3 = UnityEngine.Object.Instantiate(transform2);
				transform3.localScale = transform2.localScale * scale;
				transform3.parent = transform;
				transform3.localPosition = new Vector3(num, UnityEngine.Random.Range(minHeight, maxHeight) - 0.5f, depthOffset);
				m_instances.Add(transform3);
			}
		}

		public void Update()
		{
			for (int i = 0; i < m_instances.Count; i++)
			{
				Vector3 localPosition = m_instances[i].localPosition;
				localPosition.x -= moveSpeed * Time.deltaTime;
				if (localPosition.x <= m_minPosX)
				{
					localPosition.x += m_maxPosX - m_minPosX;
				}
				m_instances[i].localPosition = localPosition;
			}
		}
	}

	public static SkyPlane Instance;

	public float verticalScale = 4f;

	public float verticalParallax = 0.1f;

	public CloudLayer[] cloudLayers;

	public Transform[] preservedScaleTransforms;

	private float m_verticalRange;

	public void SnapToCamera()
	{
		Camera main = Camera.main;
		base.transform.parent = main.transform;
		base.transform.localPosition = Vector3.zero;
		Transform[] array = preservedScaleTransforms;
		foreach (Transform transform in array)
		{
			transform.parent = null;
		}
		float z = main.farClipPlane - 1f;
		Vector3 vector = main.ViewportToWorldPoint(new Vector3(0f, 0.5f, z));
		float num = Mathf.Abs(base.transform.position.x - vector.x) * 2f;
		float num2 = num / main.aspect;
		base.transform.localScale = new Vector3(num, num2 * verticalScale, 1f);
		Transform[] array2 = preservedScaleTransforms;
		foreach (Transform transform2 in array2)
		{
			transform2.parent = base.transform;
		}
		m_verticalRange = (verticalScale - 1f) * num2 * 0.5f;
		base.transform.localPosition = new Vector3(0f, 0f - m_verticalRange, z);
		base.transform.localRotation = Quaternion.identity;
	}

	private void GenerateClouds(float width, float height)
	{
		CloudLayer[] array = cloudLayers;
		foreach (CloudLayer cloudLayer in array)
		{
			cloudLayer.Generate(base.transform);
		}
	}

	private void Awake()
	{
		Instance = this;
	}

	private void Start()
	{
		SnapToCamera();
		GenerateClouds(base.transform.localScale.x, base.transform.localScale.y);
	}

	private void Update()
	{
		Vector3 localPosition = base.transform.localPosition;
		float value = 0f - m_verticalRange - FollowCamera.Instance.transform.position.y * verticalParallax;
		localPosition.y = Mathf.Clamp(value, 0f - m_verticalRange, m_verticalRange);
		base.transform.localPosition = localPosition;
		CloudLayer[] array = cloudLayers;
		foreach (CloudLayer cloudLayer in array)
		{
			cloudLayer.Update();
		}
	}
}
