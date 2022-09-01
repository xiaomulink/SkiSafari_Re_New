using UnityEngine;

public class GeometryUtils
{
	public struct ContactInfo
	{
		public Vector3 position;

		public Vector3 normal;

		public float distance;

		public float normalSpeed;

		public float angle;

		public bool isPoint;

		public bool isOffScreen;

		public LineCollider collider;

		public LineCollider.Line colliderLine;

		public bool IsValid()
		{
			return distance != float.MaxValue;
		}

		public void Invalidate()
		{
			distance = float.MaxValue;
			isPoint = false;
			isOffScreen = false;
			collider = null;
		}
	}

	public static bool CircleLineIntersection(Vector3 start, Vector3 dir, float castDistance, float radius, Vector3 p1, Vector3 p2, bool allowPointCollisions, ref ContactInfo contactInfo)
	{
		Vector3 vector = dir * castDistance;
		Vector3 vector2 = p2 - p1;
		Vector3 normalized = vector2.normalized;
		Vector3 vector3 = new Vector3(0f - normalized.y, normalized.x, 0f);
		float num = Vector3.Dot(dir, normalized);
		if (Mathf.Approximately(Mathf.Abs(num), 1f))
		{
			if (allowPointCollisions)
			{
				Vector3 point = ((!(num > 0f)) ? p2 : p1);
				return CirclePointIntersection(start, dir, castDistance, radius, point, ref contactInfo);
			}
			return false;
		}
		if (Vector3.Dot(dir, vector3) >= 0f)
		{
			return false;
		}
		float f = Mathf.Acos(num);
		float num2 = vector2.y * vector.x - vector2.x * vector.y;
		if (Mathf.Approximately(num2, 0f))
		{
			if (allowPointCollisions)
			{
				Vector3 point2 = ((!(num > 0f)) ? p2 : p1);
				return CirclePointIntersection(start, dir, castDistance, radius, point2, ref contactInfo);
			}
			return false;
		}
		float magnitude = vector2.magnitude;
		float num3 = (vector.x * (start.y - p1.y) - vector.y * (start.x - p1.x)) / num2 * magnitude;
		float num4 = radius / Mathf.Tan(f);
		num3 -= num4;
		if (num3 < 0f || num3 > magnitude)
		{
			if (allowPointCollisions)
			{
				Vector3 point3 = ((!(num3 > 0f)) ? p1 : p2);
				return CirclePointIntersection(start, dir, castDistance, radius, point3, ref contactInfo);
			}
			return false;
		}
		float num5 = (vector2.x * (start.y - p1.y) - vector2.y * (start.x - p1.x)) / num2;
		float num6 = radius / Mathf.Sin(f);
		float num7 = num5 * castDistance - num6;
		if (num7 < 0f - radius || num7 > castDistance)
		{
			return false;
		}
		contactInfo.distance = num7;
		contactInfo.position = start + dir * num7;
		contactInfo.normal = vector3;
		if (Vector3.Dot(contactInfo.normal, dir) > 0f)
		{
			contactInfo.normal = -contactInfo.normal;
		}
		return true;
	}

	private static bool CirclePointIntersection(Vector3 start, Vector3 dir, float castDistance, float radius, Vector3 point, ref ContactInfo contactInfo)
	{
		Vector3 vector = point - start;
		float magnitude = vector.magnitude;
		if (magnitude <= Mathf.Epsilon)
		{
			return false;
		}
		Vector3 rhs = vector / magnitude;
		float num = Vector3.Dot(dir, rhs);
		float f = Mathf.Acos(num);
		float num2 = magnitude * Mathf.Sin(f);
		if (num2 > radius)
		{
			return false;
		}
		float num3 = num * magnitude;
		float num4 = 4f * radius * radius - 4f * num2 * num2;
		float num5 = num3;
		if (!Mathf.Approximately(num4, 0f))
		{
			num5 -= Mathf.Sqrt(num4) / 2f;
		}
		if (num5 < 0f || num5 > castDistance)
		{
			return false;
		}
		contactInfo.distance = num5;
		contactInfo.position = start + dir * num5;
		contactInfo.normal = (contactInfo.position - point).normalized;
		contactInfo.normal.Normalize();
		contactInfo.isPoint = true;
		return true;
	}

	public static bool LinePointIntersection2D(Vector3 p1, Vector3 p2, Vector3 p3, Vector3 p4, ref Vector3 intersection)
	{
		float num = (p4.y - p3.y) * (p2.x - p1.x) - (p4.x - p3.x) * (p2.y - p1.y);
		if (Mathf.Approximately(num, 0f))
		{
			return false;
		}
		float num2 = (p4.x - p3.x) * (p1.y - p3.y) - (p4.y - p3.y) * (p1.x - p3.x);
		float num3 = num2 / num;
		if (num3 < 0f || num3 > 1f)
		{
			return false;
		}
		intersection.x = p1.x + num3 * (p2.x - p1.x);
		intersection.y = p1.y + num3 * (p2.y - p1.y);
		intersection.z = 0f;
		return true;
	}

	public static void CalculateNormal(Vector3 p1, Vector3 p2, out Vector3 normal)
	{
		normal.x = p1.y - p2.y;
		normal.y = p2.x - p1.x;
		normal.z = 0f;
		normal.Normalize();
	}
}
