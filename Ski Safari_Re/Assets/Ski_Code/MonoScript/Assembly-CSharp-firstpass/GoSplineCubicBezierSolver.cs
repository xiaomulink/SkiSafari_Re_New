using System.Collections.Generic;
using UnityEngine;

public class GoSplineCubicBezierSolver : AbstractGoSplineSolver
{
	public GoSplineCubicBezierSolver(List<Vector3> nodes)
	{
		_nodes = nodes;
	}

	public override void closePath()
	{
	}

	public override Vector3 getPoint(float t)
	{
		float num = 1f - t;
		return num * num * num * _nodes[0] + 3f * num * num * t * _nodes[2] + 3f * num * t * t * _nodes[3] + t * t * t * _nodes[1];
	}

	public override void drawGizmos()
	{
		Color color = Gizmos.color;
		Gizmos.color = Color.red;
		Gizmos.DrawLine(_nodes[0], _nodes[2]);
		Gizmos.DrawLine(_nodes[3], _nodes[1]);
		Gizmos.color = color;
	}
}
