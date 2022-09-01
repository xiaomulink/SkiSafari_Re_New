using UnityEngine;

public class FontUtils : MonoBehaviour
{
	public static float GetNumericStringWidth(TextMesh textMesh, int value)
	{
		float num = textMesh.characterSize * 5f;
		float num2 = num * 0.5f;
		int num3 = Mathf.FloorToInt(Mathf.Log10(value) + 1f);
		int num4 = (num3 - 1) / 3;
		return (float)num3 * num + (float)num4 * num2;
	}
}
