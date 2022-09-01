using System.Collections.Generic;
using UnityEngine;

public class GoSplineCatmullRomSolver : AbstractGoSplineSolver
{
	public GoSplineCatmullRomSolver(List<Vector3> nodes)
	{
		_nodes = nodes;
	}

	public override void closePath()
	{
		_nodes.RemoveAt(0);
		_nodes.RemoveAt(_nodes.Count - 1);
		if (_nodes[0] != _nodes[_nodes.Count - 1])
		{
			_nodes.Add(_nodes[0]);
		}
		float num = Vector3.Distance(_nodes[0], _nodes[1]);
		float num2 = Vector3.Distance(_nodes[0], _nodes[_nodes.Count - 2]);
		float num3 = num2 / Vector3.Distance(_nodes[1], _nodes[0]);
		Vector3 item = _nodes[0] + (_nodes[1] - _nodes[0]) * num3;
		float num4 = num / Vector3.Distance(_nodes[_nodes.Count - 2], _nodes[0]);
		Vector3 item2 = _nodes[0] + (_nodes[_nodes.Count - 2] - _nodes[0]) * num4;
		_nodes.Insert(0, item2);
		_nodes.Add(item);
	}

	public override Vector3 getPoint(float t)
	{
		int num = _nodes.Count - 3;
		int num2 = Mathf.Min(Mathf.FloorToInt(t * (float)num), num - 1);
		float num3 = t * (float)num - (float)num2;
		Vector3 vector = _nodes[num2];
		Vector3 vector2 = _nodes[num2 + 1];
		Vector3 vector3 = _nodes[num2 + 2];
		Vector3 vector4 = _nodes[num2 + 3];
		return 0.5f * ((-vector + 3f * vector2 - 3f * vector3 + vector4) * (num3 * num3 * num3) + (2f * vector - 5f * vector2 + 4f * vector3 - vector4) * (num3 * num3) + (-vector + vector3) * num3 + 2f * vector2);
	}

	public override void drawGizmos()
	{
		if (_nodes.Count >= 2)
		{
			Color color = Gizmos.color;
			Gizmos.color = new Color(1f, 1f, 1f, 0.3f);
			Gizmos.DrawLine(_nodes[0], _nodes[1]);
			Gizmos.DrawLine(_nodes[_nodes.Count - 1], _nodes[_nodes.Count - 2]);
			Gizmos.color = color;
		}
	}
}
