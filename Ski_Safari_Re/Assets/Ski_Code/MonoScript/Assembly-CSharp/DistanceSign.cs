using UnityEngine;

public class DistanceSign : MonoBehaviour
{
	public TextMesh textMesh;

	public int step = 500;

	public bool best;

	public void SetDistance(int distance)
	{
		textMesh.text = string.Format("{0}m", distance);
	}

	private void OnEnable()
	{
		if (best)
		{
			float @float = GameState.GetFloat("best_distance");
			SetDistance(Mathf.CeilToInt(@float));
		}
		else
		{
			float x = base.transform.position.x;
			int distance = Mathf.FloorToInt(x / (float)step) * step;
			SetDistance(distance);
		}
	}
}
