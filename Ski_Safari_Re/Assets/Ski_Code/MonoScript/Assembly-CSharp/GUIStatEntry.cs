using UnityEngine;

public class GUIStatEntry : MonoBehaviour
{
	public GUIDropShadowText descriptionText;

	public GUIDropShadowText valueText;

	public float descriptionTextOffset;

	public float valueCharacterWidth = 0.4f;

	public float separatorCharacterWidth = 0.2f;

	private StatDescriptor m_statDescriptor;

	public StatDescriptor StatDescriptor
	{
		get
		{
			return m_statDescriptor;
		}
		set
		{
			m_statDescriptor = value;
			valueText.Text = m_statDescriptor.Value.ToString("N0") + m_statDescriptor.valueSuffix;
			descriptionText.Text = m_statDescriptor.description;
			Vector3 localPosition = descriptionText.transform.localPosition;
			int num = Mathf.FloorToInt(Mathf.Log10(m_statDescriptor.Value) + 1f);
			int num2 = (num - 1) / 3;
			localPosition.x = (float)num * valueCharacterWidth + (float)num2 * separatorCharacterWidth + descriptionTextOffset;
			descriptionText.transform.localPosition = localPosition;
		}
	}
}
