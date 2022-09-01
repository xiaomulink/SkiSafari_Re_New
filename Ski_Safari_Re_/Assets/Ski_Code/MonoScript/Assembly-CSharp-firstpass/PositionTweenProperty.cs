using UnityEngine;

public class PositionTweenProperty : AbstractVector3TweenProperty
{
	protected bool _useLocalPosition;

	public bool useLocalPosition
	{
		get
		{
			return _useLocalPosition;
		}
	}

	public PositionTweenProperty(Vector3 endValue, bool isRelative = false, bool useLocalPosition = false)
		: base(endValue, isRelative)
	{
		_useLocalPosition = useLocalPosition;
	}

	public override int GetHashCode()
	{
		return base.GetHashCode();
	}

	public override bool Equals(object obj)
	{
		if (base.Equals(obj))
		{
			return _useLocalPosition == ((PositionTweenProperty)obj)._useLocalPosition;
		}
		PositionPathTweenProperty positionPathTweenProperty = obj as PositionPathTweenProperty;
		if (positionPathTweenProperty != null)
		{
			return _useLocalPosition == positionPathTweenProperty.useLocalPosition;
		}
		return false;
	}

	public override void prepareForUse()
	{
		_target = _ownerTween.target as Transform;
		_endValue = _originalEndValue;
		if (_ownerTween.isFrom)
		{
			_startValue = _endValue;
			if (_useLocalPosition)
			{
				_endValue = _target.localPosition;
			}
			else
			{
				_endValue = _target.position;
			}
		}
		else if (_useLocalPosition)
		{
			_startValue = _target.localPosition;
		}
		else
		{
			_startValue = _target.position;
		}
		base.prepareForUse();
	}

	public override void tick(float totalElapsedTime)
	{
		float value = _easeFunction(totalElapsedTime, 0f, 1f, _ownerTween.duration);
		Vector3 vector = GoTweenUtils.unclampedVector3Lerp(_startValue, _diffValue, value);
		if (_useLocalPosition)
		{
			_target.localPosition = vector;
		}
		else
		{
			_target.position = vector;
		}
	}
}
