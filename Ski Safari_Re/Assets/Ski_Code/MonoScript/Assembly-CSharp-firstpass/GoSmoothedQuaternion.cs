using UnityEngine;

public struct GoSmoothedQuaternion
{
	public GoSmoothingType smoothingType;

	public float duration;

	private Quaternion _currentValue;

	private Quaternion _target;

	private Quaternion _start;

	private float _startTime;

	public Quaternion smoothValue
	{
		get
		{
			float t = (Time.time - _startTime) / duration;
			switch (smoothingType)
			{
			case GoSmoothingType.Lerp:
				_currentValue = Quaternion.Lerp(_start, _target, t);
				break;
			case GoSmoothingType.Slerp:
				_currentValue = Quaternion.Slerp(_start, _target, t);
				break;
			}
			return _currentValue;
		}
		set
		{
			_start = smoothValue;
			_startTime = Time.time;
			_target = value;
		}
	}

	public float x
	{
		get
		{
			return _currentValue.x;
		}
		set
		{
			smoothValue = new Quaternion(value, _target.y, _target.z, _target.w);
		}
	}

	public float y
	{
		get
		{
			return _currentValue.y;
		}
		set
		{
			smoothValue = new Quaternion(_target.x, value, _target.y, _target.w);
		}
	}

	public float z
	{
		get
		{
			return _currentValue.z;
		}
		set
		{
			smoothValue = new Quaternion(_target.x, _target.y, value, _target.w);
		}
	}

	public float w
	{
		get
		{
			return _currentValue.w;
		}
		set
		{
			smoothValue = new Quaternion(_target.x, _target.y, _target.z, value);
		}
	}

	public GoSmoothedQuaternion(Quaternion quat)
	{
		_currentValue = quat;
		_start = quat;
		_target = quat;
		_startTime = Time.time;
		duration = 0.2f;
		smoothingType = GoSmoothingType.Lerp;
	}

	public static implicit operator GoSmoothedQuaternion(Quaternion q)
	{
		return new GoSmoothedQuaternion(q);
	}
}
